using LegoMinifigureApp.Services.Common;

namespace LegoMinifigureApp.Services.Storage
{
    public static class ImageErrors
    {
        public static Error Empty => Error.Validation("The image file is empty.");

        public static Error TooLarge(long maxBytes) => Error.Validation($"The image can be at most {maxBytes / (1024 * 1024)} MB.");

        public static Error UnsupportedType => Error.Validation("Only JPG, PNG and WebP images are allowed.");

        public static Error NotAnImage => Error.Validation("The file is not a valid JPG, PNG or WebP image.");

        public static Error TooManyPixels(long maxPixels) => Error.Validation($"The image is too large: at most {maxPixels / 1_000_000} megapixels are allowed.");
    }
}
