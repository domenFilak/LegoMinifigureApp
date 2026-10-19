using System.Globalization;

namespace LegoMinifigureApp.Helpers
{
    public static class UtcDates
    {
        //machine readable ISO 8601 value for <time datetime="..."> attribute, always ending in "Z"
        public static string ToIso(DateTime utcDateTime)
        {
            return DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc).ToString("o", CultureInfo.InvariantCulture);
        }

        //readable fallback text, if js doesn't run
        public static string ToFallbackText(DateTime utcDateTime)
        {
            return utcDateTime.ToString("dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture) + " UTC";
        }
    }
}
