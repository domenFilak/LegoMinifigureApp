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
        private const string UploadsFolder = "uploads";

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

            var fileName = $"{Guid.NewGuid():N}{extension}";
            var directory = Path.Combine(environment.WebRootPath, UploadsFolder, folder);
            Directory.CreateDirectory(directory);

            var fullPath = Path.Combine(directory, fileName);
            await using (var fileStream = new FileStream(fullPath, FileMode.CreateNew))
            {
                await image.SaveAsync(fileStream, CreateEncoder(extension), cancellationToken);
            }

            return $"{UploadsFolder}/{folder}/{fileName}";
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
    }
}
