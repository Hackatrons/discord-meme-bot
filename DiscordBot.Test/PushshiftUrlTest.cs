using DiscordBot.Pushshift;
using DiscordBot.Pushshift.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DiscordBot.Test;

[TestClass]
public class PushshiftUrlTest
{
    [TestMethod]
    public void QueryReturnsCorrectUrl()
    {
        Assert.AreEqual("https://api.pushshift.io/reddit/search/submission/?q=asdf",
            new PushshiftQuery()
                .Search("asdf")
                .ToString());
    }

    [TestMethod]
    public void TitleQueryReturnsCorrectUrl()
    {
        Assert.AreEqual("https://api.pushshift.io/reddit/search/submission/?title=asdf",
            new PushshiftQuery()
                .SearchTitle("asdf")
                .ToString());
    }

    [TestMethod]
    public void QueryWithLimitReturnsCorrectUrl()
    {
        Assert.AreEqual("https://api.pushshift.io/reddit/search/submission/?q=asdf&size=100",
            new PushshiftQuery()
                .Search("asdf")
                .Limit(100)
                .ToString());
    }

    [TestMethod]
    public void QueryWithScoreSortReturnsCorrectUrl()
    {
        Assert.AreEqual("https://api.pushshift.io/reddit/search/submission/?q=asdf&sort_type=score",
            new PushshiftQuery()
                .Search("asdf")
                .Sort(SortType.Score)
                .ToString());

        Assert.AreEqual("https://api.pushshift.io/reddit/search/submission/?q=asdf&sort_type=score&sort=asc",
            new PushshiftQuery()
                .Search("asdf")
                .Sort(SortType.Score, SortDirection.Ascending)
                .ToString());

        Assert.AreEqual("https://api.pushshift.io/reddit/search/submission/?q=asdf&sort_type=score&sort=desc",
            new PushshiftQuery()
                .Search("asdf")
                .Sort(SortType.Score, SortDirection.Descending)
                .ToString());
    }

    [TestMethod]
    public void QueryWithCreatedSortReturnsCorrectUrl()
    {
        Assert.AreEqual("https://api.pushshift.io/reddit/search/submission/?q=asdf&sort_type=created_utc",
            new PushshiftQuery()
                .Search("asdf")
                .Sort(SortType.CreatedDate)
                .ToString());

        Assert.AreEqual("https://api.pushshift.io/reddit/search/submission/?q=asdf&sort_type=created_utc&sort=asc",
            new PushshiftQuery()
                .Search("asdf")
                .Sort(SortType.CreatedDate, SortDirection.Ascending)
                .ToString());

        Assert.AreEqual("https://api.pushshift.io/reddit/search/submission/?q=asdf&sort_type=created_utc&sort=desc",
            new PushshiftQuery()
                .Search("asdf")
                .Sort(SortType.CreatedDate, SortDirection.Descending)
                .ToString());
    }

    [TestMethod]
    public void QueryWithCommentsSortReturnsCorrectUrl()
    {
        Assert.AreEqual("https://api.pushshift.io/reddit/search/submission/?q=asdf&sort_type=num_comments",
            new PushshiftQuery()
                .Search("asdf")
                .Sort(SortType.NumberOfComments)
                .ToString());

        Assert.AreEqual("https://api.pushshift.io/reddit/search/submission/?q=asdf&sort_type=num_comments&sort=asc",
            new PushshiftQuery()
                .Search("asdf")
                .Sort(SortType.NumberOfComments, SortDirection.Ascending)
                .ToString());

        Assert.AreEqual("https://api.pushshift.io/reddit/search/submission/?q=asdf&sort_type=num_comments&sort=desc",
            new PushshiftQuery()
                .Search("asdf")
                .Sort(SortType.NumberOfComments, SortDirection.Descending)
                .ToString());
    }

    [TestMethod]
    public void QueryWithSubredditsReturnsCorrectUrl()
    {
        Assert.AreEqual("https://api.pushshift.io/reddit/search/submission/?q=asdf&subreddit=sr1,sr2&size=100",
            new PushshiftQuery()
                .Search("asdf")
                .Subreddits("sr1", "sr2")
                .Limit(100)
                .ToString());
    }

