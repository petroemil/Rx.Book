using System.Windows;

namespace TimelineGenerator
{
    public partial class MainWindow : Window
    {
        private readonly TimeLineDrawings timeLineDrawings;

        public MainWindow()
        {
            InitializeComponent();
            this.timeLineDrawings = new TimeLineDrawings(this.Canvas, this.Border);
            this.Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            await this.timeLineDrawings.ExecuteMethods();
            Application.Current.Shutdown();
        }
    }
}
