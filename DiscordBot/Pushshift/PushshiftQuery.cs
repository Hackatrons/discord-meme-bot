using DiscordBot.Language;
using DiscordBot.Pushshift.Models;
using System.Collections.Specialized;
using System.Net;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DiscordBot.Pushshift;

/// <summary>
/// A builder for constructing pushshift REST API queries.
/// </summary>
public class PushshiftQuery
{
    // https://github.com/pushshift/api
    const string BaseUrl = "https://api.pushshift.io/reddit/search/submission";

    readonly List<string> _subreddits = new();
    readonly List<string> _fields = new();
    readonly List<string> _postHints = new();
    int? _limit;
    string? _query;
    string? _title;
    bool? _nsfw;
    SortType? _sort;
    int? _scoreFilter;
    ScoreFilterType? _scoreFilterType;
    SortDirection? _sortDirection;

    /// <summary>
    /// Filters based on one or more subreddits.
    /// </summary>
    public PushshiftQuery Subreddits(params string[] subreddits)
    {
        _subreddits.AddRange(subreddits.ThrowIfNull());
        return this;
    }

    /// <summary>
    /// Filters based on the post hint.
    /// </summary>
    public PushshiftQuery PostHints(params string[] postHints)
    {
        _postHints.AddRange(postHints.ThrowIfNull());
        return this;
    }

    /// <summary>
    /// Specifies which fields to return in the json payload based on the type.
    /// </summary>
    /// <remarks>
    /// Not really required, just reduces a bit of bandwidth and possibly provides a slight peformance improvement.
    /// </remarks>
    public PushshiftQuery Fields(IEnumerable<string> fields)
    {
        _fields.AddRange(fields.ThrowIfNull());

        return this;
    }

    /// <summary>
    /// Specifies which fields to return in the json payload based on the type.
    /// </summary>
    /// <remarks>
    /// Not really required, just reduces a bit of bandwidth and possibly provides a slight peformance improvement.
    /// Also only works for top level fields as it doesn't look like pushshift allows us to specify sub-fields e.g. "preview.enabled".
    /// By specifying "preview" however, all sub-fields are still returned from the API.
    /// </remarks>
    public PushshiftQuery Fields<T>()
    {
        var fields = typeof(T)
            .GetProperties()
            .Select(property => new
            {
                property,
                jsonAttribute = property.GetCustomAttributes<JsonPropertyNameAttribute>().SingleOrDefault()
            })
            .Select(x => x.jsonAttribute?.Name ?? x.property.Name);

        return Fields(fields);
    }

    /// <summary>
    /// Limits the number of search results returned in the response.
    /// </summary>
    public PushshiftQuery Limit(int limit)
    {
        if (limit < 1)
            throw new ArgumentOutOfRangeException(nameof(limit), limit, "limit cannot be less than 1");

        _limit = limit;
        return this;
    }

    /// <summary>
    /// Searches based on all available searchable fields.
    /// </summary>
    public PushshiftQuery Search(string query)
    {
        _query = WebUtility.UrlEncode(query.ThrowIfNullOrWhitespace());
        return this;
    }

    /// <summary>
    /// Searches based on the post title.
    /// </summary>
    public PushshiftQuery SearchTitle(string title)
    {
        _title = WebUtility.UrlEncode(title.ThrowIfNullOrWhitespace());
        return this;
    }

    /// <summary>
    /// Filters the result set to only NSFW results.
    /// </summary>
    public PushshiftQuery Nsfw()
    {
        _nsfw = true;
        return this;
    }

    /// <summary>
    /// Filters the result set to only SFW results.
    /// </summary>
    public PushshiftQuery Sfw()
    {
        _nsfw = false;
        return this;
    }

    /// <summary>
    /// Sorts the result set.
    /// </summary>
    public PushshiftQuery Sort(SortType sort, SortDirection? sortDirection = null)
    {
        _sort = sort;
        _sortDirection = sortDirection;
        return this;
    }

    /// <summary>
    /// Filters the result set based on the score.
    /// </summary>
    public PushshiftQuery FilterScore(int score, ScoreFilterType type)
    {
        _scoreFilterType = type;
        _scoreFilter = score;

        return this;
    }

    /// <summary>
    /// Returns the constructed URL.
    /// </summary>
    public override string ToString()
    {
        return ToUri().ToString();
    }

    /// <summary>
    /// Returns the constructed URL.
    /// </summary>
    public Uri ToUri()
    {
        var builder = new UriBuilder(BaseUrl);
        var parameters = new NameValueCollection();

        if (!string.IsNullOrEmpty(_query)) parameters.Add("q", _query);
        if (!string.IsNullOrEmpty(_title)) parameters.Add("title", _title);
        if (_subreddits.Any()) parameters.Add("subreddit", string.Join(",", _subreddits));
        if (_postHints.Any()) parameters.Add("post_hint", string.Join("|", _postHints));
        if (_nsfw.HasValue) parameters.Add("over_18", _nsfw.Value.ToString().ToLowerInvariant());

        if (_scoreFilterType.HasValue)
        {
            var score = _scoreFilterType.Value switch
            {
                ScoreFilterType.GreaterThan => WebUtility.UrlEncode(">") + _scoreFilter,
                ScoreFilterType.GreaterThanOrEqualTo => WebUtility.UrlEncode(">=") + _scoreFilter,
                ScoreFilterType.LessThan => WebUtility.UrlEncode("<") + _scoreFilter,
                ScoreFilterType.LessThanOrEqualTo => WebUtility.UrlEncode("<=") + _scoreFilter,
                _ => throw new InvalidOperationException("invalid score filter type")
            };

            parameters.Add("score", score);
        }

        if (_sort.HasValue)
        {
            var sortType = _sort.Value switch
            {
                SortType.Score => "score",
                SortType.NumberOfComments => "num_comments",
                SortType.CreatedDate => "created_utc",
                _ => throw new InvalidOperationException("invalid sort type specified")
            };

            parameters.Add("sort", sortType);

            if (_sortDirection.HasValue)
            {
                parameters.Add("order", _sortDirection == SortDirection.Ascending ? "asc" : "desc");
            }
        }

        if (_limit.HasValue) parameters.Add("size", _limit.Value.ToString());

        if (_fields.Any()) parameters.Add("filter", string.Join(",", _fields));

        builder.Query = string.Join("&", parameters.AllKeys.Select(x => x + "=" + parameters[x]));

        // don't add port 443 into the return string
        builder.Port = -1;

        return builder.Uri;
    }

    /// <summary>
    /// Invokes the Pushshift REST API and returns the results.
    /// </summary>
    public async Task<IEnumerable<PushshiftResult>> Execute(HttpClient client, CancellationToken cancellationToken = new())
    {
        var url = ToString();

        await using var stream = await client.ThrowIfNull().GetStreamAsync(url, cancellationToken);
        using var json = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
        using var items = json.RootElement.GetProperty("data").EnumerateArray();

        return items
            .Select(x => x.Deserialize<PushshiftResult?>())
            .Where(x => x != null && !string.IsNullOrEmpty(x.Url))
            .Select(x => x!)
            // evaluate the enumerable to avoid accessing a disposed json document
            // as the json document gets disposed after this method ends
            .ToList();
    }
}