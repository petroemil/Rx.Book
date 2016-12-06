using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TimelineGenerator
{
    public sealed partial class MainPage : Page
    {
        private readonly TimeLineDrawings timeLineDrawings;

        public MainPage()
        {
            this.InitializeComponent();
            this.timeLineDrawings = new TimeLineDrawings(this.Canvas, this.Border);
            this.Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            await this.timeLineDrawings.ExecuteMethods();
            App.Current.Exit();
        }        
    }
}