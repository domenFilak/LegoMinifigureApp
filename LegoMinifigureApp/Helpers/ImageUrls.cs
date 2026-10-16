namespace LegoMinifigureApp.Helpers
{
    public static class ImageUrls
    {
        private const string MinifigurePlaceholder = "/images/minifigure-placeholder.svg";
        public static string ForMinifigure(string? imagePath) => string.IsNullOrWhiteSpace(imagePath) ? MinifigurePlaceholder : "/" + imagePath.TrimStart('/');
    }
}
