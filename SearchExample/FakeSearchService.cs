using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SearchExample
{
    public class FakeSearchService
    {
        private async Task SimulateTimeoutAndFail()
        {
            // Simulating long time execution         
            var random = new Random();
            await Task.Delay(random.Next(300));

            // Simulating failure         
            if (random.Next(100) < 10)
                throw new Exception("Error!");
        }

        public async Task<IEnumerable<string>> GetResultsForQuery(string query)
        {
            await this.SimulateTimeoutAndFail();

            return new string[] { $"This was your query: {query}" };
        }

        public async Task<IEnumerable<string>> GetSuggestionsForQuery(string query)
        {
            await this.SimulateTimeoutAndFail();

            // Split "The Quick Brown Fox" to [ "The", "Quick", "Brown" ] and "Fox"
            var queryWords = query.ToLower().Split(' ');
            var fixedPartOfQuery = queryWords.SkipLast(1);
            var lastWordOfQuery = queryWords.Last();

            // Get all the words from the 'SampleData' 
            // that starts with the last word fragment of the query
            var suggestionsForLastWordOfQuery = SampleData
                .GetTop100EnglishWords()
                .Select(w => w.ToLower())
                .Where(w => w.StartsWith(lastWordOfQuery));

            // Combine the "fixed" part of the query 
            // with the suggestions for the last word fragment
            var suggestionsToReturn = suggestionsForLastWordOfQuery
                .Select(s => fixedPartOfQuery.Concat(new[] { s }))
                .Select(s => string.Join(" ", s))
                .Take(10);

            return suggestionsToReturn;
        }
    }
}