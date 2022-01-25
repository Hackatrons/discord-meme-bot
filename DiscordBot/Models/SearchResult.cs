using DiscordBot.Language;
using DiscordBot.Pushshift.Models;

namespace DiscordBot.Models
{
    public record SearchResult
    {
        public string Url { get; init; } = "";
        public MediaType? MediaHint { get; init; }

        public static SearchResult FromPushshift(PushshiftResult result) => new()
        {
            Url = result.Url!.ThrowIfNullOrWhitespace(),
            MediaHint = result.PostHint switch
            {
                PostHint.Image => MediaType.Image,
                PostHint.RichVideo or PostHint.HostedVideo => MediaType.Video,
                _ => null,
            }
        };
    }
}
