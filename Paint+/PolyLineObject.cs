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
    public class PolyLineObject : BaseObject
    {
        public Polyline rect;
        public UIElement Create()
        {
            rect = new Polyline();
            Canvas.SetLeft(rect, 0);
            Canvas.SetTop(rect, 0);
            rect.Stroke = new SolidColorBrush(colorStroke);
            rect.Fill = new SolidColorBrush(colorFill);
            Point Point1 = new System.Windows.Point(0, 0);
            PointCollection polygonPoints = new PointCollection();
            polygonPoints.Add(Point1);
            rect.StrokeThickness = penWidth;
            rect.Points = polygonPoints;
        
            return rect;
        }


    }
}
