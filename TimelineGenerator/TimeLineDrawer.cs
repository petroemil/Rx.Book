using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace TimelineGenerator
{
    public class TimeLineDrawer
    {
        private readonly Canvas canvas;
        private readonly Border border;

        private readonly double maxMiliseconds = 1000;
        private readonly double timeLineSpacing = 50;
        private readonly double eventSize = 25;
        private readonly double timelineThickness = 10;
        private readonly double connectionThickness = 3;
        private readonly double maxCanvasWidth = 300;
        private readonly double fontSize = 12;

        public TimeLineDrawer(Canvas canvas, Border border)
        {
            this.canvas = canvas;
            this.border = border;
        }

        private double x1, y1;
        private double x2, y2;
        private void TrySetBoundingBox(double x1, double y1, double x2, double y2)
        {
            if (this.x1 > x1) this.x1 = x1;
            if (this.y1 > y1) this.y1 = y1;
            if (this.x2 < x2) this.x2 = x2;
            if (this.y2 < y2) this.y2 = y2;
        }

        public void ClearCanvas()
        {
            this.canvas.Children.Clear();

            this.x1 = this.y1 = int.MaxValue;
            this.x2 = this.y2 = this.timelineThickness;
        }

        public void SetCanvasSize()
        {
            this.border.Width = this.x2 - this.x1;
            this.border.Height = this.y2 - this.y1;

            this.canvas.Margin = new Thickness(-this.x1, -this.y1, 0, 0);
        }

        public void DrawTimeLine(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var x1 = 0;
                var x2 = this.maxCanvasWidth + (this.timelineThickness / 2);

                var y = this.timeLineSpacing * (i + 1);

                var timelineRoundedEndSize = this.timelineThickness / 2;
                this.TrySetBoundingBox(x1 - timelineRoundedEndSize, y, x2 + timelineRoundedEndSize, y);

                var timeLine = new Line();
                timeLine.StrokeThickness = this.timelineThickness;
                timeLine.Stroke = new SolidColorBrush(Colors.LightGray);
                timeLine.StrokeStartLineCap = PenLineCap.Round;
                timeLine.StrokeEndLineCap = PenLineCap.Round;
                timeLine.SetValue(Canvas.ZIndexProperty, 0.0);
                timeLine.X1 = x1;
                timeLine.Y1 = y;
                timeLine.X2 = x2;
                timeLine.Y2 = y;

                this.canvas.Children.Add(timeLine);
            }
        }

        public void DrawTimeLine(double index, double start, double end)
        {
            var x1 = (start / this.maxMiliseconds * this.maxCanvasWidth);
            var x2 = (end / this.maxMiliseconds * this.maxCanvasWidth);

            var y = this.timeLineSpacing * (index + 1);

            var timelineRoundedEndSize = this.timelineThickness / 2;
            this.TrySetBoundingBox(x1 - timelineRoundedEndSize, y, x2 + timelineRoundedEndSize, y);

            var timeLine = new Line();
            timeLine.StrokeThickness = this.timelineThickness;
            timeLine.Stroke = new SolidColorBrush(Colors.LightGray);
            timeLine.StrokeStartLineCap = PenLineCap.Round;
            timeLine.StrokeEndLineCap = PenLineCap.Round;
            timeLine.SetValue(Canvas.ZIndexProperty, 0.0);
            timeLine.X1 = x1;
            timeLine.Y1 = y;
            timeLine.X2 = x2;
            timeLine.Y2 = y;

            this.canvas.Children.Add(timeLine);
        }

        public void ConnectEventsOnDifferentTimelines(double index1, double ms1, double index2, double ms2)
        {
            var x1 = ms1 / this.maxMiliseconds * this.maxCanvasWidth;
            var y1 = this.timeLineSpacing * (index1 + 1);

            var x2 = ms2 / this.maxMiliseconds * this.maxCanvasWidth;
            var y2 = this.timeLineSpacing * (index2 + 1);

            this.TrySetBoundingBox(x1, y1, x2, y2);

            var connector = new Line();
            connector.StrokeThickness = this.connectionThickness;
            connector.Stroke = new SolidColorBrush(Colors.DarkGray);
            connector.StrokeStartLineCap = PenLineCap.Round;
            connector.StrokeEndLineCap = PenLineCap.Round;
            connector.SetValue(Canvas.ZIndexProperty, 0.0);
            connector.X1 = x1;
            connector.Y1 = y1;
            connector.X2 = x2;
            connector.Y2 = y2;

            this.canvas.Children.Add(connector);
        }

        public void AddCompletitionEventToTimeLine(double index, double ms, bool isError)
        {
            var x1 = (ms / this.maxMiliseconds * this.maxCanvasWidth) - (this.eventSize / 2);
            var y1 = ((index + 1) * this.timeLineSpacing) - (this.eventSize / 2);

            var x2 = x1 + this.eventSize;
            var y2 = y1 + this.eventSize;

            this.TrySetBoundingBox(x1, y1, x2, y2);

            var ellipse = new Ellipse();
            ellipse.Width = this.eventSize;
            ellipse.Height = this.eventSize;
            ellipse.SetValue(Canvas.LeftProperty, x1);
            ellipse.SetValue(Canvas.TopProperty, y1);
            ellipse.Fill = new SolidColorBrush(isError ? Colors.DarkRed : Colors.DarkGreen);
            ellipse.SetValue(Canvas.ZIndexProperty, 1.0);

            this.canvas.Children.Add(ellipse);
        }

        public void AddEventToTimeLine(double index, double ms, string value)
        {
            var x1 = (ms / this.maxMiliseconds * this.maxCanvasWidth) - (this.eventSize / 2);
            var y1 = ((index + 1) * this.timeLineSpacing) - (this.eventSize / 2);

            var x2 = x1 + this.eventSize;
            var y2 = y1 + this.eventSize;

            this.TrySetBoundingBox(x1, y1, x2, y2);

            var container = new Grid();
            container.HorizontalAlignment = HorizontalAlignment.Left;
            container.VerticalAlignment = VerticalAlignment.Top;
            container.SetValue(Canvas.LeftProperty, x1);
            container.SetValue(Canvas.TopProperty, y1);
            container.SetValue(Canvas.ZIndexProperty, 1.0);

            var ellipse = new Ellipse();
            ellipse.Width = this.eventSize;
            ellipse.Height = this.eventSize;
            ellipse.Fill = new SolidColorBrush(Colors.Gray);
            container.Children.Add(ellipse);

            var textBlock = new TextBlock();
            textBlock.HorizontalAlignment = HorizontalAlignment.Center;
            textBlock.VerticalAlignment = VerticalAlignment.Center;
            textBlock.Foreground = new SolidColorBrush(Colors.White);
            textBlock.FontFamily = new FontFamily("Consolas");
            textBlock.Text = value;
            textBlock.FontSize = this.fontSize;
            textBlock.Margin = new Thickness(0, -1, 0, 0);
            container.Children.Add(textBlock);

            this.canvas.Children.Add(container);
        }
    }
}