using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

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

        private void SubscriptionSample()
        {
            var source = Observable.Empty<int>();

            source
                .ObserveOnDispatcher()
                .Subscribe(
                    next => Console.WriteLine($"OnNext: {next}"),
                    error => Console.WriteLine($"OnError: {error.Message}"),
                    () => Console.WriteLine("OnCompleted"));
        }

        private void Never()
        {
            var source = Observable.Never<string>();
        }

        private void Empty()
        {
            var source = Observable.Empty<string>();
        }

        private void Return()
        {
            var source = Observable.Return("A");
        }

        private void Throw()
        {
            var source = Observable.Throw<string>(new Exception("X"));
        }

        private void Range()
        {
            var source = Observable.Range(0, 10);
        }

        private void Generate()
        {
            var source = Observable.Generate(
                initialState: 0,
                condition: i => i < 5,
                iterate: i => i + 1,
                resultSelector: i => i * i);
        }

        private void Interval()
        {
            var source = Observable.Interval(TimeSpan.FromMilliseconds(100));
        }

        private void Timer_Relative()
        {
            var sourceRelative = Observable.Timer(TimeSpan.FromMilliseconds(500));
        }

        private void Timer_Absolute()
        {
            var sourceAbsolute = Observable.Timer(new DateTime(2063, 4, 4));
        }

        private void Timer_WithSubsequentElements()
        {
            var source = Observable.Timer(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(1));
        }

        private void ToObservable()
        {
            var source = new[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J" }.ToObservable();
        }

        private async Task SomeAsyncOperation()
        {
            // Simulating random execution time    
            var random = new Random();
            await Task.Delay(random.Next(300));

            // Simulating failure         
            if (random.Next(100) < 10)
                throw new Exception("Error!");
        }

        private void FromAsync_GoodExample()
        {
            var goodExample = Observable.FromAsync(SomeAsyncOperation);
        }

        private void FromAsync_BadExample()
        {
            var badExample = SomeAsyncOperation().ToObservable();
        }
        
        private void FromEventPattern()
        {
            var source = Observable.FromEventPattern<KeyRoutedEventArgs>(this, nameof(this.KeyDown));
        }

        public event Action<string, int, double> MySpecialEvent;
        private void FromEvent()
        {
            var source = Observable.FromEvent<Action<string, int, double>, Tuple<string, int, double>>(
                rxOnNext => (s, i, d) => rxOnNext(Tuple.Create(s, i, d)),
                eventHandler => MySpecialEvent += eventHandler, 
                eventHandler => MySpecialEvent -= eventHandler);
        }

        public event Action<string> MyOneParameterEvent;
        private void FromEvent_OneParameter()
        {
            var source = Observable.FromEvent<string>(
                eventHandler => MyOneParameterEvent += eventHandler,
                eventHandler => MyOneParameterEvent -= eventHandler);
        }
    }
}