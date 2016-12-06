using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SearchExample.TraditionalService
{
    public class RaceCondition
    {
        private readonly FakeSearchService searchService = new FakeSearchService();

        private Task<IEnumerable<string>> lastCall;
        public event Action<IEnumerable<string>> CallBack_RaceCondition;
        public async void ServiceCall_RaceCondition(string query)
        {
            var newCall = this.searchService.GetSuggestionsForQuery(query);
            this.lastCall = newCall;

            var result = await newCall;

            if (this.lastCall == newCall)
                this.CallBack_RaceCondition?.Invoke(result);
        }
    }
}
