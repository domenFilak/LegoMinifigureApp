namespace LegoMinifigureApp.Helpers
{
    public static class ListingText
    {
        public static string? Describe(bool isForSale, decimal? askingPrice, bool isForTrade)
        {
            if (isForSale && isForTrade)
            {
                return $"for sale at {PriceText.Format(askingPrice)} and for trade";
            }

            if (isForSale)
            {
                return $"for sale at {PriceText.Format(askingPrice)}";
            }

            if (isForTrade)
            {
                return "for trade";
            }

            return null;
        }
    }
}
