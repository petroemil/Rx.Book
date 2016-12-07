using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SearchExample.TraditionalService
{
    public class Retry
    {
        private readonly FakeSearchService searchService = new FakeSearchService();

        private readonly int numberOfRetries = 3;
        public async Task<IEnumerable<string>> ServiceCall_Retry(string query)
        {
            for (var tries = 0; tries < this.numberOfRetries; tries++)
            {
                try { return await this.searchService.GetSuggestionsForQuery(query); }
                catch { /* deal with the exception */ }
            }

            throw new Exception("Out of retries");
        }
    }
}
