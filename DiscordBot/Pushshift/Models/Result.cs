// ReSharper disable UnusedMember.Global

namespace DiscordBot.Pushshift.Models;

public class Result
{
    public long? ApprovedAtUtc { get; set; }
    public string? Author { get; set; }
    public string? AuthorFlairCssClass { get; set; }
    public string? AuthorFlairText { get; set; }
    public long? BannedAtUtc { get; set; }
    public bool? BrandSafe { get; set; }
    public bool? CanModPost { get; set; }
    public bool? ContestMode { get; set; }
    public long? CreatedUtc { get; set; }
    public string? Domain { get; set; }
    public string? FullLink { get; set; }
    public string? Id { get; set; }
    public bool? IsSelf { get; set; }
    public bool? IsVideo { get; set; }
    public string? LinkFlairText { get; set; }
    public bool? Locked { get; set; }
    public Media? Media { get; set; }
    public MediaEmbed? MediaEmbed { get; set; }
    public long? NumComments { get; set; }
    public bool? Over18 { get; set; }
    public string? Permalink { get; set; }
    public string? PostHint { get; set; }
    public Preview? Preview { get; set; }
    public long? RetrievedOn { get; set; }
    public long? Score { get; set; }
    public Media? SecureMedia { get; set; }
    public MediaEmbed? SecureMediaEmbed { get; set; }
    public bool? Spoiler { get; set; }
    public bool? Stickied { get; set; }
    public string? Subreddit { get; set; }
    public string? SubredditId { get; set; }
    public object? SuggestedSort { get; set; }
    public string? Thumbnail { get; set; }
    public long? ThumbnailHeight { get; set; }
    public long? ThumbnailWidth { get; set; }
    public string? Title { get; set; }
    public string? Url { get; set; }
    public long? ViewCount { get; set; }
    public FlairRichtext[]? AuthorFlairRichtext { get; set; }
    public string? AuthorFlairType { get; set; }
    public bool? IsCrosspostable { get; set; }
    public bool? IsRedditMediaDomain { get; set; }
    public string? LinkFlairBackgroundColor { get; set; }
    public FlairRichtext[]? LinkFlairRichtext { get; set; }
    public string? LinkFlairTemplateId { get; set; }
    public string? LinkFlairTextColor { get; set; }
    public string? LinkFlairType { get; set; }
    public long? NumCrossposts { get; set; }
    public string? ParentWhitelistStatus { get; set; }
    public bool? Pinned { get; set; }
    public string? RteMode { get; set; }
    public string? Selftext { get; set; }
    public string? SubredditType { get; set; }
    public string? WhitelistStatus { get; set; }
    public string? LinkFlairCssClass { get; set; }
    public bool? NoFollow { get; set; }
    public bool? SendReplies { get; set; }
    public long? SubredditSubscribers { get; set; }
}