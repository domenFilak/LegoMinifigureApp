using LegoMinifigureApp.Services.Common;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace LegoMinifigureApp.Services.Storage
{
    public class LocalImageStorageService(IWebHostEnvironment environment, ILogger<LocalImageStorageService> logger) : IImageStorageService
    {
        public const long MaxFileSizeBytes = 5 * 1024 * 1024;
        public const long MaxPixels = 40000000;
        private const int MaxDimension = 2000; //longest side of stored image in pixels
        private const int Quality = 90;
        private const int ThumbnailMaxDimension = 600; //longest side of thumbnail in pixels
        private const int ThumbnailQuality = 80;
        public const string UploadsFolder = "uploads";

        private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];

        private static readonly byte[] JpegSignature = [0xFF, 0xD8, 0xFF];
        private static readonly byte[] PngSignature = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];
        private static readonly byte[] RiffSignature = [0x52, 0x49, 0x46, 0x46];
        private static readonly byte[] WebpSignature = [0x57, 0x45, 0x42, 0x50];


        public void Delete(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                return;
            }

            DeleteFile(relativePath); //call once to delete main image
            DeleteFile(ImagePaths.ThumbnailFor(relativePath)); //call second time to delete thumbnail image as well
        }

        public async Task<string> SaveAsync(ImageUpload upload, string folder, CancellationToken cancellationToken = default)
        {
            var extension = await DetectExtensionAsync(upload.Content, cancellationToken)
                ?? throw new InvalidOperationException("The image must be validated before it is saved.");

            upload.Content.Position = 0;
            using var image = await Image.LoadAsync(upload.Content, cancellationToken);

            //turn pixels the way photo was taken; before orientation info is dropped
            image.Mutate(x => x.AutoOrient());

            //shrink large photos so longest side is max MaxDimension
            if (image.Width > MaxDimension || image.Height > MaxDimension)
            {
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Mode = ResizeMode.Max,
                    Size = new Size(MaxDimension, MaxDimension)
                }));
            }

            //remove metadata from image
            image.Metadata.ExifProfile = null; //camera, date, GPS location
            image.Metadata.XmpProfile = null; //editing software data, maybe sometimes location
            image.Metadata.IptcProfile = null; //captions, author, copyright, location

            var fileName = $"{Guid.NewGuid():N}{extension}";
            var directory = Path.Combine(environment.WebRootPath, UploadsFolder, folder);
            Directory.CreateDirectory(directory);

            var relativePath = $"{UploadsFolder}/{folder}/{fileName}";
            var fullPath = Path.Combine(directory, fileName);

            //main image
            await using (var fileStream = new FileStream(fullPath, FileMode.CreateNew))
            {
                await image.SaveAsync(fileStream, CreateEncoder(extension), cancellationToken);
            }

            //small copy of main image for grids and lists
            try
            {
                await SaveThumbnailAsync(image, relativePath, cancellationToken);
            }
            catch
            {
                Delete(relativePath); //never leave main image without its thumbnail
                throw;
            }

            return relativePath;
        }

        public async Task<Error?> ValidateAsync(ImageUpload upload, CancellationToken cancellationToken = default)
        {
            if (upload.Length == 0)
            {
                return ImageErrors.Empty;
            }

            if (upload.Length > MaxFileSizeBytes)
            {
                return ImageErrors.TooLarge(MaxFileSizeBytes);
            }

            var extension = Path.GetExtension(upload.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
            {
                return ImageErrors.UnsupportedType;
            }

            var detectedExtension = await DetectExtensionAsync(upload.Content, cancellationToken);
            if (detectedExtension is null)
            {
                return ImageErrors.NotAnImage;
            }

            //check that image can really be read and how big it is
            try
            {
                upload.Content.Position = 0;
                var info = await Image.IdentifyAsync(upload.Content, cancellationToken);

                if ((long)info.Width * info.Height > MaxPixels)
                {
                    return ImageErrors.TooManyPixels(MaxPixels);
                }
            }
            catch (ImageFormatException)
            {
                return ImageErrors.NotAnImage;
            }
            finally
            {
                upload.Content.Position = 0;
            }

            return null;
        }

        //creates thumbnails for images that do not have one yet
        public async Task CreateMissingThumbnailsAsync(CancellationToken cancellationToken = default)
        {
            var uploadsRoot = Path.Combine(environment.WebRootPath, UploadsFolder);
            if (!Directory.Exists(uploadsRoot))
            {
                return;
            }

            var createdCount = 0;

            //each folder directly inside uploads (uploads/minifigures)
            foreach (var directory in Directory.GetDirectories(uploadsRoot))
            {
                var folderName = Path.GetFileName(directory);

                //only files directly in that folder (thumbnails folder itself is skipped)
                foreach (var filePath in Directory.GetFiles(directory))
                {
                    var extension = Path.GetExtension(filePath).ToLowerInvariant();
                    if (!AllowedExtensions.Contains(extension))
                    {
                        continue;
                    }

                    var imagePath = $"{UploadsFolder}/{folderName}/{Path.GetFileName(filePath)}";
                    var thumbnailFullPath = Path.Combine(environment.WebRootPath, ImagePaths.ThumbnailFor(imagePath));

                    if (File.Exists(thumbnailFullPath))
                    {
                        continue;
                    }

                    try
                    {
                        using var image = await Image.LoadAsync(filePath, cancellationToken);
                        await SaveThumbnailAsync(image, imagePath, cancellationToken);
                        createdCount++;
                    }
                    catch (Exception ex) when (ex is ImageFormatException or IOException)
                    {
                        logger.LogWarning(ex, "Could not create thumbnail for {Path}.", filePath);
                    }
                }
            }

            if (createdCount > 0)
            {
                logger.LogInformation("Created {Count} missing thumbnails.", createdCount);
            }
        }

        //picks encoder matching the detected format; turns pixels back into a file of one format
        private static ImageEncoder CreateEncoder(string extension)
        {
            return extension switch
            {
                ".png" => new PngEncoder { SkipMetadata = true },
                ".webp" => new WebpEncoder { Quality = Quality, SkipMetadata = true },
                _ => new JpegEncoder { Quality = Quality, SkipMetadata = true }
            };
        }

        //reads first bytes of the file
        private static async Task<string?> DetectExtensionAsync(Stream content, CancellationToken cancellationToken)
        {
            var header = new byte[12];

            content.Position = 0;
            var bytesRead = await content.ReadAtLeastAsync(header, header.Length, throwOnEndOfStream: false, cancellationToken);
            content.Position = 0; //next reader starts from beginning

            if (bytesRead >= 3 && header.AsSpan(0, 3).SequenceEqual(JpegSignature))
            {
                return ".jpg";
            }

            if (bytesRead >= 8 && header.AsSpan(0, 8).SequenceEqual(PngSignature))
            {
                return ".png";
            }

            if (bytesRead >= 12 && header.AsSpan(0, 4).SequenceEqual(RiffSignature) && header.AsSpan(8, 4).SequenceEqual(WebpSignature))
            {
                return ".webp";
            }

            return null;
        }

        //makes small copy of main image used in grids and lists; always WebP
        private async Task SaveThumbnailAsync(Image image, string imagePath, CancellationToken cancellationToken)
        {
            //copy of image, original stays untouched
            using var thumbnail = image.Clone(x => { });

            //shrink only if bigger than thumbnail size, small images are never enlarged
            if (thumbnail.Width > ThumbnailMaxDimension || thumbnail.Height > ThumbnailMaxDimension)
            {
                thumbnail.Mutate(x => x.Resize(new ResizeOptions
                {
                    Mode = ResizeMode.Max,
                    Size = new Size(ThumbnailMaxDimension, ThumbnailMaxDimension)
                }));
            }

            var thumbnailPath = ImagePaths.ThumbnailFor(imagePath);
            var thumbnailFullPath = Path.Combine(environment.WebRootPath, thumbnailPath);
            Directory.CreateDirectory(Path.GetDirectoryName(thumbnailFullPath)!);

            await using var fileStream = new FileStream(thumbnailFullPath, FileMode.Create);
            await thumbnail.SaveAsync(fileStream, new WebpEncoder { Quality = ThumbnailQuality, SkipMetadata = true }, cancellationToken);
        }

        private void DeleteFile(string relativePath)
        {
            var uploadsRoot = Path.GetFullPath(Path.Combine(environment.WebRootPath, UploadsFolder));
            var fullPath = Path.GetFullPath(Path.Combine(environment.WebRootPath, relativePath));

            //only delete files inside wwwroot/uploads
            if (!fullPath.StartsWith(uploadsRoot + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            {
                logger.LogWarning("Refused to delete {Path}: it is outside the uploads folder.", fullPath);
                return;
            }

            try
            {
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }
            catch (IOException ex)
            {
                logger.LogWarning(ex, "Could not delete image {Path}.", fullPath);
            }
        }
    }
}