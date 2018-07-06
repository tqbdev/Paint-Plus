using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Paint_
{
    public class LineObject : BaseObject
    {
        public Line rect;
        public UIElement Create()
        {
            rect = new Line();
            rect.Stroke = new SolidColorBrush(colorStroke);
            rect.Fill = new SolidColorBrush(colorFill);
            rect.X1 = 0;
            rect.X2 = 300;
            rect.Y1 = 0;
            rect.Y2 = 300;
            if (styleLine == StyleLines.Dash)
            {
                double[] dashes = { 4, 4 };
                rect.StrokeDashArray = new System.Windows.Media.DoubleCollection(dashes);
            }
            else if (styleLine == StyleLines.Dot)
            {
                double[] dashes = { 1, 1 };
                rect.StrokeDashArray = new System.Windows.Media.DoubleCollection(dashes);
            }
            else if (styleLine == StyleLines.DashDot)
            {
                double[] dashes = { 4, 1, 1, 1 };
                rect.StrokeDashArray = new System.Windows.Media.DoubleCollection(dashes);
            }
            else if (styleLine == StyleLines.DashDotDot)
            {
                double[] dashes = { 4, 1, 1, 1, 1, 1 };
                rect.StrokeDashArray = new System.Windows.Media.DoubleCollection(dashes);
            }
            rect.StrokeThickness = penWidth;
            Canvas.SetLeft(rect, 0);
            Canvas.SetTop(rect, 0);
            return rect;
        }

    }
}
