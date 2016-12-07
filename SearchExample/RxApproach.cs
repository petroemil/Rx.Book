using SearchExample.RxService;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace SearchExample
{
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
                .FromEventPattern<ItemClickEventArgs>(this.suggestions, nameof(this.suggestions.ItemClick))
                .Select(e => e.EventArgs.ClickedItem as string);

            var enterKeyPressed = Observable
                .FromEventPattern<KeyRoutedEventArgs>(this.searchBox, nameof(this.searchBox.KeyDown))
                .Where(e => e.EventArgs.Key == VirtualKey.Enter)
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