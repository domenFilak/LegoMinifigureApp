using LegoMinifigureApp.Services.Common;

namespace LegoMinifigureApp.Services.Storage
{
    public class LocalImageStorageService(IWebHostEnvironment environment, ILogger<LocalImageStorageService> logger) : IImageStorageService
    {
        public const long MaxFileSizeBytes = 5 * 1024 * 1024;
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

            var fileName = $"{Guid.NewGuid():N}{extension}";
            var directory = Path.Combine(environment.WebRootPath, UploadsFolder, folder);
            Directory.CreateDirectory(directory);

            var fullPath = Path.Combine(directory, fileName);
            await using (var fileStream = new FileStream(fullPath, FileMode.CreateNew))
            {
                await upload.Content.CopyToAsync(fileStream, cancellationToken);
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

            return null;
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
