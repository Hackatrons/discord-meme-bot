using DiscordBot.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DiscordBot.Test;

[TestClass]
public class DomainBlacklistFilterTest
{
    [TestMethod]
    public void EnsureDirectDashLinksAreAllowed()
    {
        const string shouldAllow = "https://v.redd.it/123/DASH_720.mp4";
        const string shouldDisallow = "https://v.redd.it/123/";

        Assert.AreEqual(true, DomainBlacklistFilter.IsAllowed(shouldAllow));
        Assert.AreEqual(false, DomainBlacklistFilter.IsAllowed(shouldDisallow)); 
    }
}