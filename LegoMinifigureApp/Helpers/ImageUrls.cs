using LegoMinifigureApp.Services.Storage;

namespace LegoMinifigureApp.Helpers
{
    public static class ImageUrls
    {
        private const string MinifigurePlaceholder = "/images/minifigure-placeholder.svg";

        //full image
        public static string ForMinifigure(string? imagePath) => string.IsNullOrWhiteSpace(imagePath) ? MinifigurePlaceholder : "/" + imagePath.TrimStart('/');

        //small copy of full image
        public static string ForMinifigureThumbnail(string? imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                return MinifigurePlaceholder;
            }

            return "/" + ImagePaths.ThumbnailFor(imagePath.TrimStart('/'));
        }
    }
}
