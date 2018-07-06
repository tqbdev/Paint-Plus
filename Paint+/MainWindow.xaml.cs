using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Ribbon;
using System.IO;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Paint_
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region Member
        const double ScaleRate = 1.1;
        Selector controlSelector;
        TypeShape TypeDraw = TypeShape.None;
        bool isDraw = true;
        int zIndexTop = 1;
        int zIndexBack = -1;
        int penWidth = 1;
        bool isEraser = false;
        WriteableBitmap MyBitmap;
        StyleLines styleLine = StyleLines.Soild;
        Polyline rect;
        History history;
        Color outlineColor = Colors.Black;
        Color fillColor = Colors.Transparent;
        TranslateTransform Translate;
        bool IsMouseDown;
        double xOffset;
        double yOffset;
        Point initial;

        Color textColor = Colors.Black;
        FontFamily fontFamilyText;
        int fontSizeText = 8;
        #endregion

        #region Contructor
        public MainWindow()
        {
            InitializeComponent();
            history = new History(); // Khởi tạo lịch sử
            controlSelector = new Selector(MainCanvas); // Khởi tạo Selector
            ScalePercent.Text = "100";
            
            for (int i = 8; i < 73; i++)
            {
                fontSize.Items.Add(i.ToString());
            }

            fontSize.SelectedIndex = 0;
        }
        #endregion

        #region Function

        private void ScrollView_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MainCanvas_MouseUp(this, e);
        }
        private void ScrollView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainCanvas_MouseDown(this, e);
        }
        private void ScrollView_MouseMove(object sender, MouseEventArgs e)
        {
            MainCanvas_MouseMove(this, e);
        }


        private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                Point p = Mouse.GetPosition(MainCanvas);
                Transform.CenterX = MainCanvas.Width / 2;
                Transform.CenterY = MainCanvas.Height / 2;
                
                if (e.Delta > 0)
                {
                    Transform.ScaleX *= ScaleRate;
                    Transform.ScaleY *= ScaleRate;
                }
                else
                {
                    Transform.ScaleX /= ScaleRate;
                    Transform.ScaleY /= ScaleRate;
                }

                double scalePercent = Transform.ScaleX * 100;
                ScalePercent.Text = Math.Round(scalePercent, 2).ToString();

                e.Handled = true;
            }
        }

        private void MainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (!IsMouseDown) return;
                ///MainCanvainitial = Mouse.GetPosition(this);
                //xOffset = //Mouse.GetPosition(this).X - initial.X;
                //yOffset = //Mouse.GetPosition(this).Y - initial.Y;
                //MainCanvas.RenderTransform = 
                Translate = new TranslateTransform
                {
                    X = xOffset + Mouse.GetPosition(this).X - initial.X,
                    Y = yOffset + Mouse.GetPosition(this).Y - initial.Y
                };
                TransformGroup group = new TransformGroup();
                group.Children.Add(Translate);
                group.Children.Add(Transform);
                MainCanvas.RenderTransform = group;
                return;
            }
            if (isEraser == true && TypeDraw == TypeShape.Eraser)
            {
                Point point = Mouse.GetPosition(MainCanvas);
                BitmapSource bmp = (BitmapSource)MainCanvasImage.Source;

                if (bmp == null) return;

                MyBitmap = new WriteableBitmap(bmp);
                MainCanvasImage.Source = MyBitmap;
                ImageProcess.Rubber((int)point.X, (int)point.Y, penWidth, MyBitmap);
            }
            if (TypeDraw == TypeShape.PolyLine)
            {
                if (controlSelector.itemChoose != null)
                {
                    // Mouse.GetPosition(MainCanvas);
                    Point CurPoint = Mouse.GetPosition(MainCanvas);
                    CurPoint = MainCanvas.PointToScreen(CurPoint);
                    CurPoint = ((PolyLineObject)controlSelector.itemChoose.objectBase).rect.PointFromScreen(CurPoint);
                    ((PolyLineObject)controlSelector.itemChoose.objectBase).rect.Points.Add(CurPoint);
                }
                return;
            }
            if (TypeDraw == TypeShape.PolyGon)
            {
                if (controlSelector.itemChoose != null)
                {
                    // Mouse.GetPosition(MainCanvas);
                    Point CurPoint = Mouse.GetPosition(MainCanvas);
                    CurPoint = MainCanvas.PointToScreen(CurPoint);
                    CurPoint = ((PolyGonObject)controlSelector.itemChoose.objectBase).rect.PointFromScreen(CurPoint);
                    PointCollection list = ((PolyGonObject)controlSelector.itemChoose.objectBase).rect.Points;
                    ((PolyGonObject)controlSelector.itemChoose.objectBase).rect.Points[list.Count - 1] = CurPoint;
                }
            }
            controlSelector.OnMouse_Move();
            //Set thông tin statusbar
            Point p = Mouse.GetPosition(MainCanvas);
            lblCursorPosition.Text = "X: " + ((int)p.X).ToString() + ", Y: " + ((int)p.Y).ToString();
        }

        private void MainCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {


            if (e.RightButton == MouseButtonState.Pressed) return;
            if( IsMouseDown != false){
                 IsMouseDown = false;
                 yOffset = Translate.Y;
                 xOffset = Translate.X;
                 return;
            }
            controlSelector.OnMouse_Up();
            if (isEraser == true && TypeDraw == TypeShape.Eraser)
            {
                isEraser = false;
                SaveHistory();
                return;
            }
            if (TypeDraw == TypeShape.PolyGon) return;
            if (TypeDraw == TypeShape.PolyLine)
            {
                if (controlSelector.itemChoose != null)
                {
                    controlSelector.itemChoose.Selected = false;
                    controlSelector.itemChoose = null;
                    SaveHistory();
                    for (int i = 0; i < MainCanvas.Children.Count; ++i)
                    {
                        if (!(MainCanvas.Children[i] is ItemLayer)) continue;
                        ItemLayer item = (ItemLayer)MainCanvas.Children[i];
                        MainCanvas.Children.Remove((ItemLayer)MainCanvas.Children[i]);
                    }
                }
                //TypeDraw = TypeShape.None; 
                return;
            }
            //btnSelect.IsChecked = true;
            TypeDraw = TypeShape.None;

            //if (TypeDraw != TypeShape.None)
            //{
            //    TypeDraw = TypeShape.None;
            //}
        }

        private void MainCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed) return;
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                IsMouseDown = true;
                initial = Mouse.GetPosition(this);
                return;
            }
            if (TypeDraw == TypeShape.Fill || TypeDraw == TypeShape.Eraser)
            {
                return;
            }
            if (controlSelector.itemChoose != null && TypeDraw != TypeShape.PolyGon)
            {
                controlSelector.OnMouse_Down();
                return;
            }

            if (TypeDraw == TypeShape.PolyGon)
            {
                if (controlSelector.itemChoose != null)
                {
                    if (e.ClickCount == 2)
                    {
                        TypeDraw = TypeShape.None;
                        controlSelector.itemChoose.Selected = false;
                        controlSelector.itemChoose = null;
                        SaveHistory();
                        for (int i = 0; i < MainCanvas.Children.Count; ++i)
                        {
                            if (!(MainCanvas.Children[i] is ItemLayer)) continue;
                            ItemLayer item = (ItemLayer)MainCanvas.Children[i];
                            MainCanvas.Children.Remove((ItemLayer)MainCanvas.Children[i]);
                        }
                        return;
                    }
                    // Mouse.GetPosition(MainCanvas);
                    Point CurPoint = Mouse.GetPosition(MainCanvas);
                    CurPoint = MainCanvas.PointToScreen(CurPoint);
                    CurPoint = ((PolyGonObject)controlSelector.itemChoose.objectBase).rect.PointFromScreen(CurPoint);
                    ((PolyGonObject)controlSelector.itemChoose.objectBase).rect.Points.Add(CurPoint);
                    return;
                }
            }

            if (TypeDraw == TypeShape.None)
            {
                if (controlSelector.itemChoose != null)
                {
                    if (e.ClickCount == 2 && controlSelector.itemChoose.Type == TypeShape.Text)
                    {
                        MessageBox.Show("Show tab chỉnh sửa text");
                        /*Tạo 1 tab mới để chỉnh sửa*/
                        ItemLayer itemLayer = controlSelector.itemChoose;
                        itemLayer.RectText = "Set lại dòng text";
                        itemLayer.RectFontSize = 30;
                        itemLayer.ColorStroke = Color.FromRgb(255, 255, 0);
                        itemLayer.RectFontStyle = FontStyles.Italic;
                        itemLayer.RectFontStretch = FontStretches.ExtraExpanded;
                        return;
                    }
                }
                controlSelector.OnMouse_Down();
            }
            else if (TypeDraw != TypeShape.Text)
            {
                Point CurPoint = Mouse.GetPosition(MainCanvas);

                ItemLayer item = new ItemLayer(TypeDraw); // TypeDraw loại hình
                Canvas.SetLeft(item, CurPoint.X);
                Canvas.SetTop(item, CurPoint.Y);
                item.Width = 1;
                item.Height = 1;
                item.ColorStroke = outlineColor; // tùy chỉnh màu vẽ
                item.ColorFill = fillColor; // Tùy chỉnh màu fill, không fill thì = new Color();
                item.PenWidth = penWidth; // Tùy chỉnh Độ rộng
                item.MainCanvas_MouseDown += MainCanvas_MouseDown;
                item.MainCanvas_MouseUp += MainCanvas_MouseUp;
                item.MainCanvas_MouseMove += MainCanvas_MouseMove;
                item.StyleLine = styleLine;
                item.Create();
                MainCanvas.Children.Add(item);
                //history.PushBack(item);
                controlSelector.SetItemCreate(item);

            }
            else
            {

                Point CurPoint = Mouse.GetPosition(MainCanvas);

                ItemLayer item = new ItemLayer(TypeShape.Text); // TypeDraw loại hình
                Canvas.SetLeft(item, CurPoint.X);
                Canvas.SetTop(item, CurPoint.Y);
                item.Width = 1;
                item.Height = 1;
                item.MainCanvas_MouseDown += MainCanvas_MouseDown;
                item.MainCanvas_MouseUp += MainCanvas_MouseUp;
                item.MainCanvas_MouseMove += MainCanvas_MouseMove;

                if (textToolsGroup != null) textToolsGroup.Visibility = Visibility.Visible;
                if (MainRibbon != null) MainRibbon.SelectedTabIndex = 0;

                item.Create();
                /*Sử dụng sau hàm Create*/
                item.RectFontFamily = fontFamilyText;
                item.RectText = "Helllo31387126378216hfdjisdhfkjsdhkfjdshf";
                item.RectFontSize = fontSizeText;
                item.ColorStroke = textColor; // tùy chỉnh màu vẽ

                MainCanvas.Children.Add(item);
                //history.PushBack(item);
                controlSelector.SetItemCreate(item);
            }
        }

        private void MainCanvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                return;
            }
            if (!(e.Source != MainCanvas && e.Source.GetType() == typeof(ItemLayer) && controlSelector.itemChoose == (ItemLayer)e.Source))
            {

                for (int i = 0; i < MainCanvas.Children.Count; ++i)
                {
                    if (!(MainCanvas.Children[i] is ItemLayer)) continue;
                    ItemLayer item = (ItemLayer)MainCanvas.Children[i];
                    if (item.Selected == true)
                    {
                        item.Selected = false;
                        SaveHistory();
                    }
                    //Canvas.SetZIndex(item, 0);
                }
                controlSelector.itemChoose = null;
                //SaveCanvas();
                for (int i = 0; i < MainCanvas.Children.Count; ++i)
                {
                    if (!(MainCanvas.Children[i] is ItemLayer)) continue;
                    ItemLayer item = (ItemLayer)MainCanvas.Children[i];
                    MainCanvas.Children.Remove((ItemLayer)MainCanvas.Children[i]);
                }
            }
            if (TypeDraw == TypeShape.Eraser)
            {
                isEraser = true;
                Point p = Mouse.GetPosition(MainCanvas);
                BitmapSource bmp = (BitmapSource)MainCanvasImage.Source;

                if (bmp == null) return;
                MyBitmap = new WriteableBitmap(bmp);
                MainCanvasImage.Source = MyBitmap;
                ImageProcess.Rubber((int)p.X, (int)p.Y, penWidth, MyBitmap);
            }
            if (TypeDraw == TypeShape.Fill)
            {
                System.Drawing.Color color = System.Drawing.Color.FromArgb(fillColor.A, fillColor.R, fillColor.G, fillColor.B);
                Point p = Mouse.GetPosition(MainCanvas);
                ImageProcess.FloodFill(MainCanvas, MainCanvasImage, p, color);
                SaveHistory();
                return;
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //MainCanvas.Height = this.ActualHeight;
            //MainCanvas.Width = this.ActualWidth;
        }

        #endregion

        #region Test
        private void btnUndo_Click(object sender, RoutedEventArgs e)
        {
            string file = history.Undo();
            if (MainCanvas.Height != 1028)
            {
                MainCanvas.Height = 1028;
                MainCanvas.Width = 2048;
            }
            else
            {
                MainCanvas.Height = 480;
                MainCanvas.Width = 720;
            }
            if (file != null)
                SetImage(file);
        }
        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < MainCanvas.Children.Count; ++i)
            {
                if (!(MainCanvas.Children[i] is ItemLayer)) continue;
                ItemLayer item = (ItemLayer)MainCanvas.Children[i];
                MainCanvas.Children.Remove((ItemLayer)MainCanvas.Children[i]);
            }
            MainCanvasImage.Source = null;
            FileSystem.ClearTemp();
            history.ClearTemp();
            history.ClearMain();
        }

        private void btnRedo_Click(object sender, RoutedEventArgs e)
        {
            string file = history.Redo();
            if (file != null)
                SetImage(file);
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            string ImagePath = FileSystem.OpenFile();
            if (ImagePath == "") return;
            SetImage(ImagePath);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            FileSystem.SaveFile(MainCanvas);
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog prnt = new PrintDialog();
            if (prnt.ShowDialog() == true)
            {
                {
                    prnt.PrintVisual(MainCanvas, "Printing Canvas");
                }
            }
        }

        private void btnReSize_Click(object sender, RoutedEventArgs e)
        {
            var ResizeWindow = new ResizeWindow(MainCanvas.Width, MainCanvas.Height);

            ResizeWindow.Owner = Application.Current.MainWindow;
            ResizeWindow.SizeUpdate += (s, args) => {
                MainCanvas.Width = args.Width;
                MainCanvas.Height = args.Height;
            };
            ResizeWindow.ShowInTaskbar = false;
            ResizeWindow.Show();
            ResizeWindow.Activate();
            ResizeWindow.Topmost = true;
            ResizeWindow.Topmost = false;
            ResizeWindow.Focus();
        }
        #endregion

        #region MenuEvent
        private void MenuItem_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (controlSelector.itemChoose != null)
            {
                MainCanvas.Children.Remove(controlSelector.itemChoose);
                //Canvas.SetZIndex(controlSelector.itemChoose, zIndexTop);
                //zIndexTop++;
            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            if (controlSelector.itemChoose != null)
            {
                Canvas.SetZIndex(controlSelector.itemChoose, zIndexBack);
                zIndexBack--;
            }
        }
        #endregion

        private void ShapeChecked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            int tag = -1;
            bool check = Int32.TryParse(radioButton.Tag as string, out tag);

            if (check)
            {
                TypeDraw = (TypeShape)tag;
                if (controlSelector != null && TypeDraw != TypeShape.None) controlSelector.itemChoose = null;

                switch (TypeDraw)
                {
                    case TypeShape.None:
                    case TypeShape.Line:
                    case TypeShape.PolyLine:
                        if (fillColorBtn != null) fillColorBtn.IsEnabled = false;
                        if (textToolsGroup != null) textToolsGroup.Visibility = Visibility.Collapsed;
                        break;
                    case TypeShape.Eraser:
                        if (fillColorBtn != null) fillColorBtn.IsEnabled = false;
                        if (outlineColorBtn != null) outlineColorBtn.IsEnabled = false;
                        if (textToolsGroup != null) textToolsGroup.Visibility = Visibility.Collapsed;
                        break;
                    case TypeShape.Fill:
                        if (outlineColorBtn != null) outlineColorBtn.IsEnabled = false;
                        if (fillColorBtn != null) fillColorBtn.IsEnabled = true;
                        if (textToolsGroup != null) textToolsGroup.Visibility = Visibility.Collapsed;
                        break;
                    case TypeShape.Text:
                        break;
                    default:
                        if (fillColorBtn != null) fillColorBtn.IsEnabled = true;
                        if (outlineColorBtn != null) outlineColorBtn.IsEnabled = true;
                        if (textToolsGroup != null) textToolsGroup.Visibility = Visibility.Collapsed;
                        break;
                }
            }
        }

        private void WidthChecked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            int tag = -1;
            bool check = Int32.TryParse(radioButton.Tag as string, out tag);

            if (check)
            {
                penWidth = tag;
            }
        }

        private void StyleChecked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            int tag = -1;
            bool check = Int32.TryParse(radioButton.Tag as string, out tag);

            if (check)
            {
                styleLine = (StyleLines)tag;
            }
        }


        private void Outline_ColorChanged(object sender, RoutedEventArgs e)
        {
            Fluent.ColorGallery colorGallery = sender as Fluent.ColorGallery;
            outlineColor = (Color)colorGallery.SelectedColor;
        }

        private void Fill_ColorChanged(object sender, RoutedEventArgs e)
        {
            Fluent.ColorGallery colorGallery = sender as Fluent.ColorGallery;
            fillColor = (Color)colorGallery.SelectedColor;
        }

        private void SetImage(string path)
        {
            if (path == "")
            {
                MainCanvasImage.Source = null;
                return;
            }
            BitmapImage temp = new BitmapImage();
            temp.BeginInit();
            temp.CacheOption = BitmapCacheOption.OnLoad;
            temp.UriSource = new Uri(path);
            temp.EndInit();

            MainCanvasImage.Width = MainCanvas.Width;
            MainCanvasImage.Height = MainCanvas.Height;
            MainCanvasImage.Source = temp;
        }

        private void SaveHistory()
        {
            string file = FileSystem.CanvasToFileTemp(MainCanvas);

            FileSystem.AddTemp(file);

            SetImage(file);

            history.PushBack(file);
            history.ClearTemp();

            MainCanvas.UpdateLayout();
        }

        public void Rubber(int x, int y, int RubberWidth, int rubberHeight, WriteableBitmap image)
        {
            var halfRubberWidth = RubberWidth / 2;
            var halfRubberHeight = rubberHeight / 2;
            if (x > (image.PixelWidth - halfRubberWidth))
            {
                x = (image.PixelWidth - halfRubberWidth);
            }
            if (y > (image.PixelHeight - halfRubberHeight))
            {
                y = (image.PixelHeight - halfRubberHeight);
            }
            if (x < halfRubberWidth)
            {
                x = halfRubberWidth;
            }
            if (y < halfRubberHeight)
            {
                y = halfRubberHeight;
            }


            int stride = image.PixelWidth * image.Format.BitsPerPixel / 8;
            int byteSize = stride * image.PixelHeight * image.Format.BitsPerPixel / 8;
            var ary = new byte[byteSize];
            image.CopyPixels(ary, stride, 0);

            var curix = ((y - 10) * stride) + ((x - 10) * 4);

            for (var iy = 0; iy < rubberHeight; iy++)
            {
                for (var ix = 0; ix < RubberWidth; ix++)
                    for (var b = 0; b < 4; b++)
                    {
                        ary[curix] = 0;
                        curix++;
                    }
                curix = curix + stride - (RubberWidth * 4);
            }

            image.WritePixels(new Int32Rect(0, 0, image.PixelWidth, image.PixelHeight), ary, stride, 0);
        }






        private void SaveCanvas()
        {

            MainCanvas.Background = System.Windows.Media.Brushes.Transparent;
            MainCanvas.UpdateLayout();
            Rect bounds = VisualTreeHelper.GetDescendantBounds(MainCanvas);
            double dpi = 96d;

            RenderTargetBitmap rtb = new RenderTargetBitmap((int)bounds.Width, (int)bounds.Height, dpi, dpi, System.Windows.Media.PixelFormats.Default);

            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(MainCanvas);
                dc.DrawRectangle(vb, null, new Rect(new System.Windows.Point(), bounds.Size));
            }
            rtb.Render(dv);
            BitmapEncoder pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create(rtb));

            try
            {
                System.IO.MemoryStream ms = new System.IO.MemoryStream();

                pngEncoder.Save(ms);

                BitmapImage temp = new BitmapImage();
                temp.BeginInit();
                temp.CacheOption = BitmapCacheOption.OnLoad;
                temp.StreamSource = ms;
                temp.EndInit();

                MainCanvasImage.Width = MainCanvas.Width;
                MainCanvasImage.Height = MainCanvas.Height;
                MainCanvasImage.Source = temp;

                ms.Close();
                ms.Dispose();

            }
            catch (Exception error)
            {
                MessageBox.Show("Lỗi SaveCanvas");
            }
            MainCanvas.Background = System.Windows.Media.Brushes.White;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            FileSystem.ClearTemp();
        }

        private void Text_ColorChanged(object sender, RoutedEventArgs e)
        {
            Fluent.ColorGallery colorGallery = sender as Fluent.ColorGallery;
            textColor = (Color)colorGallery.SelectedColor;
        }

        private void fontFamilySelected(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox != null)
            {
                FontFamily font = comboBox.SelectedItem as FontFamily;
                fontFamilyText = font;
            }
        }

        private void fontSizeSelected(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox != null)
            {
                string sizeStr = comboBox.SelectedItem as string;
                // Parse ve int
                int fontSize = 0;
                bool check = int.TryParse(sizeStr, out fontSize);
                if (check == true)
                {
                    fontSizeText = fontSize;
                }
            }
        }
    }
}
