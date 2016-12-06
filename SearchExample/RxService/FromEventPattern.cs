using System;
using System.Linq;
using System.Reactive.Linq;
using Windows.UI.Xaml.Controls;

namespace SearchExample.RxService
{
    public class FromEventPattern
    {
        private readonly TextBox searchBox;
        private readonly Button searchButton;
        private readonly TextBlock errorLabel;
        private readonly ListView suggestions;

        private void FromEvent_Sample1()
        {
            var queryTextChanged = Observable.FromEventPattern(this.searchBox, nameof(this.searchBox.TextChanged));
        }

        private void FromEvent_Sample2()
        {
            var queryTextChanged = Observable
                .FromEventPattern(this.searchBox, nameof(this.searchBox.TextChanged))
                .Select(e => this.searchBox.Text);
        }

        private void FromEvent_Sample3()
        {
            var queryTextChanged = Observable
                .FromEventPattern(this.searchBox, nameof(this.searchBox.TextChanged))
                .Select(e => this.searchBox.Text)
                .Throttle(TimeSpan.FromMilliseconds(100))
                .DistinctUntilChanged();
        }
    }
}
