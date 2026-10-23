namespace LegoMinifigureApp.Helpers
{
    public static class RowVersionText
    {
        public static string Encode(byte[] rowVersion)
        {
            return Convert.ToBase64String(rowVersion);
        }

        public static byte[]? TryDecode(string? text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }

            try
            {
                return Convert.FromBase64String(text);
            }
            catch (FormatException)
            {
                return null;
            }
        }
    }
}
