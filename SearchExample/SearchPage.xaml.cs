using System.Reflection;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SearchExample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SearchPage : Page
    {
        public SearchPage()
        {
            this.InitializeComponent();

            //var traditional = new TraditionalApproach(this.SearchBox, this.SearchButton, this.ErrorLabel, this.Suggestions);
            //traditional.Initialise();

            var rx = new RxApproach(this.SearchBox, this.SearchButton, this.ErrorLabel, this.Suggestions);
            rx.Initialise();
        }
    }
}
