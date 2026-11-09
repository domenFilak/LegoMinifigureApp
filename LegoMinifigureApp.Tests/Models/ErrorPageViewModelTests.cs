using LegoMinifigureApp.Models.ViewModels.Errors;
using System;
using System.Collections.Generic;
using System.Text;

namespace LegoMinifigureApp.Tests.Models
{
    public class ErrorPageViewModelTests
    {
        [Fact]
        public void ForStatusCode_404_IsYellowNotFoundPage()
        {
            var model = ErrorPageViewModel.ForStatusCode(404);

            Assert.Equal(404, model.StatusCode);
            Assert.Equal("error-brick-yellow", model.BrickClass);
            Assert.False(string.IsNullOrWhiteSpace(model.Title));
            Assert.Null(model.ReferenceId);
        }

        [Theory]
        [InlineData(400)]
        [InlineData(403)]
        [InlineData(405)]
        [InlineData(429)]
        public void ForStatusCode_OtherClientErrors_AreBluePageWithSameCode(int statusCode)
        {
            var model = ErrorPageViewModel.ForStatusCode(statusCode);

            Assert.Equal(statusCode, model.StatusCode);
            Assert.Equal("error-brick-blue", model.BrickClass);
        }

        [Theory]
        [InlineData(500)]
        [InlineData(503)]
        public void ForStatusCode_ServerErrors_AreRedTiltedPageWithSameCode(int statusCode)
        {
            var model = ErrorPageViewModel.ForStatusCode(statusCode);

            Assert.Equal(statusCode, model.StatusCode);
            Assert.Contains("error-brick-red", model.BrickClass);
            Assert.Contains("error-brick-tilted", model.BrickClass);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(200)]
        [InlineData(302)]
        [InlineData(399)]
        [InlineData(600)]
        [InlineData(999)]
        public void ForStatusCode_NotAnErrorCode_IsTreatedAsNotFound(int statusCode)
        {
            var model = ErrorPageViewModel.ForStatusCode(statusCode);

            Assert.Equal(404, model.StatusCode);
        }

        [Fact]
        public void ForServerError_IsRedTilted500WithReference()
        {
            var model = ErrorPageViewModel.ForServerError("abc-123");

            Assert.Equal(500, model.StatusCode);
            Assert.Equal("abc-123", model.ReferenceId);
            Assert.Contains("error-brick-tilted", model.BrickClass);
        }
    }
}
