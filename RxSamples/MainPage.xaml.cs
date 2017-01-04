using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
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
            this.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void TextBlockUsageSample()
        {
            Console.WriteLine("Hello Rx");
        }

        private void Subscribe<T>(IObservable<T> source, string subscribtionName)
        {
            source
                .ObserveOnDispatcher()
                .Subscribe(
                    next => Console.WriteLine($"[{subscribtionName}] [OnNext]: {next}"),
                    error => Console.WriteLine($"[{subscribtionName}] [OnError]: {error.Message}"),
                    () => Console.WriteLine($"[{subscribtionName}] [OnCompleted]"));
        }

        private void SubscriptionSample()
        {
            var source = Observable.Empty<string>();

            this.Subscribe(source, "Sample");
        }

        private void Never()
        {
            var source = Observable.Never<string>();

            this.Subscribe(source, "Never");
        }

        private void Empty()
        {
            var source = Observable.Empty<string>();

            this.Subscribe(source, "Empty");
        }

        private void Return()
        {
            var source = Observable.Return("A");

            this.Subscribe(source, "Return");
        }

        private void Throw()
        {
            var source = Observable.Throw<string>(new Exception("X"));

            this.Subscribe(source, "Throw");
        }

        private void Range()
        {
            var source = Observable.Range(0, 10);

            this.Subscribe(source, "Range");
        }

        private void Generate()
        {
            var source = Observable.Generate(
                initialState: 0,
                condition: i => i < 5,
                iterate: i => i + 1,
                resultSelector: i => i * i);

            this.Subscribe(source, "Generate");
        }

        private void Interval()
        {
            var source = Observable.Interval(TimeSpan.FromMilliseconds(100));

            this.Subscribe(source, "Interval");
        }

        private void Timer_Relative()
        {
            var sourceRelative = Observable.Timer(TimeSpan.FromMilliseconds(500));

            this.Subscribe(sourceRelative, "Timer");
        }

        private void Timer_Absolute()
        {
            var sourceAbsolute = Observable.Timer(new DateTime(2063, 4, 4));

            this.Subscribe(sourceAbsolute, "Timer");
        }

        private void Timer_WithSubsequentElements()
        {
            var source = Observable.Timer(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(1));

            this.Subscribe(source, "Timer");
        }

        private void ToObservable()
        {
            var source = new[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J" }.ToObservable();

            this.Subscribe(source, "ToObservable");
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

            this.Subscribe(goodExample, "FromAsync");
        }

        private void FromAsync_BadExample()
        {
            var badExample = SomeAsyncOperation().ToObservable();

            this.Subscribe(badExample, "FromAsync");
        }

        private void FromEventPattern()
        {
            var source = Observable.FromEventPattern<KeyRoutedEventArgs>(this, nameof(this.KeyDown));

            this.Subscribe(source, "FromEventPattern");
        }

        public event Action<string, int, double> MySpecialEvent;
        private void FromEvent()
        {
            var source = Observable.FromEvent<Action<string, int, double>, Tuple<string, int, double>>(
                rxOnNext => (s, i, d) => rxOnNext(Tuple.Create(s, i, d)),
                eventHandler => MySpecialEvent += eventHandler,
                eventHandler => MySpecialEvent -= eventHandler);

            this.Subscribe(source, "FromEvent");
        }

        public event Action<string> MyOneParameterEvent;
        private void FromEvent_OneParameter()
        {
            var source = Observable.FromEvent<string>(
                eventHandler => MyOneParameterEvent += eventHandler,
                eventHandler => MyOneParameterEvent -= eventHandler);

            this.Subscribe(source, "FromEvent");
        }

        public event Action MyParameterlessEvent;
        private void FromEvent_Parameterless()
        {
            var source = Observable.FromEvent(
                eventHandler => MyParameterlessEvent += eventHandler,
                eventHandler => MyParameterlessEvent -= eventHandler);

            this.Subscribe(source, "FromEvent");
        }

        public async void Publish_DemonstrateTheColdObservable()
        {
            var source = Observable.Interval(TimeSpan.FromSeconds(1));

            // Subscribe with the 1st observer
            this.Subscribe(source, "Publish - #1");

            // Wait 3 seconds
            await Task.Delay(TimeSpan.FromSeconds(3));

            // Subscribe with the 2nd observer
            this.Subscribe(source, "Publish - #2");
        }

        public async void Publish_CallingConnectBeforeTheTwoSubscribtions()
        {
            var originalSource = Observable.Interval(TimeSpan.FromSeconds(1));
            var publishedSource = originalSource.Publish();

            // Call Connect to activate the source and subscribe immediately with the 1st observer
            publishedSource.Connect();
            this.Subscribe(publishedSource, "Publish - #1");

            // Wait 3 seconds
            await Task.Delay(TimeSpan.FromSeconds(3));

            // Subscribe with the 2nd observer
            this.Subscribe(publishedSource, "Publish - #2");
        }

        public async void Publish_CallingConnectAfterTheFirstSubscribtion()
        {
            var originalSource = Observable.Interval(TimeSpan.FromSeconds(1));
            var publishedSource = originalSource.Publish();

            // Subscribe to the not-yet-activated source stream with the 1st observer
            this.Subscribe(publishedSource, "Publish - #1");

            // Wait 3 seconds
            await Task.Delay(TimeSpan.FromSeconds(3));

            // Call Connect to activate the source and subscribe with the 2nd observer
            publishedSource.Connect();
            this.Subscribe(publishedSource, "Publish - #2");
        }

        public async void Replay_DemonstrateTheHotObservable()
        {
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Start();

            var source = Observable
                .FromEventPattern(timer, nameof(timer.Tick))
                .Select(e => DateTime.Now);

            Console.WriteLine($"Subscribing with #1 observer at {DateTime.Now}");
            this.Subscribe(source, "Replay - #1");

            await Task.Delay(TimeSpan.FromSeconds(3));

            Console.WriteLine($"Subscribing with #2 observer at {DateTime.Now}");
            this.Subscribe(source, "Replay - #2");
        }

        public async void Replay_CallingConnectBeforeTheTwoSubscribtions()
        {
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Start();

            var originalSource = Observable
                .FromEventPattern(timer, nameof(timer.Tick))
                .Select(e => DateTime.Now);

            var replayedSource = originalSource.Replay();

            Console.WriteLine($"Cold stream activated at {DateTime.Now}");
            replayedSource.Connect();

            Console.WriteLine($"Subscribing with #1 observer at {DateTime.Now}");
            this.Subscribe(replayedSource, "Replay - #1");

            await Task.Delay(TimeSpan.FromSeconds(3));

            Console.WriteLine($"Subscribing with #2 observer at {DateTime.Now}");
            this.Subscribe(replayedSource, "Replay - #2");
        }

        public async void Replay_CallingConnectAfterTheFirstSubscribtion()
        {
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Start();

            var originalSource = Observable
                .FromEventPattern(timer, nameof(timer.Tick))
                .Select(e => DateTime.Now);

            var replayedSource = originalSource.Replay();

            Console.WriteLine($"Subscribing with #1 observer at {DateTime.Now}");
            this.Subscribe(replayedSource, "Replay - #1");

            await Task.Delay(TimeSpan.FromSeconds(3));

            Console.WriteLine($"Cold stream activated at {DateTime.Now}");
            replayedSource.Connect();

            Console.WriteLine($"Subscribing with #2 observer at {DateTime.Now}");
            this.Subscribe(replayedSource, "Replay - #2");
        }

        public async void Subject_TurnsAnythingToHot()
        {
            // Create the source (cold) observable
            var interval = Observable.Interval(TimeSpan.FromSeconds(1));

            var subject = new Subject<long>();

            // Subscribe the subject to the source observable
            // With this you activate the source observable
            interval.Subscribe(subject);

            this.Subscribe(subject, "Subject #1");
            await Task.Delay(TimeSpan.FromSeconds(3));
            this.Subscribe(subject, "Subject #2");
        }

        public void Subject()
        {
            var subject = new Subject<string>();
            subject.OnNext("1");
            this.Subscribe(subject, "Subject");
            subject.OnNext("2");
            subject.OnNext("3");
            subject.OnCompleted();
            subject.OnNext("4");
        }

        public void ReplaySubject()
        {
            var replaySubject = new ReplaySubject<string>();
            replaySubject.OnNext("1");
            this.Subscribe(replaySubject, "ReplaySubject #1");
            replaySubject.OnNext("2");
            this.Subscribe(replaySubject, "ReplaySubject #2");
            replaySubject.OnNext("3");
        }

        public void BehaviorSubject()
        {
            var behaviorSubject = new BehaviorSubject<string>("0");
            this.Subscribe(behaviorSubject, "BehaviorSubject #1");
            behaviorSubject.OnNext("1");
            behaviorSubject.OnNext("2");
            this.Subscribe(behaviorSubject, "BehaviorSubject #2");
            behaviorSubject.OnNext("3");
            behaviorSubject.OnCompleted();
            this.Subscribe(behaviorSubject, "BehaviorSubject #3");
        }

        public void AsyncSubject()
        {
            var asyncSubject = new AsyncSubject<string>();
            asyncSubject.OnNext("1");
            this.Subscribe(asyncSubject, "AsyncSubject #1");
            asyncSubject.OnNext("2");
            asyncSubject.OnNext("3");
            asyncSubject.OnCompleted();
            this.Subscribe(asyncSubject, "AsyncSubject #2");
        }

        public void Select()
        {
            var source = Observable
                .FromEventPattern<KeyRoutedEventArgs>(this, nameof(this.KeyDown))
                .Select(e => e.EventArgs.Key.ToString());

            this.Subscribe(source, "Select");
        }

        public void Where()
        {
            var source = Observable
                .FromEventPattern<PointerRoutedEventArgs>(this, nameof(this.PointerMoved))
                .Select(e => e.EventArgs.GetCurrentPoint(this).Position)
                .Select(p => new Point((int)(p.X / 100), (int)(p.Y / 100)))
                .Where(p => p.X == p.Y)
                .Select(p => $"{p.X}, {p.Y}");

            this.Subscribe(source, "Where");
        }

        public void DistinctUntilChanged()
        {
            var source = Observable
                .FromEventPattern<PointerRoutedEventArgs>(this, nameof(this.PointerMoved))
                .Select(e => e.EventArgs.GetCurrentPoint(this).Position)
                .Select(p => new Point((int)(p.X / 100), (int)(p.Y / 100)))
                .DistinctUntilChanged()
                .Select(p => $"{p.X}, {p.Y}");

            this.Subscribe(source, "DistinctUntilChanged");
        }

        public void Skip()
        {
            var source = Observable
                .Interval(TimeSpan.FromSeconds(1))
                .Skip(5);

            this.Subscribe(source, "Skip");
        }

        public void SkipLast()
        {
            var source = Observable
                .Range(0, 5)
                .SkipLast(3);

            this.Subscribe(source, "SkipLast");
        }

        public void SkipUntil()
        {
            var source = Observable
                .Interval(TimeSpan.FromSeconds(1))
                .SkipUntil(DateTime.Now + TimeSpan.FromSeconds(5));

            this.Subscribe(source, "SkipUntil");
        }

        public void SkipWhile()
        {
            var source = Observable
                .Interval(TimeSpan.FromSeconds(1))
                .SkipWhile(num => num % 2 == 0);

            this.Subscribe(source, "SkipWhile");
        }

        public void Take()
        {
            var source = Observable
                .Interval(TimeSpan.FromSeconds(1))
                .Take(5);

            this.Subscribe(source, "Take");
        }

        public void TakeLast()
        {
            var source = Observable
                .Range(0, 5)
                .TakeLast(3);

            this.Subscribe(source, "TakeLast");
        }

        public void TakeUntil()
        {
            var source = Observable
                .Interval(TimeSpan.FromSeconds(1))
                .TakeUntil(DateTime.Now + TimeSpan.FromSeconds(5));

            this.Subscribe(source, "TakeUntil");
        }

        public void TakeWhile()
        {
            var source = Observable
                .Interval(TimeSpan.FromSeconds(1))
                .TakeWhile(num => num % 2 == 0);

            this.Subscribe(source, "TakeWhile");
        }

        public void SkipAndTake()
        {
            var source = Observable
                .Interval(TimeSpan.FromSeconds(1))
                .Skip(3)
                .Take(4);

            this.Subscribe(source, "Skip and Take");
        }

        public void First()
        {
            var source = Observable
                .Interval(TimeSpan.FromSeconds(1))
                .FirstAsync();

            this.Subscribe(source, "First");
        }

        public void Last()
        {
            var source = Observable
                .Range(0, 4)
                .LastAsync();

            this.Subscribe(source, "Last");
        }

        public void ElementAt()
        {
            var source = Observable
                .Interval(TimeSpan.FromSeconds(1))
                .ElementAt(2);

            this.Subscribe(source, "ElementAt");
        }

        public void Single()
        {
            var source = Observable
                .Return(42)
                .SingleAsync();

            this.Subscribe(source, "Single");
        }

        public void Max()
        {
            var source = Observable
                .Range(0, 5)
                .Max();

            this.Subscribe(source, "Max");
        }

        public void StartWith()
        {
            var source = Observable
                .Range(0, 5)
                .StartWith(42);

            this.Subscribe(source, "StartWith");
        }

        public void DefaultIfEmpty()
        {
            var source = Observable
                .Empty<int>()
                .DefaultIfEmpty(42);

            this.Subscribe(source, "DefaultIfEmpty");
        }
    }
}