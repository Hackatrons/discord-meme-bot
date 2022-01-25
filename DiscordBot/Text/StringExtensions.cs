namespace DiscordBot.Text
{
    /// <summary>
    /// Contains extension methods for strings
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Performs a case insensitive search
        /// </summary>
        public static bool ContainsIgnoreCase(this string s, string value)
            => s.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
    }
}
