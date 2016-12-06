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

            var words = SampleData.GetTop100EnglishWords().Select(x => x.ToLower());

            var wordsOfQuery = query.ToLower().Split(' ');
            var lastWordOfQuery = wordsOfQuery.Last();
            var suggestionsForLastWordOfQuery = words.Where(w => w.StartsWith(lastWordOfQuery));

            var headOfQuery = "";
            if (wordsOfQuery.Length > 1)
            {
                headOfQuery = String.Join(" ", wordsOfQuery.SkipLast(1));
            }
            
            return suggestionsForLastWordOfQuery.Select(s => $"{headOfQuery} {s}").Take(10);
        }
    }
}