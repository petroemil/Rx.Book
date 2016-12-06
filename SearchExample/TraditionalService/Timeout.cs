using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SearchExample.TraditionalService
{
    public class Timeout
    {
        private readonly FakeSearchService searchService = new FakeSearchService();

        private readonly TimeSpan timeoutInterval = TimeSpan.FromMilliseconds(500);
        public async Task<IEnumerable<string>> ServiceCall_Timeout(string query)
        {
            var newTask = this.searchService.GetSuggestionsForQuery(query);
            var timeoutTask = Task.Delay(this.timeoutInterval);

            var firstTaskToEnd = await Task.WhenAny(newTask, timeoutTask);

            if (newTask == firstTaskToEnd) return newTask.Result;
            else throw new Exception("Timeout");
        }
    }
}
