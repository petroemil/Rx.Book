using System.Windows;

namespace SearchExample
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //var traditional = new TraditionalApproach(this.SearchBox, this.SearchButton, this.ErrorLabel, this.Suggestions);
            //traditional.Initialise();
            
            var rx = new RxApproach(this.SearchBox, this.SearchButton, this.ErrorLabel, this.Suggestions);
            rx.Initialise();
        }
    }
}
