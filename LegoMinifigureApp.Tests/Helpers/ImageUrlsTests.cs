using LegoMinifigureApp.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace LegoMinifigureApp.Tests.Helpers
{
    public class ImageUrlsTests
    {
        private const string Placeholder = "/images/minifigure-placeholder.svg";

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void ForMinifigure_NoImage_ReturnsPlaceholder(string? imagePath)
        {
            var url = ImageUrls.ForMinifigure(imagePath);

            Assert.Equal(Placeholder, url);
        }

        [Theory]
        [InlineData("uploads/minifigures/abc.jpg")]
        [InlineData("/uploads/minifigures/abc.jpg")]
        public void ForMinifigure_WithImage_ReturnsUrlWithOneLeadingSlash(string imagePath)
        {
            var url = ImageUrls.ForMinifigure(imagePath);

            Assert.Equal("/uploads/minifigures/abc.jpg", url);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ForMinifigureThumbnail_NoImage_ReturnsPlaceholder(string? imagePath)
        {
            var url = ImageUrls.ForMinifigureThumbnail(imagePath);

            Assert.Equal(Placeholder, url);
        }

        [Theory]
        [InlineData("uploads/minifigures/abc.jpg")]
        [InlineData("/uploads/minifigures/abc.jpg")]
        public void ForMinifigureThumbnail_WithImage_ReturnsThumbnailUrl(string imagePath)
        {
            var url = ImageUrls.ForMinifigureThumbnail(imagePath);

            Assert.Equal("/uploads/minifigures/thumbs/abc.webp", url);
        }
    }
}
