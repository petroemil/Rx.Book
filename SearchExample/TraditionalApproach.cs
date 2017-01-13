using SearchExample.TraditionalService;
using System;
using System.Collections.Generic;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SearchExample
{
    public class TraditionalApproach
    {
        private readonly TextBox searchBox;
        private readonly Button searchButton;
        private readonly TextBlock errorLabel;
        private readonly ListView suggestions;

        public TraditionalApproach(TextBox searchBox, Button searchButton, TextBlock errorLabel, ListView suggestions)
        {
            this.searchBox = searchBox;
            this.searchButton = searchButton;
            this.errorLabel = errorLabel;
            this.suggestions = suggestions;
        }

        private readonly FakeSearchService searchService = new FakeSearchService();

        public void Initialise_Naive()
        {
            this.searchBox.TextChanged += async (s, e) => 
            {
                var query = this.searchBox.Text;
                var suggestions = await this.searchService.GetSuggestionsForQuery(query);
                this.suggestions.ItemsSource = suggestions;
            };

            this.searchButton.Click += async (s, e) => 
            {
                var query = this.searchBox.Text;
                var results = await this.searchService.GetResultsForQuery(query);
                this.suggestions.ItemsSource = results;
            };
        }

        public void Initialise()
        {
            this.InitialiseSuggestions();
            this.InitialiseResults();
        }

        private void InitialiseSuggestions()
        {
            var suggestionsServiceHelper = new ServiceCallWrapper<string, IEnumerable<string>>(this.searchService.GetSuggestionsForQuery);

            // Subscribing to events to trigger service call
            this.searchBox.TextChanged += (s, e) => suggestionsServiceHelper.ServiceCall(this.searchBox.Text);

            // Registering callback methods
            suggestionsServiceHelper.CallBack += this.CallBack;
            suggestionsServiceHelper.ErrorCallBack += this.ErrorCallBack;
        }

        private void InitialiseResults()
        {
            var resultsServiceHelper = new ServiceCallWrapper<string, IEnumerable<string>>(this.searchService.GetResultsForQuery);

            // Subscribing to events to trigger service call
            this.searchButton.Click += (s, e) => resultsServiceHelper.ServiceCall(this.searchBox.Text);
            this.suggestions.ItemClick += (s, e) => resultsServiceHelper.ServiceCall(e.ClickedItem as string);
            this.searchBox.KeyDown += (s, e) =>
            {
                if (e.Key == VirtualKey.Enter)
                    resultsServiceHelper.ServiceCall(this.searchBox.Text);
            };

            // Registering callback methods
            resultsServiceHelper.CallBack += this.CallBack;
            resultsServiceHelper.ErrorCallBack += this.ErrorCallBack;
        }
        
        private void CallBack(IEnumerable<string> items)
        {
            this.errorLabel.Visibility = Visibility.Collapsed;
            this.suggestions.ItemsSource = items;
        }

        private void ErrorCallBack(Exception exception)
        {
            this.errorLabel.Visibility = Visibility.Visible;
            this.errorLabel.Text = exception.Message;
        }
    }
}
