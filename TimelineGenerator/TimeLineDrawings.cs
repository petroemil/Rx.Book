using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace TimelineGenerator
{
    public class TimeLineDrawings
    {
        private readonly TimeLineRenderer timelineRenderer;
        private readonly TimeLineDrawer timelineDrawer;

        public TimeLineDrawings(Canvas canvas, Border border)
        {
            this.timelineDrawer = new TimeLineDrawer(canvas, border);
            this.timelineRenderer = new TimeLineRenderer(border);
        }

        public IEnumerable<MethodInfo> ListMethods()
        {
            return this.GetType()
                .GetRuntimeMethods()
                .Where(method => method.ReturnType == typeof(Task) && method.Name != nameof(this.ExecuteMethods));
        }

        public async Task ExecuteMethods()
        {
            foreach (var method in this.ListMethods())
            {
                this.timelineDrawer.ClearCanvas();
                await (Task)method.Invoke(this, null);
                this.timelineDrawer.SetCanvasSize();
                await this.timelineRenderer.SaveBMP(method.Name);
            }
        }

        public async Task Example()
        {
            this.timelineDrawer.DrawTimeLine(1);
            this.timelineDrawer.AddEventToTimeLine(0, 100, "A");
            this.timelineDrawer.AddEventToTimeLine(0, 200, "B");
            this.timelineDrawer.AddEventToTimeLine(0, 400, "C");
            this.timelineDrawer.AddEventToTimeLine(0, 700, "D");
            this.timelineDrawer.AddEventToTimeLine(0, 900, "E");
            this.timelineDrawer.AddCompletitionEventToTimeLine(0, 1000, false);
        }

        public async Task Never()
        {
            this.timelineDrawer.DrawTimeLine(1);
        }

        public async Task Empty()
        {
            this.timelineDrawer.AddCompletitionEventToTimeLine(0, 0, false);
        }

        public async Task Return()
        {
            this.timelineDrawer.DrawTimeLine(0, 0, 100);
            this.timelineDrawer.AddEventToTimeLine(0, 0, "A");
            this.timelineDrawer.AddCompletitionEventToTimeLine(0, 100, false);
        }

        public async Task Throw()
        {
            this.timelineDrawer.AddCompletitionEventToTimeLine(0, 0, true);
        }

        public async Task Range()
        {
            this.timelineDrawer.DrawTimeLine(1);
            this.timelineDrawer.AddEventToTimeLine(0, 0, "0");
            this.timelineDrawer.AddEventToTimeLine(0, 100, "1");
            this.timelineDrawer.AddEventToTimeLine(0, 200, "2");
            this.timelineDrawer.AddEventToTimeLine(0, 300, "3");
            this.timelineDrawer.AddEventToTimeLine(0, 400, "4");
            this.timelineDrawer.AddEventToTimeLine(0, 500, "5");
            this.timelineDrawer.AddEventToTimeLine(0, 600, "6");
            this.timelineDrawer.AddEventToTimeLine(0, 700, "7");
            this.timelineDrawer.AddEventToTimeLine(0, 800, "8");
            this.timelineDrawer.AddEventToTimeLine(0, 900, "9");
            this.timelineDrawer.AddCompletitionEventToTimeLine(0, 1000, false);
        }

        public async Task Generate()
        {
            this.timelineDrawer.DrawTimeLine(1);
            this.timelineDrawer.AddEventToTimeLine(0, 0, "0");
            this.timelineDrawer.AddEventToTimeLine(0, 100, "1");
            this.timelineDrawer.AddEventToTimeLine(0, 200, "4");
            this.timelineDrawer.AddEventToTimeLine(0, 300, "9");
            this.timelineDrawer.AddEventToTimeLine(0, 400, "16");
            this.timelineDrawer.AddEventToTimeLine(0, 500, "25");
            this.timelineDrawer.AddEventToTimeLine(0, 600, "36");
            this.timelineDrawer.AddEventToTimeLine(0, 700, "49");
            this.timelineDrawer.AddEventToTimeLine(0, 800, "64");
            this.timelineDrawer.AddEventToTimeLine(0, 900, "81");
            this.timelineDrawer.AddCompletitionEventToTimeLine(0, 1000, false);
        }

        public async Task ToObservable()
        {
            this.timelineDrawer.DrawTimeLine(1);
            this.timelineDrawer.AddEventToTimeLine(0, 0, "A");
            this.timelineDrawer.AddEventToTimeLine(0, 100, "B");
            this.timelineDrawer.AddEventToTimeLine(0, 200, "C");
            this.timelineDrawer.AddEventToTimeLine(0, 300, "D");
            this.timelineDrawer.AddEventToTimeLine(0, 400, "E");
            this.timelineDrawer.AddEventToTimeLine(0, 500, "F");
            this.timelineDrawer.AddEventToTimeLine(0, 600, "G");
            this.timelineDrawer.AddEventToTimeLine(0, 700, "H");
            this.timelineDrawer.AddEventToTimeLine(0, 800, "I");
            this.timelineDrawer.AddEventToTimeLine(0, 900, "J");
            this.timelineDrawer.AddCompletitionEventToTimeLine(0, 1000, false);
        }

        public async Task Interval()
        {
            this.timelineDrawer.DrawTimeLine(1);
            this.timelineDrawer.AddEventToTimeLine(0, 100, "0");
            this.timelineDrawer.AddEventToTimeLine(0, 300, "1");
            this.timelineDrawer.AddEventToTimeLine(0, 500, "2");
            this.timelineDrawer.AddEventToTimeLine(0, 700, "3");
            this.timelineDrawer.AddEventToTimeLine(0, 900, "4");
        }

        public async Task Timer()
        {
            this.timelineDrawer.DrawTimeLine(1);
            this.timelineDrawer.AddEventToTimeLine(0, 900, "0");
            this.timelineDrawer.AddCompletitionEventToTimeLine(0, 1000, false);
        }

        public async Task TimerWithDelay()
        {
            this.timelineDrawer.DrawTimeLine(1);
            this.timelineDrawer.AddEventToTimeLine(0, 500, "0");
            this.timelineDrawer.AddEventToTimeLine(0, 700, "1");
            this.timelineDrawer.AddEventToTimeLine(0, 900, "2");
        }

        public async Task FromEvent()
        {
            this.timelineDrawer.DrawTimeLine(1);
            this.timelineDrawer.AddEventToTimeLine(0, 100, "H");
            this.timelineDrawer.AddEventToTimeLine(0, 200, "E");
            this.timelineDrawer.AddEventToTimeLine(0, 400, "L");
            this.timelineDrawer.AddEventToTimeLine(0, 700, "L");
            this.timelineDrawer.AddEventToTimeLine(0, 900, "O");
        }

        public async Task FromAsync()
        {
            this.timelineDrawer.DrawTimeLine(0, 0, 200);
            this.timelineDrawer.AddEventToTimeLine(0, 100, "A");
            this.timelineDrawer.AddCompletitionEventToTimeLine(0, 200, false);
        }

        public async Task Amb()
        {
            this.timelineDrawer.DrawTimeLine(3);

            this.timelineDrawer.AddEventToTimeLine(0, 200, "0");
            this.timelineDrawer.AddEventToTimeLine(0, 300, "1");
            this.timelineDrawer.AddEventToTimeLine(0, 400, "2");
            this.timelineDrawer.AddEventToTimeLine(0, 500, "3");
            this.timelineDrawer.AddEventToTimeLine(0, 600, "4");

            this.timelineDrawer.AddEventToTimeLine(1, 100, "10");
            this.timelineDrawer.AddEventToTimeLine(1, 200, "20");
            this.timelineDrawer.AddEventToTimeLine(1, 300, "30");
            this.timelineDrawer.AddEventToTimeLine(1, 400, "40");
            this.timelineDrawer.AddEventToTimeLine(1, 500, "50");

            this.timelineDrawer.AddEventToTimeLine(2, 100, "10");
            this.timelineDrawer.AddEventToTimeLine(2, 200, "20");
            this.timelineDrawer.AddEventToTimeLine(2, 300, "30");
            this.timelineDrawer.AddEventToTimeLine(2, 400, "40");
            this.timelineDrawer.AddEventToTimeLine(2, 500, "50");

            this.timelineDrawer.ConnectEventsOnDifferentTimelines(1, 100, 2, 100);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(1, 200, 2, 200);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(1, 300, 2, 300);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(1, 400, 2, 400);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(1, 500, 2, 500);
        }

        public async Task Switch()
        {
            this.timelineDrawer.DrawTimeLine(0, 0, 1000);
            this.timelineDrawer.DrawTimeLine(1, 250, 1000);
            this.timelineDrawer.DrawTimeLine(2, 450, 1000);
            this.timelineDrawer.DrawTimeLine(3, 0, 1000);

            this.timelineDrawer.AddEventToTimeLine(0, 100, "0");
            this.timelineDrawer.AddEventToTimeLine(0, 200, "1");
            this.timelineDrawer.AddEventToTimeLine(0, 300, "2");
            this.timelineDrawer.AddEventToTimeLine(0, 400, "3");
            this.timelineDrawer.AddEventToTimeLine(0, 500, "4");

            this.timelineDrawer.AddEventToTimeLine(1, 300, "10");
            this.timelineDrawer.AddEventToTimeLine(1, 400, "20");
            this.timelineDrawer.AddEventToTimeLine(1, 500, "30");
            this.timelineDrawer.AddEventToTimeLine(1, 600, "40");
            this.timelineDrawer.AddEventToTimeLine(1, 700, "50");

            this.timelineDrawer.AddEventToTimeLine(2, 500, "100");
            this.timelineDrawer.AddEventToTimeLine(2, 600, "200");
            this.timelineDrawer.AddEventToTimeLine(2, 700, "300");
            this.timelineDrawer.AddEventToTimeLine(2, 800, "400");
            this.timelineDrawer.AddEventToTimeLine(2, 900, "500");

            this.timelineDrawer.AddEventToTimeLine(3, 100, "0");
            this.timelineDrawer.AddEventToTimeLine(3, 200, "1");
            this.timelineDrawer.AddEventToTimeLine(3, 300, "10");
            this.timelineDrawer.AddEventToTimeLine(3, 400, "20");
            this.timelineDrawer.AddEventToTimeLine(3, 500, "100");
            this.timelineDrawer.AddEventToTimeLine(3, 600, "200");
            this.timelineDrawer.AddEventToTimeLine(3, 700, "300");
            this.timelineDrawer.AddEventToTimeLine(3, 800, "400");
            this.timelineDrawer.AddEventToTimeLine(3, 900, "500");

            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 100, 3, 100);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 200, 3, 200);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(1, 300, 3, 300);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(1, 400, 3, 400);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(2, 500, 3, 500);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(2, 600, 3, 600);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(2, 700, 3, 700);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(2, 800, 3, 800);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(2, 900, 3, 900);
        }

        public async Task Where()
        {
            this.timelineDrawer.DrawTimeLine(2);

            this.timelineDrawer.AddEventToTimeLine(0, 100, "1,1");
            this.timelineDrawer.AddEventToTimeLine(0, 300, "1,2");
            this.timelineDrawer.AddEventToTimeLine(0, 500, "1,3");
            this.timelineDrawer.AddEventToTimeLine(0, 700, "2,3");
            this.timelineDrawer.AddEventToTimeLine(0, 900, "3,3");

            this.timelineDrawer.AddEventToTimeLine(1, 100, "1,1");
            this.timelineDrawer.AddEventToTimeLine(1, 900, "3,3");

            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 100, 1, 100);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 900, 1, 900);
        }

        public async Task DistinctUntilChanged()
        {
            this.timelineDrawer.DrawTimeLine(2);

            this.timelineDrawer.AddEventToTimeLine(0, 100, "1,1");
            this.timelineDrawer.AddEventToTimeLine(0, 200, "1,1");
            this.timelineDrawer.AddEventToTimeLine(0, 300, "1,1");
            this.timelineDrawer.AddEventToTimeLine(0, 400, "1,2");
            this.timelineDrawer.AddEventToTimeLine(0, 500, "1,2");
            this.timelineDrawer.AddEventToTimeLine(0, 600, "2,2");
            this.timelineDrawer.AddEventToTimeLine(0, 700, "2,3");
            this.timelineDrawer.AddEventToTimeLine(0, 800, "2,2");
            this.timelineDrawer.AddEventToTimeLine(0, 900, "2,2");

            this.timelineDrawer.AddEventToTimeLine(1, 100, "1,1");
            this.timelineDrawer.AddEventToTimeLine(1, 400, "1,2");
            this.timelineDrawer.AddEventToTimeLine(1, 600, "2,2");
            this.timelineDrawer.AddEventToTimeLine(1, 700, "2,3");
            this.timelineDrawer.AddEventToTimeLine(1, 800, "2,2");

            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 100, 1, 100);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 400, 1, 400);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 600, 1, 600);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 700, 1, 700);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 800, 1, 800);
        }

        public async Task Skip()
        {
            this.timelineDrawer.DrawTimeLine(2);

            this.timelineDrawer.AddEventToTimeLine(0, 100, "0");
            this.timelineDrawer.AddEventToTimeLine(0, 200, "1");
            this.timelineDrawer.AddEventToTimeLine(0, 300, "2");
            this.timelineDrawer.AddEventToTimeLine(0, 400, "3");
            this.timelineDrawer.AddEventToTimeLine(0, 500, "4");
            this.timelineDrawer.AddEventToTimeLine(0, 600, "5");
            this.timelineDrawer.AddEventToTimeLine(0, 700, "6");
            this.timelineDrawer.AddEventToTimeLine(0, 800, "7");
            this.timelineDrawer.AddEventToTimeLine(0, 900, "8");

            this.timelineDrawer.AddEventToTimeLine(1, 600, "5");
            this.timelineDrawer.AddEventToTimeLine(1, 700, "6");
            this.timelineDrawer.AddEventToTimeLine(1, 800, "7");
            this.timelineDrawer.AddEventToTimeLine(1, 900, "8");

            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 600, 1, 600);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 700, 1, 700);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 800, 1, 800);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 900, 1, 900);
        }

        public async Task SkipLast()
        {
            this.timelineDrawer.DrawTimeLine(0, 0, 800);
            this.timelineDrawer.DrawTimeLine(1, 0, 1000);

            this.timelineDrawer.AddEventToTimeLine(0, 100, "0");
            this.timelineDrawer.AddEventToTimeLine(0, 300, "1");
            this.timelineDrawer.AddEventToTimeLine(0, 500, "2");
            this.timelineDrawer.AddEventToTimeLine(0, 700, "3");
            this.timelineDrawer.AddCompletitionEventToTimeLine(0, 800, false);

            this.timelineDrawer.AddEventToTimeLine(1, 800, "0");
            this.timelineDrawer.AddEventToTimeLine(1, 900, "1");
            this.timelineDrawer.AddCompletitionEventToTimeLine(1, 1000, false);

            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 800, 1, 800);
        }

        public async Task SkipUntil()
        {
            this.timelineDrawer.DrawTimeLine(2);

            this.timelineDrawer.AddEventToTimeLine(0, 100, "0");
            this.timelineDrawer.AddEventToTimeLine(0, 200, "1");
            this.timelineDrawer.AddEventToTimeLine(0, 300, "2");
            this.timelineDrawer.AddEventToTimeLine(0, 400, "3");
            this.timelineDrawer.AddEventToTimeLine(0, 500, "4");
            this.timelineDrawer.AddEventToTimeLine(0, 600, "5");
            this.timelineDrawer.AddEventToTimeLine(0, 700, "6");
            this.timelineDrawer.AddEventToTimeLine(0, 800, "7");
            this.timelineDrawer.AddEventToTimeLine(0, 900, "8");

            this.timelineDrawer.AddEventToTimeLine(1, 600, "5");
            this.timelineDrawer.AddEventToTimeLine(1, 700, "6");
            this.timelineDrawer.AddEventToTimeLine(1, 800, "7");
            this.timelineDrawer.AddEventToTimeLine(1, 900, "8");

            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 600, 1, 600);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 700, 1, 700);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 800, 1, 800);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 900, 1, 900);
        }

        public async Task SkipWhile()
        {
            this.timelineDrawer.DrawTimeLine(2);

            this.timelineDrawer.AddEventToTimeLine(0, 100, "0");
            this.timelineDrawer.AddEventToTimeLine(0, 200, "1");
            this.timelineDrawer.AddEventToTimeLine(0, 300, "2");
            this.timelineDrawer.AddEventToTimeLine(0, 400, "3");
            this.timelineDrawer.AddEventToTimeLine(0, 500, "4");
            this.timelineDrawer.AddEventToTimeLine(0, 600, "5");
            this.timelineDrawer.AddEventToTimeLine(0, 700, "6");
            this.timelineDrawer.AddEventToTimeLine(0, 800, "7");
            this.timelineDrawer.AddEventToTimeLine(0, 900, "8");

            this.timelineDrawer.AddEventToTimeLine(1, 200, "1");
            this.timelineDrawer.AddEventToTimeLine(1, 300, "2");
            this.timelineDrawer.AddEventToTimeLine(1, 400, "3");
            this.timelineDrawer.AddEventToTimeLine(1, 500, "4");
            this.timelineDrawer.AddEventToTimeLine(1, 600, "5");
            this.timelineDrawer.AddEventToTimeLine(1, 700, "6");
            this.timelineDrawer.AddEventToTimeLine(1, 800, "7");
            this.timelineDrawer.AddEventToTimeLine(1, 900, "8");

            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 200, 1, 200);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 300, 1, 300);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 400, 1, 400);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 500, 1, 500);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 600, 1, 600);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 700, 1, 700);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 800, 1, 800);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 900, 1, 900);
        }

        public async Task Take()
        {
            this.timelineDrawer.DrawTimeLine(1);
            this.timelineDrawer.DrawTimeLine(1, 0, 600);

            this.timelineDrawer.AddEventToTimeLine(0, 100, "0");
            this.timelineDrawer.AddEventToTimeLine(0, 200, "1");
            this.timelineDrawer.AddEventToTimeLine(0, 300, "2");
            this.timelineDrawer.AddEventToTimeLine(0, 400, "3");
            this.timelineDrawer.AddEventToTimeLine(0, 500, "4");
            this.timelineDrawer.AddEventToTimeLine(0, 600, "5");
            this.timelineDrawer.AddEventToTimeLine(0, 700, "6");
            this.timelineDrawer.AddEventToTimeLine(0, 800, "7");
            this.timelineDrawer.AddEventToTimeLine(0, 900, "8");

            this.timelineDrawer.AddEventToTimeLine(1, 100, "0");
            this.timelineDrawer.AddEventToTimeLine(1, 200, "1");
            this.timelineDrawer.AddEventToTimeLine(1, 300, "2");
            this.timelineDrawer.AddEventToTimeLine(1, 400, "3");
            this.timelineDrawer.AddEventToTimeLine(1, 500, "4");
            this.timelineDrawer.AddCompletitionEventToTimeLine(1, 600, false);

            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 100, 1, 100);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 200, 1, 200);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 300, 1, 300);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 400, 1, 400);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 500, 1, 500);
        }

        public async Task TakeLast()
        {
            this.timelineDrawer.DrawTimeLine(0, 0, 700);
            this.timelineDrawer.DrawTimeLine(1, 0, 900);

            this.timelineDrawer.AddEventToTimeLine(0, 100, "0");
            this.timelineDrawer.AddEventToTimeLine(0, 300, "1");
            this.timelineDrawer.AddEventToTimeLine(0, 500, "2");
            this.timelineDrawer.AddCompletitionEventToTimeLine(0, 700, false);

            this.timelineDrawer.AddEventToTimeLine(1, 700, "1");
            this.timelineDrawer.AddEventToTimeLine(1, 800, "2");
            this.timelineDrawer.AddCompletitionEventToTimeLine(1, 900, false);

            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 700, 1, 700);
        }

        public async Task TakeUntil()
        {
            this.timelineDrawer.DrawTimeLine(1);
            this.timelineDrawer.DrawTimeLine(1, 0, 600);

            this.timelineDrawer.AddEventToTimeLine(0, 100, "0");
            this.timelineDrawer.AddEventToTimeLine(0, 200, "1");
            this.timelineDrawer.AddEventToTimeLine(0, 300, "2");
            this.timelineDrawer.AddEventToTimeLine(0, 400, "3");
            this.timelineDrawer.AddEventToTimeLine(0, 500, "4");
            this.timelineDrawer.AddEventToTimeLine(0, 600, "5");
            this.timelineDrawer.AddEventToTimeLine(0, 700, "6");
            this.timelineDrawer.AddEventToTimeLine(0, 800, "7");
            this.timelineDrawer.AddEventToTimeLine(0, 900, "8");

            this.timelineDrawer.AddEventToTimeLine(1, 100, "0");
            this.timelineDrawer.AddEventToTimeLine(1, 200, "1");
            this.timelineDrawer.AddEventToTimeLine(1, 300, "2");
            this.timelineDrawer.AddEventToTimeLine(1, 400, "3");
            this.timelineDrawer.AddEventToTimeLine(1, 500, "4");
            this.timelineDrawer.AddCompletitionEventToTimeLine(1, 600, false);

            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 100, 1, 100);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 200, 1, 200);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 300, 1, 300);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 400, 1, 400);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 500, 1, 500);
        }

        public async Task TakeWhile()
        {
            this.timelineDrawer.DrawTimeLine(1);
            this.timelineDrawer.DrawTimeLine(1, 0, 200);

            this.timelineDrawer.AddEventToTimeLine(0, 100, "0");
            this.timelineDrawer.AddEventToTimeLine(0, 200, "1");
            this.timelineDrawer.AddEventToTimeLine(0, 300, "2");
            this.timelineDrawer.AddEventToTimeLine(0, 400, "3");
            this.timelineDrawer.AddEventToTimeLine(0, 500, "4");
            this.timelineDrawer.AddEventToTimeLine(0, 600, "5");
            this.timelineDrawer.AddEventToTimeLine(0, 700, "6");
            this.timelineDrawer.AddEventToTimeLine(0, 800, "7");
            this.timelineDrawer.AddEventToTimeLine(0, 900, "8");

            this.timelineDrawer.AddEventToTimeLine(1, 100, "0");
            this.timelineDrawer.AddCompletitionEventToTimeLine(1, 200, false);

            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 100, 1, 100);
        }

        public async Task SkipAndTake()
        {
            this.timelineDrawer.DrawTimeLine(1);
            this.timelineDrawer.DrawTimeLine(1, 0, 800);

            this.timelineDrawer.AddEventToTimeLine(0, 100, "0");
            this.timelineDrawer.AddEventToTimeLine(0, 200, "1");
            this.timelineDrawer.AddEventToTimeLine(0, 300, "2");
            this.timelineDrawer.AddEventToTimeLine(0, 400, "3");
            this.timelineDrawer.AddEventToTimeLine(0, 500, "4");
            this.timelineDrawer.AddEventToTimeLine(0, 600, "5");
            this.timelineDrawer.AddEventToTimeLine(0, 700, "6");
            this.timelineDrawer.AddEventToTimeLine(0, 800, "7");
            this.timelineDrawer.AddEventToTimeLine(0, 900, "8");

            this.timelineDrawer.AddEventToTimeLine(1, 400, "3");
            this.timelineDrawer.AddEventToTimeLine(1, 500, "4");
            this.timelineDrawer.AddEventToTimeLine(1, 600, "5");
            this.timelineDrawer.AddEventToTimeLine(1, 700, "6");
            this.timelineDrawer.AddCompletitionEventToTimeLine(1, 800, false);

            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 400, 1, 400);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 500, 1, 500);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 600, 1, 600);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 700, 1, 700);
        }

        public async Task First()
        {
            this.timelineDrawer.DrawTimeLine(1);
            this.timelineDrawer.DrawTimeLine(1, 0, 200);

            this.timelineDrawer.AddEventToTimeLine(0, 100, "0");
            this.timelineDrawer.AddEventToTimeLine(0, 300, "1");
            this.timelineDrawer.AddEventToTimeLine(0, 500, "2");
            this.timelineDrawer.AddEventToTimeLine(0, 700, "3");
            this.timelineDrawer.AddEventToTimeLine(0, 900, "4");

            this.timelineDrawer.AddEventToTimeLine(1, 100, "0");
            this.timelineDrawer.AddCompletitionEventToTimeLine(1, 200, false);

            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 100, 1, 100);
        }

        public async Task Last()
        {
            this.timelineDrawer.DrawTimeLine(1);
            this.timelineDrawer.DrawTimeLine(1, 0, 900);

            this.timelineDrawer.AddEventToTimeLine(0, 100, "0");
            this.timelineDrawer.AddEventToTimeLine(0, 300, "1");
            this.timelineDrawer.AddEventToTimeLine(0, 500, "2");
            this.timelineDrawer.AddEventToTimeLine(0, 700, "3");
            this.timelineDrawer.AddCompletitionEventToTimeLine(0, 800, false);

            this.timelineDrawer.AddEventToTimeLine(1, 800, "3");
            this.timelineDrawer.AddCompletitionEventToTimeLine(1, 900, false);

            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 800, 1, 800);
        }

        public async Task ElementAt()
        {
            this.timelineDrawer.DrawTimeLine(1);
            this.timelineDrawer.DrawTimeLine(1, 0, 600);

            this.timelineDrawer.AddEventToTimeLine(0, 100, "0");
            this.timelineDrawer.AddEventToTimeLine(0, 300, "1");
            this.timelineDrawer.AddEventToTimeLine(0, 500, "2");
            this.timelineDrawer.AddEventToTimeLine(0, 700, "3");
            this.timelineDrawer.AddEventToTimeLine(0, 900, "4");

            this.timelineDrawer.AddEventToTimeLine(1, 500, "2");
            this.timelineDrawer.AddCompletitionEventToTimeLine(1, 600, false);

            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 500, 1, 500);
        }

        public async Task Single()
        {
            this.timelineDrawer.DrawTimeLine(0, 0, 200);
            this.timelineDrawer.DrawTimeLine(1, 0, 300);

            this.timelineDrawer.AddEventToTimeLine(0, 100, "42");
            this.timelineDrawer.AddCompletitionEventToTimeLine(0, 200, false);

            this.timelineDrawer.AddEventToTimeLine(1, 200, "42");
            this.timelineDrawer.AddCompletitionEventToTimeLine(1, 300, false);

            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 200, 1, 200);
        }

        public async Task DefaultIfEmpty()
        {
            this.timelineDrawer.DrawTimeLine(0, 0, 500);
            this.timelineDrawer.DrawTimeLine(1, 0, 500);
            this.timelineDrawer.DrawTimeLine(2, 0, 600);

            this.timelineDrawer.AddEventToTimeLine(0, 100, "0");
            this.timelineDrawer.AddEventToTimeLine(0, 200, "1");
            this.timelineDrawer.AddEventToTimeLine(0, 300, "2");
            this.timelineDrawer.AddEventToTimeLine(0, 400, "3");
            this.timelineDrawer.AddCompletitionEventToTimeLine(0, 500, false);

            this.timelineDrawer.AddCompletitionEventToTimeLine(1, 500, false);

            this.timelineDrawer.AddEventToTimeLine(2, 500, "42");
            this.timelineDrawer.AddCompletitionEventToTimeLine(2, 600, false);

            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 500, 1, 500);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(1, 500, 2, 500);
        }

        public async Task StartWith()
        {
            this.timelineDrawer.DrawTimeLine(2);

            this.timelineDrawer.AddEventToTimeLine(0, 100, "0");
            this.timelineDrawer.AddEventToTimeLine(0, 300, "1");
            this.timelineDrawer.AddEventToTimeLine(0, 500, "2");
            this.timelineDrawer.AddEventToTimeLine(0, 700, "3");
            this.timelineDrawer.AddEventToTimeLine(0, 900, "4");

            this.timelineDrawer.AddEventToTimeLine(1, 0, "42");
            this.timelineDrawer.AddEventToTimeLine(1, 100, "0");
            this.timelineDrawer.AddEventToTimeLine(1, 300, "1");
            this.timelineDrawer.AddEventToTimeLine(1, 500, "2");
            this.timelineDrawer.AddEventToTimeLine(1, 700, "3");
            this.timelineDrawer.AddEventToTimeLine(1, 900, "4");

            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 100, 1, 100);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 300, 1, 300);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 500, 1, 500);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 700, 1, 700);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 900, 1, 900);
        }

        public async Task Max()
        {
            this.timelineDrawer.DrawTimeLine(1);
            this.timelineDrawer.DrawTimeLine(1, 0, 900);

            this.timelineDrawer.AddEventToTimeLine(0, 100, "0");
            this.timelineDrawer.AddEventToTimeLine(0, 300, "1");
            this.timelineDrawer.AddEventToTimeLine(0, 500, "2");
            this.timelineDrawer.AddEventToTimeLine(0, 700, "3");
            this.timelineDrawer.AddCompletitionEventToTimeLine(0, 800, false);

            this.timelineDrawer.AddEventToTimeLine(1, 800, "3");
            this.timelineDrawer.AddCompletitionEventToTimeLine(1, 900, false);

            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 800, 1, 800);
        }

        public async Task Delay()
        {
            this.timelineDrawer.DrawTimeLine(2);

            this.timelineDrawer.AddEventToTimeLine(0, 100, "0");
            this.timelineDrawer.AddEventToTimeLine(0, 300, "1");
            this.timelineDrawer.AddEventToTimeLine(0, 500, "2");
            this.timelineDrawer.AddEventToTimeLine(0, 700, "3");
            this.timelineDrawer.AddEventToTimeLine(0, 900, "4");

            this.timelineDrawer.AddEventToTimeLine(1, 500, "0");
            this.timelineDrawer.AddEventToTimeLine(1, 700, "1");
            this.timelineDrawer.AddEventToTimeLine(1, 900, "2");

            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 100, 1, 500);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 300, 1, 700);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 500, 1, 900);
        }

        public async Task Throttle()
        {
            this.timelineDrawer.DrawTimeLine(2);

            this.timelineDrawer.AddEventToTimeLine(0, 100, "A");
            this.timelineDrawer.AddEventToTimeLine(0, 500, "B");
            this.timelineDrawer.AddEventToTimeLine(0, 600, "C");

            this.timelineDrawer.AddEventToTimeLine(1, 400, "A");
            this.timelineDrawer.AddEventToTimeLine(1, 900, "C");

            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 400, 1, 400);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 900, 1, 900);
        }

        public async Task Timeout()
        {
            this.timelineDrawer.DrawTimeLine(1);
            this.timelineDrawer.DrawTimeLine(1, 0, 300);

            this.timelineDrawer.AddEventToTimeLine(0, 500, "0");
            this.timelineDrawer.AddEventToTimeLine(0, 600, "1");
            this.timelineDrawer.AddEventToTimeLine(0, 700, "2");
            this.timelineDrawer.AddEventToTimeLine(0, 800, "3");
            this.timelineDrawer.AddEventToTimeLine(0, 900, "4");

            this.timelineDrawer.AddCompletitionEventToTimeLine(1, 300, true);

            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 300, 1, 300);
        }

        public async Task OnErrorResumeNext()
        {
            this.timelineDrawer.DrawTimeLine(0, 0, 300);
            this.timelineDrawer.DrawTimeLine(1, 300, 500);
            this.timelineDrawer.DrawTimeLine(2, 0, 500);

            this.timelineDrawer.AddEventToTimeLine(0, 100, "0");
            this.timelineDrawer.AddEventToTimeLine(0, 200, "1");
            this.timelineDrawer.AddCompletitionEventToTimeLine(0, 300, true);

            this.timelineDrawer.AddEventToTimeLine(1, 300, "10");
            this.timelineDrawer.AddEventToTimeLine(1, 400, "20");
            this.timelineDrawer.AddEventToTimeLine(1, 500, "30");

            this.timelineDrawer.AddEventToTimeLine(2, 100, "0");
            this.timelineDrawer.AddEventToTimeLine(2, 200, "1");
            this.timelineDrawer.AddEventToTimeLine(2, 300, "10");
            this.timelineDrawer.AddEventToTimeLine(2, 400, "20");
            this.timelineDrawer.AddEventToTimeLine(2, 500, "30");

            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 300, 1, 300);
        }

        public async Task Concat()
        {
            this.timelineDrawer.DrawTimeLine(0, 0, 600);
            this.timelineDrawer.DrawTimeLine(1, 0, 1000);
            this.timelineDrawer.DrawTimeLine(2, 0, 1000);

            this.timelineDrawer.AddEventToTimeLine(0, 100, "0");
            this.timelineDrawer.AddEventToTimeLine(0, 200, "1");
            this.timelineDrawer.AddEventToTimeLine(0, 500, "2");
            this.timelineDrawer.AddCompletitionEventToTimeLine(0, 600, false);

            this.timelineDrawer.AddEventToTimeLine(1, 400, "10");
            this.timelineDrawer.AddEventToTimeLine(1, 800, "20");

            this.timelineDrawer.AddEventToTimeLine(2, 100, "0");
            this.timelineDrawer.AddEventToTimeLine(2, 200, "1");
            this.timelineDrawer.AddEventToTimeLine(2, 500, "2");
            this.timelineDrawer.AddEventToTimeLine(2, 800, "20");

            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 100, 2, 100);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 200, 2, 200);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 500, 2, 500);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 600, 1, 600);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(1, 800, 2, 800);
        }

        public async Task Merge()
        {
            this.timelineDrawer.DrawTimeLine(3);

            this.timelineDrawer.AddEventToTimeLine(0, 100, "0");
            this.timelineDrawer.AddEventToTimeLine(0, 200, "1");
            this.timelineDrawer.AddEventToTimeLine(0, 500, "2");
            this.timelineDrawer.AddEventToTimeLine(0, 900, "3");

            this.timelineDrawer.AddEventToTimeLine(1, 400, "10");
            this.timelineDrawer.AddEventToTimeLine(1, 600, "20");
            this.timelineDrawer.AddEventToTimeLine(1, 800, "30");

            this.timelineDrawer.AddEventToTimeLine(2, 100, "0");
            this.timelineDrawer.AddEventToTimeLine(2, 200, "1");
            this.timelineDrawer.AddEventToTimeLine(2, 400, "10");
            this.timelineDrawer.AddEventToTimeLine(2, 500, "2");
            this.timelineDrawer.AddEventToTimeLine(2, 600, "20");
            this.timelineDrawer.AddEventToTimeLine(2, 800, "30");
            this.timelineDrawer.AddEventToTimeLine(2, 900, "3");

            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 100, 2, 100);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 200, 2, 200);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(1, 400, 2, 400);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 500, 2, 500);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(1, 600, 2, 600);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(1, 800, 2, 800);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 900, 2, 900);
        }

        public async Task Zip()
        {
            this.timelineDrawer.DrawTimeLine(3);
            
            this.timelineDrawer.AddEventToTimeLine(0, 100, "0");
            this.timelineDrawer.AddEventToTimeLine(0, 200, "1");
            this.timelineDrawer.AddEventToTimeLine(0, 800, "2");
            this.timelineDrawer.AddEventToTimeLine(0, 900, "3");

            this.timelineDrawer.AddEventToTimeLine(1, 300, "A");
            this.timelineDrawer.AddEventToTimeLine(1, 600, "B");
            this.timelineDrawer.AddEventToTimeLine(1, 700, "C");

            this.timelineDrawer.AddEventToTimeLine(2, 300, "0A");
            this.timelineDrawer.AddEventToTimeLine(2, 600, "1B");
            this.timelineDrawer.AddEventToTimeLine(2, 800, "2C");

            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 100, 2, 300);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(1, 300, 2, 300);

            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 200, 2, 600);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(1, 600, 2, 600);

            this.timelineDrawer.ConnectEventsOnDifferentTimelines(1, 700, 2, 800);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 800, 2, 800);
        }

        public async Task CombineLatest()
        {
            this.timelineDrawer.DrawTimeLine(3);

            this.timelineDrawer.AddEventToTimeLine(0, 100, "0");
            this.timelineDrawer.AddEventToTimeLine(0, 200, "1");
            this.timelineDrawer.AddEventToTimeLine(0, 500, "2");
            this.timelineDrawer.AddEventToTimeLine(0, 600, "3");

            this.timelineDrawer.AddEventToTimeLine(1, 400, "A");
            this.timelineDrawer.AddEventToTimeLine(1, 800, "B");

            this.timelineDrawer.AddEventToTimeLine(2, 400, "1A");
            this.timelineDrawer.AddEventToTimeLine(2, 500, "2A");
            this.timelineDrawer.AddEventToTimeLine(2, 600, "3A");
            this.timelineDrawer.AddEventToTimeLine(2, 800, "3B");

            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 200, 2, 400);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(1, 400, 2, 400);

            this.timelineDrawer.ConnectEventsOnDifferentTimelines(1, 400, 2, 500);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 500, 2, 500);

            this.timelineDrawer.ConnectEventsOnDifferentTimelines(1, 400, 2, 600);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 600, 2, 600);

            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 600, 2, 800);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(1, 800, 2, 800);
        }

        public async Task Scan()
        {
            this.timelineDrawer.DrawTimeLine(2);

            this.timelineDrawer.AddEventToTimeLine(0, 100, "0");
            this.timelineDrawer.AddEventToTimeLine(0, 300, "1");
            this.timelineDrawer.AddEventToTimeLine(0, 500, "2");
            this.timelineDrawer.AddEventToTimeLine(0, 700, "3");
            this.timelineDrawer.AddEventToTimeLine(0, 900, "4");

            this.timelineDrawer.AddEventToTimeLine(1, 100, "0");
            this.timelineDrawer.AddEventToTimeLine(1, 300, "1");
            this.timelineDrawer.AddEventToTimeLine(1, 500, "3");
            this.timelineDrawer.AddEventToTimeLine(1, 700, "6");
            this.timelineDrawer.AddEventToTimeLine(1, 900, "10");

            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 100, 1, 100);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 300, 1, 300);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 500, 1, 500);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 700, 1, 700);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 900, 1, 900);
        }

        public async Task Aggregate()
        {
            this.timelineDrawer.DrawTimeLine(0, 0, 600);
            this.timelineDrawer.DrawTimeLine(1, 0, 700);

            this.timelineDrawer.AddEventToTimeLine(0, 100, "0");
            this.timelineDrawer.AddEventToTimeLine(0, 200, "1");
            this.timelineDrawer.AddEventToTimeLine(0, 300, "2");
            this.timelineDrawer.AddEventToTimeLine(0, 400, "3");
            this.timelineDrawer.AddEventToTimeLine(0, 500, "4");
            this.timelineDrawer.AddCompletitionEventToTimeLine(0, 600, false);

            this.timelineDrawer.AddEventToTimeLine(1, 600, "10");
            this.timelineDrawer.AddCompletitionEventToTimeLine(1, 700, false);

            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 600, 1, 600);
        }

        public async Task GroupBy()
        {
            this.timelineDrawer.DrawTimeLine(0, 0, 1000);
            this.timelineDrawer.DrawTimeLine(1, 0, 1000);
            this.timelineDrawer.DrawTimeLine(2, 100, 1000);
            this.timelineDrawer.DrawTimeLine(3, 200, 1000);

            this.timelineDrawer.AddEventToTimeLine(0, 100, "0");
            this.timelineDrawer.AddEventToTimeLine(0, 200, "1");
            this.timelineDrawer.AddEventToTimeLine(0, 300, "2");
            this.timelineDrawer.AddEventToTimeLine(0, 400, "3");
            this.timelineDrawer.AddEventToTimeLine(0, 500, "4");
            this.timelineDrawer.AddEventToTimeLine(0, 600, "5");
            this.timelineDrawer.AddEventToTimeLine(0, 700, "6");
            this.timelineDrawer.AddEventToTimeLine(0, 800, "7");
            this.timelineDrawer.AddEventToTimeLine(0, 900, "8");

            this.timelineDrawer.AddEventToTimeLine(1, 100, "T");
            this.timelineDrawer.AddEventToTimeLine(1, 200, "F");

            this.timelineDrawer.AddEventToTimeLine(2, 100, "0");
            this.timelineDrawer.AddEventToTimeLine(2, 300, "2");
            this.timelineDrawer.AddEventToTimeLine(2, 500, "4");
            this.timelineDrawer.AddEventToTimeLine(2, 700, "6");
            this.timelineDrawer.AddEventToTimeLine(2, 900, "8");

            this.timelineDrawer.AddEventToTimeLine(3, 200, "1");
            this.timelineDrawer.AddEventToTimeLine(3, 400, "3");
            this.timelineDrawer.AddEventToTimeLine(3, 600, "5");
            this.timelineDrawer.AddEventToTimeLine(3, 800, "7");

            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 100, 2, 100);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 200, 3, 200);
        }

        public async Task GroupByAdvanced()
        {
            this.timelineDrawer.DrawTimeLine(0, 0, 1000);
            this.timelineDrawer.DrawTimeLine(1, 0, 1000);
            this.timelineDrawer.DrawTimeLine(2, 100, 1000);
            this.timelineDrawer.DrawTimeLine(3, 200, 1000);
            this.timelineDrawer.DrawTimeLine(4, 0, 1000);

            this.timelineDrawer.AddEventToTimeLine(0, 100, "0");
            this.timelineDrawer.AddEventToTimeLine(0, 200, "1");
            this.timelineDrawer.AddEventToTimeLine(0, 300, "2");
            this.timelineDrawer.AddEventToTimeLine(0, 400, "3");
            this.timelineDrawer.AddEventToTimeLine(0, 500, "4");
            this.timelineDrawer.AddEventToTimeLine(0, 600, "5");
            this.timelineDrawer.AddEventToTimeLine(0, 700, "6");
            this.timelineDrawer.AddEventToTimeLine(0, 800, "7");
            this.timelineDrawer.AddEventToTimeLine(0, 900, "8");

            this.timelineDrawer.AddEventToTimeLine(1, 100, "T");
            this.timelineDrawer.AddEventToTimeLine(1, 200, "F");

            this.timelineDrawer.AddEventToTimeLine(2, 150, "0");
            this.timelineDrawer.AddEventToTimeLine(2, 350, "2");
            this.timelineDrawer.AddEventToTimeLine(2, 550, "6");
            this.timelineDrawer.AddEventToTimeLine(2, 750, "12");
            this.timelineDrawer.AddEventToTimeLine(2, 950, "20");

            this.timelineDrawer.AddEventToTimeLine(3, 250, "1");
            this.timelineDrawer.AddEventToTimeLine(3, 450, "4");
            this.timelineDrawer.AddEventToTimeLine(3, 650, "9");
            this.timelineDrawer.AddEventToTimeLine(3, 850, "16");

            this.timelineDrawer.AddEventToTimeLine(4, 150, "T0");
            this.timelineDrawer.AddEventToTimeLine(4, 250, "F1");
            this.timelineDrawer.AddEventToTimeLine(4, 350, "T2");
            this.timelineDrawer.AddEventToTimeLine(4, 450, "F4");
            this.timelineDrawer.AddEventToTimeLine(4, 550, "T6");
            this.timelineDrawer.AddEventToTimeLine(4, 650, "F9");
            this.timelineDrawer.AddEventToTimeLine(4, 750, "T12");
            this.timelineDrawer.AddEventToTimeLine(4, 850, "F16");
            this.timelineDrawer.AddEventToTimeLine(4, 950, "T20");
            
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 100, 2, 100);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 200, 3, 200);

            this.timelineDrawer.ConnectEventsOnDifferentTimelines(2, 150, 4, 150);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(2, 350, 4, 350);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(2, 550, 4, 550);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(2, 750, 4, 750);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(2, 950, 4, 950);

            this.timelineDrawer.ConnectEventsOnDifferentTimelines(3, 250, 4, 250);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(3, 450, 4, 450);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(3, 650, 4, 650);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(3, 850, 4, 850);
        }

        public async Task ColdObservableSample()
        {
            this.timelineDrawer.DrawTimeLine(0, 0, 1000);
            this.timelineDrawer.DrawTimeLine(1, 400, 1000);

            this.timelineDrawer.AddEventToTimeLine(0, 100, "0");
            this.timelineDrawer.AddEventToTimeLine(0, 300, "1");
            this.timelineDrawer.AddEventToTimeLine(0, 500, "2");
            this.timelineDrawer.AddEventToTimeLine(0, 700, "3");
            this.timelineDrawer.AddEventToTimeLine(0, 900, "4");

            this.timelineDrawer.AddEventToTimeLine(1, 500, "0");
            this.timelineDrawer.AddEventToTimeLine(1, 700, "1");
            this.timelineDrawer.AddEventToTimeLine(1, 900, "2");
        }

        public async Task PublishSample1()
        {
            this.timelineDrawer.DrawTimeLine(0, 0, 1000);
            this.timelineDrawer.DrawTimeLine(1, 400, 1000);

            this.timelineDrawer.AddEventToTimeLine(0, 100, "0");
            this.timelineDrawer.AddEventToTimeLine(0, 300, "1");
            this.timelineDrawer.AddEventToTimeLine(0, 500, "2");
            this.timelineDrawer.AddEventToTimeLine(0, 700, "3");
            this.timelineDrawer.AddEventToTimeLine(0, 900, "4");

            this.timelineDrawer.AddEventToTimeLine(1, 500, "2");
            this.timelineDrawer.AddEventToTimeLine(1, 700, "3");
            this.timelineDrawer.AddEventToTimeLine(1, 900, "4");
        }

        public async Task PublishSample2()
        {
            this.timelineDrawer.DrawTimeLine(0, 0, 1000);
            this.timelineDrawer.DrawTimeLine(1, 400, 1000);

            this.timelineDrawer.AddEventToTimeLine(0, 500, "0");
            this.timelineDrawer.AddEventToTimeLine(0, 700, "1");
            this.timelineDrawer.AddEventToTimeLine(0, 900, "2");

            this.timelineDrawer.AddEventToTimeLine(1, 500, "0");
            this.timelineDrawer.AddEventToTimeLine(1, 700, "1");
            this.timelineDrawer.AddEventToTimeLine(1, 900, "2");
        }

        public async Task HotObservableSample()
        {
            this.timelineDrawer.DrawTimeLine(0, 0, 1000);
            this.timelineDrawer.DrawTimeLine(1, 400, 1000);

            this.timelineDrawer.AddEventToTimeLine(0, 100, "0");
            this.timelineDrawer.AddEventToTimeLine(0, 300, "1");
            this.timelineDrawer.AddEventToTimeLine(0, 500, "2");
            this.timelineDrawer.AddEventToTimeLine(0, 700, "3");
            this.timelineDrawer.AddEventToTimeLine(0, 900, "4");

            this.timelineDrawer.AddEventToTimeLine(1, 500, "2");
            this.timelineDrawer.AddEventToTimeLine(1, 700, "3");
            this.timelineDrawer.AddEventToTimeLine(1, 900, "4");
        }

        public async Task ReplaySample1()
        {
            this.timelineDrawer.DrawTimeLine(0, 0, 1000);
            this.timelineDrawer.DrawTimeLine(1, 400, 1000);

            this.timelineDrawer.AddEventToTimeLine(0, 100, "0");
            this.timelineDrawer.AddEventToTimeLine(0, 300, "1");
            this.timelineDrawer.AddEventToTimeLine(0, 500, "2");
            this.timelineDrawer.AddEventToTimeLine(0, 700, "3");
            this.timelineDrawer.AddEventToTimeLine(0, 900, "4");

            this.timelineDrawer.AddEventToTimeLine(1, 400, "0");
            this.timelineDrawer.AddEventToTimeLine(1, 475, "1");
            this.timelineDrawer.AddEventToTimeLine(1, 550, "2");
            this.timelineDrawer.AddEventToTimeLine(1, 700, "3");
            this.timelineDrawer.AddEventToTimeLine(1, 900, "4");
        }

        public async Task ReplaySample2()
        {
            this.timelineDrawer.DrawTimeLine(0, 0, 1000);
            this.timelineDrawer.DrawTimeLine(1, 400, 1000);

            this.timelineDrawer.AddEventToTimeLine(0, 500, "2");
            this.timelineDrawer.AddEventToTimeLine(0, 700, "3");
            this.timelineDrawer.AddEventToTimeLine(0, 900, "4");

            this.timelineDrawer.AddEventToTimeLine(1, 500, "2");
            this.timelineDrawer.AddEventToTimeLine(1, 700, "3");
            this.timelineDrawer.AddEventToTimeLine(1, 900, "4");
        }

        public async Task RefCount()
        {
            this.timelineDrawer.DrawTimeLine(0, 0, 600);
            this.timelineDrawer.DrawTimeLine(1, 300, 600);
            this.timelineDrawer.DrawTimeLine(2, 600, 1000);

            this.timelineDrawer.AddEventToTimeLine(0, 100, "0");
            this.timelineDrawer.AddEventToTimeLine(0, 200, "1");
            this.timelineDrawer.AddEventToTimeLine(0, 300, "2");
            this.timelineDrawer.AddEventToTimeLine(0, 400, "3");
            this.timelineDrawer.AddEventToTimeLine(0, 500, "4");

            this.timelineDrawer.AddEventToTimeLine(1, 400, "3");
            this.timelineDrawer.AddEventToTimeLine(1, 500, "4");

            this.timelineDrawer.AddEventToTimeLine(2, 700, "0");
            this.timelineDrawer.AddEventToTimeLine(2, 800, "1");
            this.timelineDrawer.AddEventToTimeLine(2, 900, "2");
        }

        public async Task Sample()
        {
            this.timelineDrawer.DrawTimeLine(0, 0, 1000);
            this.timelineDrawer.DrawTimeLine(1, 0, 1000);

            this.timelineDrawer.AddEventToTimeLine(0, 100, "A");
            this.timelineDrawer.AddEventToTimeLine(0, 200, "B");
            this.timelineDrawer.AddEventToTimeLine(0, 400, "C");
            this.timelineDrawer.AddEventToTimeLine(0, 700, "D");
            this.timelineDrawer.AddEventToTimeLine(0, 900, "E");

            this.timelineDrawer.AddEventToTimeLine(1, 300, "B");
            this.timelineDrawer.AddEventToTimeLine(1, 600, "C");
            this.timelineDrawer.AddEventToTimeLine(1, 900, "E");

            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 300, 1, 300);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 600, 1, 600);
            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 900, 1, 900);
        }

        public async Task Materialize_OnNext()
        {
            this.timelineDrawer.DrawTimeLine(0, 0, 200);
            this.timelineDrawer.DrawTimeLine(1, 0, 200);

            this.timelineDrawer.AddEventToTimeLine(0, 100, "1");
            this.timelineDrawer.AddEventToTimeLine(1, 100, "(1)");

            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 100, 1, 100);
        }

        public async Task Materialize_OnCompleted()
        {
            this.timelineDrawer.DrawTimeLine(0, 0, 200);
            this.timelineDrawer.DrawTimeLine(1, 0, 200);

            this.timelineDrawer.AddCompletitionEventToTimeLine(0, 100, false);

            this.timelineDrawer.AddEventToTimeLine(1, 100, "(C)");
            this.timelineDrawer.AddCompletitionEventToTimeLine(1, 200, false);

            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 100, 1, 100);
        }

        public async Task Materialize_OnError()
        {
            this.timelineDrawer.DrawTimeLine(0, 0, 200);
            this.timelineDrawer.DrawTimeLine(1, 0, 200);

            this.timelineDrawer.AddCompletitionEventToTimeLine(0, 100, true);

            this.timelineDrawer.AddEventToTimeLine(1, 100, "(E)");
            this.timelineDrawer.AddCompletitionEventToTimeLine(1, 200, false);

            this.timelineDrawer.ConnectEventsOnDifferentTimelines(0, 100, 1, 100);
        }
    }
}
