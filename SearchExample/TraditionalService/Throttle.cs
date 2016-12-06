using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SearchExample.TraditionalService
{
    public class Throttle
    {
        private readonly FakeSearchService searchService = new FakeSearchService();

        private readonly TimeSpan throttleInterval = TimeSpan.FromMilliseconds(500);
        private DateTime lastThrottledParameterDate;
        public event Action<IEnumerable<string>> CallBack_Throttle;
        public async void ServiceCall_Throttle(string query)
        {
            this.lastThrottledParameterDate = DateTime.Now;
            await Task.Delay(this.throttleInterval);

            if (DateTime.Now - this.lastThrottledParameterDate < this.throttleInterval) return;

            var suggestions = await this.searchService.GetSuggestionsForQuery(query);
            this.CallBack_Throttle?.Invoke(suggestions);
        }
    }
}
