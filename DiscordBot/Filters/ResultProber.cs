using DiscordBot.Language;
using DiscordBot.Models;

namespace DiscordBot.Filters;

/// <summary>
/// Probes search result URLs to determine if they return a successful HTTP response. 
/// </summary>
public class ResultProber
{
    readonly IHttpClientFactory _httpClientFactory;

    public ResultProber(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory.ThrowIfNull();
    }

    /// <summary>
    /// Performs a HTTP head request to the result URL to determine if the URL exists and returns a successful response.
    /// </summary>
    public async Task<ProbeResult> Probe(SearchResult result)
    {
        if (!Uri.TryCreate(result.Url, UriKind.Absolute, out var uri))
            return new ProbeResult
            {
                IsAlive = false
            };

        using var client = _httpClientFactory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Head, uri);

        try
        {
            using var response = await client.SendAsync(request);

            return new ProbeResult
            {
                IsAlive = response.IsSuccessStatusCode,
                // we may have followed some redirects in which case the end url is different to the original url
                // provide the redirected url as discord doesn't seem to follow redirects
                RedirectedUrl = response.RequestMessage?.RequestUri?.ToString(),
                // some services like reddit and imgur will give us an etag back
                // where the picture/content itself may have a different url, but the etag may be the identical (e.g. think file hash)
                Etag = response.Headers.ETag?.Tag,
                HttpStatusCode = response.StatusCode,
                ContentType = response.Content.Headers.ContentType?.MediaType
            };
        }
        catch (HttpRequestException ex)
        {
            return new ProbeResult
            {
                IsAlive = false,
                HttpStatusCode = ex.StatusCode,
                Error = ex.Message
            };
        }
    }
}