using System;
using System.Reactive.Linq;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace RxSamples
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        public void TextBlockUsageSample()
        {
            Console.WriteLine("Hello Rx");
        }

        public void SubscriptionSample()
        {
            var source = Observable.Empty<int>();

            source
                .ObserveOnDispatcher()
                .Subscribe(
                    next => Console.WriteLine($"OnNext: {next}"),
                    error => Console.WriteLine($"OnError: {error.Message}"),
                    () => Console.WriteLine("OnCompleted"));
        }
    }
}
