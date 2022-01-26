using DiscordBot.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using DiscordBot.Ranking;

namespace DiscordBot.Test
{
    [TestClass]
    public class RankTest
    {
        [TestMethod]
        public void ResultsAreRanked()
        {
            var results = new SearchResult[]
            {
                // highest scoring result
                new()
                {
                    CreatedUtc = DateTime.UtcNow,
                    Score = 5000,
                    NumberOfComments = 200,
                    Url = "http://1"
                },
                // high score but old result
                new()
                {
                    Score = 5000,
                    NumberOfComments = 200,
                    CreatedUtc = DateTime.UtcNow.AddYears(-1),
                    Url = "http://2"
                },
                // low score old result
                new()
                {
                    Score = 100,
                    NumberOfComments = 20,
                    CreatedUtc = DateTime.UtcNow.AddMonths(-6),
                    Url = "http://3"
                },
                // low score but recent result
                new()
                {
                    Score = 1,
                    NumberOfComments = 1,
                    CreatedUtc = DateTime.UtcNow,
                    Url = "http://4"
                },
            };

            var ranked = results
                .Select(result => new
                {
                    result,
                    rank = result.Rank()
                })
                .OrderByDescending(x => x.rank)
                .ToList();

            for (var i = 0; i < ranked.Count; i++)
            {
                Assert.AreEqual(results[i], ranked[i].result);
            }
        }
    }
}
