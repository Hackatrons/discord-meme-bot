namespace DiscordBot.Time;

public static class DateTimeExtensions
{
    public static DateTime UnixTimeStampToDateTime(double unixTimeStamp) =>
        DateTime.UnixEpoch.AddSeconds(unixTimeStamp);
}