using LegoMinifigureApp.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace LegoMinifigureApp.Tests.Helpers
{
    public class TooltipTextTests
    {
        [Fact]
        public void AskingPrice_WithPrice_ShowsPrice()
        {
            var text = TooltipText.AskingPrice(12.5m);

            Assert.Equal("Listed for sale at 12.50 €", text);
        }

        [Fact]
        public void AskingPrice_NoPrice_SaysSo()
        {
            var text = TooltipText.AskingPrice(null);

            Assert.Equal("No asking price", text);
        }

        [Fact]
        public void TradeNotes_WithNotes_ReturnsNotes()
        {
            var text = TooltipText.TradeNotes("Looking for Space figures");

            Assert.Equal("Looking for Space figures", text);
        }

        [Fact]
        public void TradeNotes_NoNotes_SaysSo()
        {
            var text = TooltipText.TradeNotes(null);

            Assert.Equal("No trade notes", text);
        }
    }
}
