using DiscordBot.Pushshift;
using DiscordBot.Pushshift.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace DiscordBot.Test;

[TestClass]
public class ResultExpanderTest
{
    [TestMethod]
    public void RegexCorrectlyExtractsUrls()
    {
        const string url1 = "https://asdf.com/lol.mp4?q=123";
        const string url2 = "https://asdf.com/lol.gif";
        const string url3 = "http://www.asdftube.com/video/abc123";

        var result = new PushshiftResult
        {
            Selftext = $@"
                Hi there check out this link {url1} and this one '{url2}' and finally this one:{url3}"
        };

        var urls = result.ExtractUrls().ToList();
        Assert.AreEqual(3, urls.Count);

        Assert.IsTrue(urls.Any(x => x.Url.Equals(url1)));
        Assert.IsTrue(urls.Any(x => x.Url.Equals(url2)));
        Assert.IsTrue(urls.Any(x => x.Url.Equals(url3)));
    }
}