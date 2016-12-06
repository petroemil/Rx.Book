using System;
using System.Collections.Generic;

namespace SearchExample.TraditionalService
{
    public class Distinct
    {
        private readonly FakeSearchService searchService = new FakeSearchService();

        private string lastDistinctParameter;
        public event Action<IEnumerable<string>> CallBack_Distinct;
        public async void ServiceCall_Distinct(string query)
        {
            if (this.lastDistinctParameter != query)
                this.lastDistinctParameter = query;
            else
                return;

            var suggestions = await this.searchService.GetSuggestionsForQuery(query);

            this.CallBack_Distinct?.Invoke(suggestions);
        }
    }
}
