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
    public class RectangleObject : BaseObject
    {
        public Rectangle rect;
        public UIElement Create()
        {
            Rectangle rect = new Rectangle();
            rect.Stroke = new SolidColorBrush(colorStroke);
            rect.Fill = new SolidColorBrush(colorFill);
            rect.Width = 300;
            rect.Height = 300;
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
