using System.Globalization;

namespace LegoMinifigureApp.Helpers
{
    public static class PriceText
    {
        public static string? Format(decimal? amount)
        {
            if (!amount.HasValue)
            {
                return null;
            }

            return amount.Value.ToString("0.00", CultureInfo.InvariantCulture) + " €";
        }
    }
}
