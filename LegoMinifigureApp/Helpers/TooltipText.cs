using System.Net.NetworkInformation;

namespace LegoMinifigureApp.Helpers
{
    public static class TooltipText
    {
        public static string AskingPrice(decimal? askingPrice)
        {
            if (askingPrice is null)
            {
                return "No asking price";
            }

            return $"Listed for sale at {PriceText.Format(askingPrice)}";
        }

        public static string TradeNotes(string? tradeNotes)
        {
            if (tradeNotes is null)
            {
                return "No trade notes";
            }

            return tradeNotes;
        }
    }
}
