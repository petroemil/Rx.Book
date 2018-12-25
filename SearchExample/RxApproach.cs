using SearchExample.RxService;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Threading;

namespace SearchExample
{
    // A little helper extension until the Rx library get support for .NET Core 3.0 WPF
    public static class RxDispatcherHelper
    {
        public static SynchronizationContext SynchronizationContext { get; set; }
        public static IObservable<T> ObserveOnDispatcher<T>(this IObservable<T> source)
            => source.ObserveOn(SynchronizationContext);
    }

    public class RxApproach
    {
        private readonly TextBox searchBox;
        private readonly Button searchButton;
        private readonly TextBlock errorLabel;
        private readonly ListView suggestions;

        public RxApproach(TextBox searchBox, Button searchButton, TextBlock errorLabel, ListView suggestions)
        {
            this.searchBox = searchBox;
            this.searchButton = searchButton;
            this.errorLabel = errorLabel;
            this.suggestions = suggestions;
        }

        private readonly FakeSearchService searchService = new FakeSearchService();

        public void Initialise()
        {
            RxDispatcherHelper.SynchronizationContext = SynchronizationContext.Current;

            this.InitialiseSuggestions();
            this.InitialiseResults();
        }

        private void InitialiseSuggestions()
        {
            // Define source event (observable)
            var queryTextChanged = Observable
                .FromEventPattern(this.searchBox, nameof(this.searchBox.TextChanged))
                .Select(e => this.searchBox.Text);

            // Subscribe to the prepared event stream
            queryTextChanged
                .CallService(this.searchService.GetSuggestionsForQuery)
                .ObserveOnDispatcher()
                .Do(this.OnNext, this.OnError)
                .Retry()
                .Subscribe();
        }

        private void InitialiseResults()
        {
            // Define source events (observables)
            var searchButtonClicked = Observable
                .FromEventPattern(this.searchButton, nameof(this.searchButton.Click))
                .Select(_ => this.searchBox.Text);

            var suggestionSelected = Observable
                .FromEventPattern<SelectionChangedEventArgs>(this.suggestions, nameof(this.suggestions.SelectionChanged))
                .Select(e => e.EventArgs.AddedItems.OfType<string>().SingleOrDefault())
                .Where(s => s != null);

            var enterKeyPressed = Observable
                .FromEventPattern<KeyEventArgs>(this.searchBox, nameof(this.searchBox.KeyDown))
                .Where(e => e.EventArgs.Key == Key.Enter)
                .Select(_ => this.searchBox.Text);
            
            // Merge the source events into a single stream
            var mergedInput = Observable
                .Merge(searchButtonClicked, enterKeyPressed, suggestionSelected);

            // Subscribe to the prepared event stream
            mergedInput
                .CallService(this.searchService.GetResultsForQuery)
                .ObserveOnDispatcher()
                .Do(this.OnNext, this.OnError)
                .Retry()
                .Subscribe();
        }

        private void OnNext(IEnumerable<string> values)
        {
            this.errorLabel.Visibility = Visibility.Collapsed;
            this.suggestions.ItemsSource = values;
        }

        private void OnError(Exception exception)
        {
            this.errorLabel.Visibility = Visibility.Visible;
            this.errorLabel.Text = exception.Message;
        }
    }
}