namespace LegoMinifigureApp.Services.Storage
{
    //this class knows where thumbnails (small pictures) of full sized minifig pictures are
    public static class ImagePaths
    {
        public const string ThumbnailFolder = "thumbs";
        public const string ThumbnailExtension = ".webp";

        //"uploads/minifigures/abc.jpg" => "uploads/minifigures/thumbs/abc.webp"
        //calculate where the thumbnail lives, based on main images path
        public static string ThumbnailFor(string imagePath)
        {
            var lastSlash = imagePath.LastIndexOf('/');

            string folder;
            string fileName;

            if (lastSlash >= 0)
            {
                folder = imagePath.Substring(0, lastSlash);
                fileName = imagePath.Substring(lastSlash + 1);
            }
            else
            {
                folder = string.Empty;
                fileName = imagePath;
            }

            var nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

            if (folder.Length == 0)
            {
                return $"{ThumbnailFolder}/{nameWithoutExtension}{ThumbnailExtension}";
            }

            return $"{folder}/{ThumbnailFolder}/{nameWithoutExtension}{ThumbnailExtension}";
        }
    }
}