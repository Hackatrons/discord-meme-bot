namespace DiscordBot.Services;

internal interface IInitialise : IDisposable
{
    void Initialise();
}