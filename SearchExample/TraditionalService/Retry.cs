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
            var errorHappened = false;
            var tries = 0;
            do
            {
                errorHappened = false;
                try
                {
                    return await this.searchService.GetSuggestionsForQuery(query);
                }
                catch
                {
                    tries++;
                    errorHappened = true;
                }
            } while (errorHappened == true && tries < this.numberOfRetries);

            throw new Exception("Out of retries");
        }
    }
}
