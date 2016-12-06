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
        private readonly double canvasWidth = 300;
        private readonly double timeLineSpacing = 50;
        private readonly double eventSize = 25;

        private double canvasHeight = 0;
        private double canvasTop = 0;

        public TimeLineDrawer(Canvas canvas, Border border)
        {
            this.canvas = canvas;
            this.border = border;
        }

        private void TrySetCanvasHeight(double y)
        {
            if (y > this.canvasHeight) this.canvasHeight = y;
        }

        private void TrySetCanvasTop(double y)
        {
            if (this.canvasTop == 0 || y < this.canvasTop) this.canvasTop = y;
        }

        public void ClearCanvas()
        {
            this.canvas.Children.Clear();
            this.canvasHeight = 0;
            this.canvasTop = 0;
        }

        public void SetCanvasSize()
        {
            this.canvas.Width = this.canvasWidth;
            this.canvas.Height = this.canvasHeight;
            
            this.border.Width = this.canvasWidth;
            this.border.Height = this.canvasHeight - this.canvasTop;

            this.canvas.Margin = new Thickness(0, -this.canvasTop, 0, 0);
        }

        public void DrawTimeLine(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var x1 = 0;
                var x2 = this.canvasWidth;

                var y = this.timeLineSpacing * (i + 1);

                this.TrySetCanvasHeight(y);
                this.TrySetCanvasTop(y);

                var timeLine = new Line();
                timeLine.StrokeThickness = 10;
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
            var x1 = start / this.maxMiliseconds * this.canvasWidth;
            var x2 = end / this.maxMiliseconds * this.canvasWidth;

            var y = this.timeLineSpacing * (index + 1);

            this.TrySetCanvasHeight(y);
            this.TrySetCanvasTop(y);

            var timeLine = new Line();
            timeLine.StrokeThickness = 10;
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
            var x1 = ms1 / this.maxMiliseconds * this.canvasWidth;
            var y1 = this.timeLineSpacing * (index1 + 1);

            var x2 = ms2 / this.maxMiliseconds * this.canvasWidth;
            var y2 = this.timeLineSpacing * (index2 + 1);

            this.TrySetCanvasHeight(y1);
            this.TrySetCanvasHeight(y2);

            this.TrySetCanvasTop(y1);
            this.TrySetCanvasTop(y2);

            var connector = new Line();
            connector.StrokeThickness = 3;
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
            var x1 = (ms / this.maxMiliseconds * this.canvasWidth) - (this.eventSize / 2);
            var y1 = ((index + 1) * this.timeLineSpacing) - (this.eventSize / 2);

            var x2 = x1 + this.eventSize;
            var y2 = y1 + this.eventSize;

            this.TrySetCanvasHeight(y2);
            this.TrySetCanvasTop(y1);

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
            var x1 = (ms / this.maxMiliseconds * this.canvasWidth) - (this.eventSize / 2);
            var y1 = ((index + 1) * this.timeLineSpacing) - (this.eventSize / 2);

            var x2 = x1 + this.eventSize;
            var y2 = y1 + this.eventSize;

            this.TrySetCanvasHeight(y2);
            this.TrySetCanvasTop(y1);

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
            textBlock.FontSize = 12;
            textBlock.Margin = new Thickness(0, -1, 0, 0);
            container.Children.Add(textBlock);

            this.canvas.Children.Add(container);
        }
    }
}