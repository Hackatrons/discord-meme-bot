using DiscordBot.Language;
using DiscordBot.Models;

namespace DiscordBot.Filters;

/// <summary>
/// Probes search result URLs and filters out those that don't return a successful HTTP response. 
/// </summary>
public class UrlCheckFilter : IResultFilter
{
    readonly IHttpClientFactory _httpClientFactory;

    // inject a factory instead of a HttpClient as the lifespan of a HttpClient in this class is short and sporadic
    // and this class is a singleton
    // so we don't want to hold a reference to a lingering HttpClient forever
    public UrlCheckFilter(IHttpClientFactory httpClientFactory) => _httpClientFactory = httpClientFactory.ThrowIfNull();

    public IAsyncEnumerable<SearchResult> Filter(IAsyncEnumerable<SearchResult> input) => input
        .ThrowIfNull()
        .SelectAwait(async result => new
        {
            result,
            urlCheck = await TestUrl(result.Url)
        })
        .Where(x => x.urlCheck.Success)
        // some services like reddit and imgur will give us an etag back
        // where the picture/content itself may have a different url, but the etag may be the identical (e.g. think file hash)
        // so we can filter out duplicates via the etag
        .Distinct(x => x.urlCheck.Etag ?? x.result.Url)
        // if we got a redirect url, use it instead
        .Select(x => x.result with { Url = x.urlCheck.RedirectedToUrl ?? x.result.Url });

    async Task<(bool Success, string? Etag, string? RedirectedToUrl)> TestUrl(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            return (false, null, null);

        using var client = _httpClientFactory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Head, uri);
        using var response = await client.SendAsync(request);

        // we may have followed some redirects in which case the end url is different to the original url
        // provide the redirected url as discord doesn't seem to follow redirects
        var responseUrl = response.RequestMessage?.RequestUri?.ToString();

        return (response.IsSuccessStatusCode, response.Headers.ETag?.Tag, responseUrl);
    }
}