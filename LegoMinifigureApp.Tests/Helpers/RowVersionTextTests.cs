using LegoMinifigureApp.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace LegoMinifigureApp.Tests.Helpers
{
    public class RowVersionTextTests
    {
        //typical sql server rowversion: 8 bytes
        private static readonly byte[] SampleRowVersion = [0, 0, 0, 0, 0, 0, 7, 209];

        [Fact]
        public void Encode_ReturnsBase64Text()
        {
            var text = RowVersionText.Encode(SampleRowVersion);

            Assert.Equal("AAAAAAAAB9E=", text);
        }

        [Fact]
        public void TryDecode_EncodedText_ReturnsSameBytes()
        {
            var text = RowVersionText.Encode(SampleRowVersion);

            var bytes = RowVersionText.TryDecode(text);

            Assert.Equal(SampleRowVersion, bytes);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void TryDecode_Empty_ReturnsNull(string? text)
        {
            var bytes = RowVersionText.TryDecode(text);

            Assert.Null(bytes);
        }

        [Theory]
        [InlineData("%%%")]
        [InlineData("not base64!")]
        public void TryDecode_InvalidText_ReturnsNullInsteadOfCrashing(string text)
        {
            var bytes = RowVersionText.TryDecode(text);

            Assert.Null(bytes);
        }
    }
}
