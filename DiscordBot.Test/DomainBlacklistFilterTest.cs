using System.Linq;
using System.Threading.Tasks;
using DiscordBot.Filters;
using DiscordBot.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DiscordBot.Test;

[TestClass]
public class DomainBlacklistFilterTest
{
    [TestMethod]
    public async Task EnsureDirectDashLinksAreAllowed()
    {
        var filter = new DomainBlacklistFilter(new UnitTestLogger<DomainBlacklistFilter>());

        var shouldAllow = new SearchResult
        {
            Url = "https://v.redd.it/123/DASH_720.mp4"
        };
        var shouldDisallow = new SearchResult
        {
            Url = "https://v.redd.it/123/"
        };

        var allowed = await filter.Filter(new[] { shouldAllow }.ToAsyncEnumerable()).ToListAsync();
        Assert.AreEqual(1, allowed.Count);

        var disallowed = await filter.Filter(new[] { shouldDisallow }.ToAsyncEnumerable()).ToListAsync();
        Assert.AreEqual(0, disallowed.Count);
    }
}