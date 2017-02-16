using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.System;
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

        private IDisposable Subscribe<T>(IObservable<T> source, string subscribtionName)
        {
            return source
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
            this.Subscribe(source, "Cold Observable - #1");

            // Wait 3 seconds
            await Task.Delay(TimeSpan.FromSeconds(3));

            // Subscribe with the 2nd observer
            this.Subscribe(source, "Cold Observable - #2");
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
            this.Subscribe(source, "Hot Observable - #1");

            await Task.Delay(TimeSpan.FromSeconds(3));

            Console.WriteLine($"Subscribing with #2 observer at {DateTime.Now}");
            this.Subscribe(source, "Hot Observable - #2");
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

        public async void RefCount()
        {
            var hotInterval = Observable
                .Interval(TimeSpan.FromSeconds(1))
                .Publish()
                .RefCount();

            var subscription1 = this.Subscribe(hotInterval, "RefCount #1");

            await Task.Delay(TimeSpan.FromSeconds(3));

            var subscription2 = this.Subscribe(hotInterval, "RefCount #2");

            await Task.Delay(TimeSpan.FromSeconds(3));

            subscription1.Dispose();
            subscription2.Dispose();

            var subscription3 = this.Subscribe(hotInterval, "RefCount #3");
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
                .Range(0, 4)
                .SkipLast(2);

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
                .Range(0, 3)
                .TakeLast(2);

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
                .Range(0, 5)
                .Where(x => x > 10)
                .DefaultIfEmpty(42);

            this.Subscribe(source, "DefaultIfEmpty");
        }

        public void Materialize()
        {
            var source = Observable
                .Interval(TimeSpan.FromSeconds(1))
                .Materialize();

            this.Subscribe(source, "Materialize");
        }

        public void Timestamp()
        {
            var source = Observable
                .Interval(TimeSpan.FromSeconds(1))
                .Timestamp();

            this.Subscribe(source, "Timestamp");
        }

        public void TimeInterval()
        {
            var source = Observable
                .Interval(TimeSpan.FromSeconds(1))
                .TimeInterval();

            this.Subscribe(source, "TimeInterval");
        }

        public void Delay()
        {
            var source = Observable
                .Interval(TimeSpan.FromSeconds(1))
                .Delay(TimeSpan.FromSeconds(2));

            this.Subscribe(source, "Delay");
        }

        public void Throttle()
        {
            var source = Observable
                .FromEventPattern<KeyRoutedEventArgs>(this, nameof(this.KeyDown))
                .Select(e => e.EventArgs.Key)
                .Throttle(TimeSpan.FromSeconds(1));

            this.Subscribe(source, "Throttle");
        }

        public void Sample()
        {
            var source = Observable
                .FromEventPattern<PointerRoutedEventArgs>(this, nameof(this.PointerMoved))
                .Select(e => e.EventArgs.GetCurrentPoint(this).Position)
                .Sample(TimeSpan.FromMilliseconds(500));

            this.Subscribe(source, "Sample");
        }

        public void Timeout()
        {
            var source = Observable
                .Timer(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(1))
                .Timeout(TimeSpan.FromSeconds(3));

            this.Subscribe(source, "Timeout");
        }

        private async Task<int> RandomlyFailingService()
        {
            if (new Random().NextDouble() < 0.75)
                throw new Exception("Service Failed");

            return 42;
        }

        public void Retry()
        {
            var source = Observable
                .FromAsync(() => this.RandomlyFailingService())
                .Retry(3);

            this.Subscribe(source, "Retry");
        }

        public void OnErrorResumeNext()
        {
            var source1 = Observable.Create<int>(observer => () =>
            {
                observer.OnNext(0);
                observer.OnNext(1);
                observer.OnError(new Exception(""));
            });

            var source2 = Observable.Create<int>(observer => () =>
            {
                observer.OnNext(11);
                observer.OnNext(12);
                observer.OnNext(13);
            });

            var source = source1.OnErrorResumeNext(source2);

            this.Subscribe(source, "OnErrorResumeNext");
        }

        public void Catch()
        {
            var source1 = Observable.Throw<int>(new KeyNotFoundException("Problem"));
            var source2 = Observable.Return(42);
            var source3 = Observable.Return(0);

            var source = source1
                .Catch((KeyNotFoundException ex) => source2)
                .Catch((Exception ex) => source3);
        }

        public void Finally()
        {
            var source = Observable
                .Return(42)
                .Finally(() => { /* ... */ });
        }

        public void SelectMany()
        {
            var source = new[] { 'A', 'B', 'C', 'D' }
                .ToObservable()
                .SelectMany(letter => Observable
                    .Range(1, 5)
                    .Delay(number => Observable.Timer(TimeSpan.FromSeconds(number)))
                    .Select(number => $"{letter} -> {number}"));

            this.Subscribe(source, "SelectMany");
        }

        public void Merge()
        {
            var source = new[] { 'A', 'B', 'C', 'D' }
                .ToObservable()
                .Select(letter => Observable
                    .Range(1, 5)
                    .Delay(number => Observable.Timer(TimeSpan.FromSeconds(number)))
                    .Select(number => $"{letter} -> {number}"))
                .Merge();

            this.Subscribe(source, "Merge");
        }

        public void Concat()
        {
            var source = new[] { 'A', 'B', 'C', 'D' }
                .ToObservable()
                .Select(letter => Observable
                    .Range(1, 5)
                    .Delay(number => Observable.Timer(TimeSpan.FromSeconds(number)))
                    .Select(number => $"{letter} -> {number}"))
                .Concat();

            this.Subscribe(source, "Concat");
        }

        public void Zip()
        {
            var source1 = new Subject<int>();
            var source2 = new Subject<char>();

            var source = Observable.Zip(source1, source2, (i, c) => $"{i}:{c}");

            this.Subscribe(source, "Zip");

            source1.OnNext(0);
            source1.OnNext(1);
            source2.OnNext('A');
            source2.OnNext('B');
            source2.OnNext('C');
            source1.OnNext(2);
            source1.OnNext(3);
        }

        public void CombineLatest()
        {
            var source1 = new Subject<int>();
            var source2 = new Subject<char>();

            var source = Observable.CombineLatest(source1, source2, (i, c) => $"{i}:{c}");

            this.Subscribe(source, "CombineLatest");

            source1.OnNext(0);
            source1.OnNext(1);
            source2.OnNext('A');
            source1.OnNext(2);
            source1.OnNext(3);
            source2.OnNext('B');
        }

        public void Amb()
        {
            var source1 = Observable
                .Timer(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(1));

            var source2 = Observable
                .Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
                .Select(x => x * 10);

            var source = Observable.Amb(source1, source2);

            this.Subscribe(source, "Amb");
        }

        public void Switch()
        {
            var baseStream = Observable
                .Interval(TimeSpan.FromSeconds(5))
                .Select(i => Math.Pow(10, i));

            var subStreams = baseStream
                .Select(i => Observable
                    .Interval(TimeSpan.FromSeconds(1))
                    .Select(j => i * j));

            var source = subStreams.Switch();

            this.Subscribe(source, "Switch");
        }

        // Window
        // Buffer

        public void Scan()
        {
            var source = Observable
                .Range(0, 5)
                .Scan((accumulator, current) => accumulator += current);

            this.Subscribe(source, "Scan");
        }

        public void Aggregate()
        {
            var source = Observable
                .Range(0, 5)
                .Aggregate((accumulator, current) => accumulator += current);

            this.Subscribe(source, "Aggregate");
        }

        public void GroupBy()
        {
            var source = Observable
                .Interval(TimeSpan.FromSeconds(1))
                .GroupBy(x => x % 2 == 0);

            this.Subscribe(source, "GroupBy");
        }

        public void GroupByAdvanced()
        {
            var source = Observable
                .Interval(TimeSpan.FromSeconds(1))
                .GroupBy(x => x % 2 == 0)
                .SelectMany(x => x
                    .Scan((accumulator, current) => accumulator += current)
                    .Select(y => Tuple.Create(x.Key, y)));

            this.Subscribe(source, "GroupBy Advanced");
        }

        public void HistoricalScheduler()
        {
            // Arrange
            var baseTime = DateTimeOffset.Now;

            var scheduler = new HistoricalScheduler(baseTime);

            var expectedValues = new[]
            {
                Timestamped.Create(0L, baseTime + TimeSpan.FromSeconds(10)),
                Timestamped.Create(1L, baseTime + TimeSpan.FromSeconds(20)),
                Timestamped.Create(4L, baseTime + TimeSpan.FromSeconds(30)),
                Timestamped.Create(9L, baseTime + TimeSpan.FromSeconds(40)),
                Timestamped.Create(16L, baseTime + TimeSpan.FromSeconds(50)),
                Timestamped.Create(25L, baseTime + TimeSpan.FromSeconds(60))
            };

            var actualValues = new List<Timestamped<long>>();

            var source = Observable
                .Interval(TimeSpan.FromSeconds(10), scheduler)
                .Select(x => x * x)
                .Take(6);

            var testSource = source
                .Timestamp(scheduler)
                .Do(x => actualValues.Add(x));

            // Act
            testSource.Subscribe();
            scheduler.Start();

            // Assert
            if (expectedValues.SequenceEqual(actualValues, TestDataEqualityComparer.Instance))
                Console.WriteLine("The test was successful");
            else
                Console.WriteLine("The test failed");
        }

        private class TestDataEqualityComparer : IEqualityComparer<Timestamped<long>>
        {
            // Singleton object
            private TestDataEqualityComparer() { }
            private static TestDataEqualityComparer instance;
            public static TestDataEqualityComparer Instance => instance ?? (instance = new TestDataEqualityComparer());

            // Interface implementation
            public int GetHashCode(Timestamped<long> obj) => 0;
            public bool Equals(Timestamped<long> x, Timestamped<long> y)
                => x.Value == y.Value && AreDateTimeOffsetsClose(x.Timestamp, y.Timestamp, TimeSpan.FromMilliseconds(10));

            // Helper method to compare DateTimes
            private static bool AreDateTimeOffsetsClose(DateTimeOffset a, DateTimeOffset b, TimeSpan treshold)
                => Math.Abs((a - b).Ticks) <= treshold.Ticks;
        }

        public async void AwaitExample1()
        {
            var result = await Observable
                .FromAsync(async () => "Hello World")
                .Timeout(TimeSpan.FromSeconds(5))
                .Retry(3);
        }

        public async void AwaitExample2()
        {
            var result = await Observable
                .FromEventPattern<PointerRoutedEventArgs>(this, nameof(this.PointerMoved))
                .Select(e => e.EventArgs.GetCurrentPoint(this).Position)
                .Take(TimeSpan.FromSeconds(5))
                .ToList();
        }

        public async void AwaitExample3()
        {
            var enter = Observable
                .FromEventPattern<KeyRoutedEventArgs>(this, nameof(this.KeyDown))
                .Select(e => e.EventArgs.Key)
                .Where(k => k == VirtualKey.Enter)
                .FirstAsync();

            var esc = Observable
                .FromEventPattern<KeyRoutedEventArgs>(this, nameof(this.KeyDown))
                .Select(e => e.EventArgs.Key)
                .Where(k => k == VirtualKey.Escape)
                .FirstAsync();

            var dialogResult = await Observable.Amb(enter, esc);

            Console.WriteLine($"The user pressed the {dialogResult} key");
        }

        public async void AwaitExample4()
        {
            try
            {
                await Observable.Throw<int>(new Exception("Some problem happened in the stream"));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async void AwaitRxDelay()
        {
            var scheduler = CurrentThreadScheduler.Instance;
            await Observable.Timer(TimeSpan.FromSeconds(5), scheduler);
        }
    }
}