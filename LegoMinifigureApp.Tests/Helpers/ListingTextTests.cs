using LegoMinifigureApp.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace LegoMinifigureApp.Tests.Helpers
{
    public class ListingTextTests
    {
        [Fact]
        public void Describe_ForSaleAndForTrade_MentionsBoth()
        {
            var text = ListingText.Describe(isForSale: true, askingPrice: 12.5m, isForTrade: true);

            Assert.Equal("for sale at 12.50 € and for trade", text);
        }

        [Fact]
        public void Describe_OnlyForSale_MentionsPrice()
        {
            var text = ListingText.Describe(isForSale: true, askingPrice: 12.5m, isForTrade: false);

            Assert.Equal("for sale at 12.50 €", text);
        }

        [Fact]
        public void Describe_OnlyForTrade_SaysForTrade()
        {
            var text = ListingText.Describe(isForSale: false, askingPrice: null, isForTrade: true);

            Assert.Equal("for trade", text);
        }

        [Fact]
        public void Describe_NotListed_ReturnsNull()
        {
            var text = ListingText.Describe(isForSale: false, askingPrice: null, isForTrade: false);

            Assert.Null(text);
        }
    }
}
