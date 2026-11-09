using LegoMinifigureApp.Services.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace LegoMinifigureApp.Tests.Services.Storage
{
    public class ImagePathsTests
    {
        [Theory]
        [InlineData("uploads/minifigures/abc.jpg", "uploads/minifigures/thumbs/abc.webp")]
        [InlineData("uploads/minifigures/abc.png", "uploads/minifigures/thumbs/abc.webp")]
        [InlineData("uploads/minifigures/abc.webp", "uploads/minifigures/thumbs/abc.webp")]
        [InlineData("uploads/other/folder/xyz.jpeg", "uploads/other/folder/thumbs/xyz.webp")]
        public void ThumbnailFor_ImageInFolder_ReturnsWebpInThumbsSubfolder(string imagePath, string expected)
        {
            var thumbnailPath = ImagePaths.ThumbnailFor(imagePath);

            Assert.Equal(expected, thumbnailPath);
        }

        [Fact]
        public void ThumbnailFor_ImageWithoutFolder_ReturnsThumbsFolder()
        {
            var thumbnailPath = ImagePaths.ThumbnailFor("abc.jpg");

            Assert.Equal("thumbs/abc.webp", thumbnailPath);
        }

        [Fact]
        public void ThumbnailFor_AlwaysUsesForwardSlashes()
        {
            var thumbnailPath = ImagePaths.ThumbnailFor("uploads/minifigures/abc.jpg");

            Assert.DoesNotContain("\\", thumbnailPath);
        }
    }
}
