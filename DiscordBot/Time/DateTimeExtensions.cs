namespace DiscordBot.Time;

/// <summary>
/// Extension methods for datetimes.
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// Returns a datetime instance from the specified unix timestamp.
    /// </summary>
    public static DateTime UnixTimeStampToDateTime(double secondsSinceUnix) =>
        DateTime.UnixEpoch.AddSeconds(secondsSinceUnix);
}