using DiscordBot.Models;

namespace DiscordBot.Ranking
{
    internal static class ResultRank
    {
        /// <summary>
        /// Returns a score/rank of the search result based on how likely the result is to be "good".
        /// </summary>
        public static double Rank(this SearchResult result) =>
            PopularityScore(result) +
            RecencyScore(result);

        static double RecencyScore(SearchResult result)
        {
            // favour recent results
            var age = DateTime.UtcNow - result.CreatedUtc;

            return 1 - (1 / Math.Log10(age.TotalDays));
        }

        static double PopularityScore(SearchResult result)
        {
            // unfortunately pushshift currently doesn't refresh the score (number of upvotes) often, which is logged as a bug in their githubL
            // https://github.com/pushshift/api/issues/14
            // I've seen results over 30 days that still have a score of 1 from pushshift, but are >1000 in reddit
            // somehow though the number of comments are kept relatively up to date
            const int minScore = 1;

            return Math.Max(minScore, Math.Log10(result.NumberOfComments) + Math.Log10(result.Score));
        }
    }
}
