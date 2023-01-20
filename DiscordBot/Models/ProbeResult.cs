using System.Net;

namespace DiscordBot.Models;

/// <summary>
/// Contains the response information from probing a search result url.
/// </summary>
public record ProbeResult
{
    /// <summary>
    /// When the URL is alive and returns a successful HTTP response.
    /// </summary>
    public bool IsAlive { get; init; }

    /// <summary>
    /// The HTTP response status code.
    /// </summary>
    public HttpStatusCode? HttpStatusCode { get; init; }

    /// <summary>
    /// A description of the error that occurred if the HTTP request failed.
    /// </summary>
    public string? Error { get; init; }

    /// <summary>
    /// The final redirect url if there were any redirects from the search result url.
    /// </summary>
    public string? RedirectedUrl { get; init; }

    /// <summary>
    /// The etag of the search result (if any).
    /// </summary>
    public string? Etag { get; init; }

    /// <summary>
    /// The HTTP response content type of the search result (if any).
    /// </summary>
    public string? ContentType { get; init; }
}