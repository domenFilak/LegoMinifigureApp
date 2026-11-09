using LegoMinifigureApp.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace LegoMinifigureApp.Tests.Helpers
{
    public class PriceTextTests
    {
        [Theory]
        [InlineData(12.5, "12.50 €")]
        [InlineData(12, "12.00 €")]
        [InlineData(0.99, "0.99 €")]
        [InlineData(1234.5, "1234.50 €")]
        public void Format_Amount_ReturnsTwoDecimalsAndEuroSign(double amount, string expected)
        {
            var text = PriceText.Format((decimal)amount);

            Assert.Equal(expected, text);
        }

        [Fact]
        public void Format_NoAmount_ReturnsNull()
        {
            var text = PriceText.Format(null);

            Assert.Null(text);
        }
    }
}