    [TestMethod]
    public void QueryWithOnlyNsfwReturnsCorrectUrl()
    {
        Assert.AreEqual("https://api.pushshift.io/reddit/search/submission/?q=asdf&subreddit=sr1,sr2&over_18=true&size=100",
            new PushshiftQuery()
                .Search("asdf")
                .Subreddits("sr1", "sr2")
                .Nsfw(true)
                .Limit(100)
                .ToString());
    }

    [TestMethod]
    public void QueryWithOnlySfwReturnsCorrectUrl()
    {
        Assert.AreEqual("https://api.pushshift.io/reddit/search/submission/?q=asdf&subreddit=sr&over_18=false&size=100",
            new PushshiftQuery()
                .Search("asdf")
                .Subreddits("sr")
                .Nsfw(false)
                .Limit(100)
                .ToString());
    }

    [TestMethod]
    public void QueryWithSortReturnsCorrectUrl()
    {
        Assert.AreEqual("https://api.pushshift.io/reddit/search/submission/?q=asdf&subreddit=sr&sort_type=score&size=100",
            new PushshiftQuery()
                .Search("asdf")
                .Subreddits("sr")
                .Sort(SortType.Score)
                .Limit(100)
                .ToString());
    }

    [TestMethod]
    public void NoQueryReturnsCorrectUrl()
    {
        Assert.AreEqual("https://api.pushshift.io/reddit/search/submission/?subreddit=sr&sort_type=score&size=100",
            new PushshiftQuery()
                .Subreddits("sr")
                .Sort(SortType.Score)
                .Limit(100)
                .ToString());
    }

    [TestMethod]
    public void ScoreGreaterThanReturnsCorrectUrl()
    {
        Assert.AreEqual("https://api.pushshift.io/reddit/search/submission/?subreddit=sr&score=>100&sort_type=score&size=100",
            new PushshiftQuery()
                .Subreddits("sr")
                .FilterScore(100, ScoreFilterType.GreaterThan)
                .Sort(SortType.Score)
                .Limit(100)
                .ToString());
    }

    [TestMethod]
    public void ScoreGreaterThanOrEqualToReturnsCorrectUrl()
    {
        Assert.AreEqual("https://api.pushshift.io/reddit/search/submission/?subreddit=sr&score=>%3D100&sort_type=score&size=100",
            new PushshiftQuery()
                .Subreddits("sr")
                .FilterScore(100, ScoreFilterType.GreaterThanOrEqualTo)
                .Sort(SortType.Score)
                .Limit(100)
                .ToString());
    }

    [TestMethod]
    public void ScoreLessThanReturnsCorrectUrl()
    {
        Assert.AreEqual("https://api.pushshift.io/reddit/search/submission/?subreddit=sr&score=<100&sort_type=score&size=100",
            new PushshiftQuery()
                .Subreddits("sr")
                .FilterScore(100, ScoreFilterType.LessThan)
                .Sort(SortType.Score)
                .Limit(100)
                .ToString());
    }

    [TestMethod]
    public void ScoreLessThanOrEqualToReturnsCorrectUrl()
    {
        Assert.AreEqual("https://api.pushshift.io/reddit/search/submission/?subreddit=sr&score=<%3D100&sort_type=score&size=100",
            new PushshiftQuery()
                .Subreddits("sr")
                .FilterScore(100, ScoreFilterType.LessThanOrEqualTo)
                .Sort(SortType.Score)
                .Limit(100)
                .ToString());
    }

    [TestMethod]
    public void QueryWithFieldsReturnsCorrectUrl()
    {
        Assert.AreEqual("https://api.pushshift.io/reddit/search/submission/?q=asdf&fields=url,post_hint,num_comments,created_utc,score",
            new PushshiftQuery()
                .Search("asdf")
                .Fields<PushshiftResult>()
                .ToString());
    }
}