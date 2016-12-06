using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace SearchExample.RxService
{
    public class ToObservable
    {
        private readonly FakeSearchService searchService;

        private void ToObservable_Sample1()
        {
            var observableSuggestions = this.searchService
                .GetSuggestionsForQuery("X")
                .ToObservable();
        }

        private void ToObservable_Sample2()
        {
            var observableSuggestions = this.searchService
                .GetSuggestionsForQuery("X")
                .ToObservable()
                .Timeout(TimeSpan.FromMilliseconds(250))
                .Retry(3);
        }
    }
}
