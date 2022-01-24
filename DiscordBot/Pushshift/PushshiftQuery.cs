using DiscordBot.Language;
using DiscordBot.Pushshift.Models;
using O9d.Json.Formatting;
using System.Collections.Specialized;
using System.Net;
using System.Text.Json;

namespace DiscordBot.Pushshift;

public class PushshiftQuery
{
    // https://github.com/pushshift/api
    const string BaseUrl = "https://api.pushshift.io/reddit/search/submission/";

    static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy()
    };

    readonly List<string> _subreddits = new();
    int? _limit;
    string? _query;
    string? _title;
    bool? _nsfw;
    SortType? _sort;
    int? _scoreFilter;
    ScoreFilterType? _scoreFilterType;
    SortDirection? _sortDirection;

    public PushshiftQuery Subreddits(IEnumerable<string> subreddits)
    {
        _subreddits.AddRange(subreddits.ThrowIfNull());
        return this;
    }

    public PushshiftQuery Subreddits(params string[] subreddits)
    {
        _subreddits.AddRange(subreddits.ThrowIfNull());
        return this;
    }

    public PushshiftQuery Limit(int limit)
    {
        if (limit < 1)
            throw new ArgumentOutOfRangeException(nameof(limit), limit, "limit cannot be less than 1");

        _limit = limit;
        return this;
    }

    public PushshiftQuery Search(string query)
    {
        _query = WebUtility.UrlEncode(query.ThrowIfNullOrWhitespace());
        return this;
    }

    public PushshiftQuery SearchTitle(string title)
    {
        _title = WebUtility.UrlEncode(title.ThrowIfNullOrWhitespace());
        return this;
    }

    public PushshiftQuery Nsfw(bool nsfw)
    {
        _nsfw = nsfw;
        return this;
    }

    public PushshiftQuery Sort(SortType sort, SortDirection? sortDirection = null)
    {
        _sort = sort;
        _sortDirection = sortDirection;
        return this;
    }

    public PushshiftQuery FilterScore(int score, ScoreFilterType type)
    {
        _scoreFilterType = type;
        _scoreFilter = score;

        return this;
    }

    public override string ToString()
    {
        return ToUri().ToString();
    }

    public Uri ToUri()
    {
        var builder = new UriBuilder(BaseUrl);
        var parameters = new NameValueCollection();

        if (!string.IsNullOrEmpty(_query)) parameters.Add("q", _query);
        if (!string.IsNullOrEmpty(_title)) parameters.Add("title", _title);
        if (_subreddits.Count > 0) parameters.Add("subreddit", string.Join(",", _subreddits));
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

            parameters.Add("sort_type", sortType);

            if (_sortDirection.HasValue)
            {
                parameters.Add("sort", _sortDirection == SortDirection.Ascending ? "asc" : "desc");
            }
        }

        if (_limit.HasValue) parameters.Add("size", _limit.Value.ToString());

        builder.Query = string.Join("&", parameters.AllKeys.Select(x => x + "=" + parameters[x]));

        // don't add port 443 into the return string
        builder.Port = -1;

        return builder.Uri;
    }

    public async Task<IEnumerable<Result>> Execute(HttpClient client, CancellationToken cancellationToken = new())
    {
        var url = ToString();
        var stream = await client.GetStreamAsync(url, cancellationToken);
        var json = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
        var items = json.RootElement.GetProperty("data").EnumerateArray();

        return items
            .Select(x => x.Deserialize<Result?>(SerializerOptions))
            .Where(x => x != null)
            .Select(x => x!);
    }
}