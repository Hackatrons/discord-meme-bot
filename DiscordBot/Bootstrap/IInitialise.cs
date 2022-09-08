namespace DiscordBot.Bootstrap;

/// <summary>
/// Represents a class that requires initialising and disposing.
/// </summary>
internal interface IInitialise : IDisposable
{
    /// <summary>
    /// Initialises the instance.
    /// </summary>
    void Initialise();
}