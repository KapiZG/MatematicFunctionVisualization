using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GeneratorWykresow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        private double frequency = 1;
        private int zoom = 1;
        private double functionScaleHeight;
        private double functionScaleWidth;
        private Brush gridColor;
        private Color axesColor;
        private bool isLiveGeneration;
        private bool isAutoCleaning;
        public MainWindow()
        {
            InitializeComponent();

            gridColor = Brushes.Black;
            axesColor = Colors.Black;

            DrawGrid();

        }

        private void DrawGrid()
        {
            functionScaleHeight = poleWykresu.Height;
            functionScaleWidth = poleWykresu.Width;

            poleWykresu.Children.Clear();

            List<LineGeometry> lines = new List<LineGeometry>();
            List<Path> paths = new List<Path>();

            for (int ifPlus = 1; ifPlus > -2; ifPlus -= 2)
            {
                for (int i = (int)(functionScaleWidth / 2); i < functionScaleWidth && i > 0; i += 10 * zoom * ifPlus)
                {
                    lines.Add(new LineGeometry());
                    lines.Last().StartPoint = new Point(i, 0);
                    lines.Last().EndPoint = new Point(i, functionScaleHeight);

                    paths.Add(new Path());
                    paths.Last().Stroke = gridColor;
                    paths.Last().StrokeThickness = 0.5;
                    paths.Last().Data = lines.Last();

                    poleWykresu.Children.Add(paths.Last());
                }

                for (int i = (int)(functionScaleHeight / 2); i < functionScaleHeight && i > 0; i += 10 * zoom * ifPlus)
                {
                    lines.Add(new LineGeometry());
                    lines.Last().StartPoint = new Point(0, i);
                    lines.Last().EndPoint = new Point(functionScaleWidth, i);

                    paths.Add(new Path());
                    paths.Last().Stroke = gridColor;
                    paths.Last().StrokeThickness = 0.5;
                    paths.Last().Data = lines.Last();

                    poleWykresu.Children.Add(paths.Last());
                }
            }

            DrawXYAxis();

            lines.Clear();
            paths.Clear();
        }

        private void DrawXYAxis()
        {
            LineGeometry midleX = new LineGeometry();
            LineGeometry midleY = new LineGeometry();

            midleX.StartPoint = new Point(functionScaleWidth / 2, 0);
            midleY.StartPoint = new Point(0, functionScaleHeight / 2);
            midleX.EndPoint = new Point(functionScaleWidth / 2, functionScaleHeight);
            midleY.EndPoint = new Point(functionScaleWidth, functionScaleHeight / 2);

            Path midleXPath = new Path();
            Path midleYPath = new Path();

            midleXPath.Stroke = new SolidColorBrush(axesColor);
            midleXPath.StrokeThickness = 1;
            midleXPath.Data = midleX;

            midleYPath.Stroke = new SolidColorBrush(axesColor);
            midleYPath.StrokeThickness = 1;
            midleYPath.Data = midleY;

            poleWykresu.Children.Add(midleYPath);
            poleWykresu.Children.Add(midleXPath);
        }


        private delegate double WzorMatematyczny(double x);
        private void ConvertGridValueToCanvaboxValue(WzorMatematyczny wzor)
        {
            //Objaśnienie tego czegoś jak działa: Kod działa na zasadzie obliczenia y, w skali gotowego wykresu funkcji, czytaj 
            // x przekształczamy z współrzędnych i wymiarów canva boxa do wartości jakiej wynosi dany x na osi X (np 0 w skali wielkości canva boxa to -21 na osi X)
            // potem obliczamy y przy użyciu wzoru na funkcję liniową
            // Teraz przekształcamy otrzymaną wartość spowrotem na współrzędne canva boxa (czyli np z -21 na 0)
            // Powtarzamy ten proces w kółko i w punkty od których ma rysować funkcję dajemy zmienne i (jako x) i y (jako y) (użyłem i zamiast x, ponieważ wartość x osi jest
            //inna niż współrzędne wartości canva boxa)
            if(isAutoCleaning) 
            {
                DrawGrid();
            }
            double y;
            double i = 0;
            double x = functionScaleWidth / 2 / (10 * zoom) * -1;
            List<LineGeometry> lines = new List<LineGeometry>();
            List<Path> paths = new List<Path>();

            while(x <= functionScaleWidth / 2 / (10 * zoom))
            {
                lines.Add(new LineGeometry());
                if (Double.IsNaN(wzor(x))) { i += 10d * (double)zoom / frequency; x += 1d / frequency; continue; };
                y = wzor(x);
                y = y * (10 * zoom);
                if (y > functionScaleHeight || y < -functionScaleHeight) { i += 10d * (double)zoom / frequency; x += 1d / frequency; continue; }
                if (y < 0) { y *= -1; y += functionScaleHeight / 2; }
                else if (y > 0) { y = functionScaleHeight / 2 - y; }
                else { y = functionScaleHeight / 2; }
                lines.Last().StartPoint = new Point(i, y);

                i += 10d * (double)zoom / frequency;
                x += 1d / frequency;
                if(Double.IsNaN(wzor(x))) { continue; };
                y = wzor(x);
                y = y * (10 * zoom);
                if (y > functionScaleHeight || y < -functionScaleHeight) { continue; }
                if (y < 0) { y *= -1; y += functionScaleHeight / 2; }
                else if (y > 0) { y = functionScaleHeight / 2 - y; }
                else { y = functionScaleHeight / 2; }
                lines.Last().EndPoint = new Point(i, y);

                paths.Add(new Path());
                paths.Last().Stroke = new SolidColorBrush(Colors.Blue);
                paths.Last().StrokeThickness = 1;
                paths.Last().Data = lines.Last();

                poleWykresu.Children.Add(paths.Last());
            }
        }

        private void DrawLineFunction(object sender, EventArgs e)
        {
            int a = (int)linearFunctionA.Value;
            int b = (int)linearFunctionB.Value;


            ConvertGridValueToCanvaboxValue(x => a * x + b);

        }

        private void DrawSquartFunction(object sender, EventArgs e)
        {
            int a = (int)squareFunctionA.Value;
            int b = (int)squareFunctionB.Value;
            int c = (int)squareFunctionC.Value;

            ConvertGridValueToCanvaboxValue(x => a * (x * x) + b * x + c);

        }

        private void DrawHomographicFunction(object sender, EventArgs e)
        {
            int a = (int)homographicFunctionA.Value;
            int b = (int)homographicFunctionB.Value;
            int c = (int)homographicFunctionC.Value;
            int d = (int)homographicFunctionD.Value;

            ConvertGridValueToCanvaboxValue(x => (a * x + b) / (c * x + d));
        }

        private void DrawExponentialFunction(object sender, EventArgs e)
        {
            double a = exponentialFunctionA.Value;

            ConvertGridValueToCanvaboxValue(x => Math.Pow(a, x));
        }

        private void DrawLogaritmicFunction(object sender, EventArgs e)
        {
            double a = logarithmicFunctionA.Value;
            ConvertGridValueToCanvaboxValue(x =>
            {
                if (Double.IsNaN(Math.Log(x, a))) { return double.NaN; }
                return Math.Log(x, a) ;
            });
        }

        private void OnSelectFunction(object sender, EventArgs e)
        {
            String optionSelected = (sender as ComboBox)?.Text ?? "error";
            lineFunctionEquation.Visibility = Visibility.Hidden;
            squareFunctionEquation.Visibility = Visibility.Hidden;
            homographicFunctionEquation.Visibility = Visibility.Hidden;
            exponentialFunctionEquation.Visibility = Visibility.Hidden;
            logarithmicFunctionEquation.Visibility = Visibility.Hidden;

            generateButton.Click -= new RoutedEventHandler(DrawLineFunction);
            generateButton.Click -= new RoutedEventHandler(DrawSquartFunction);
            generateButton.Click -= new RoutedEventHandler(DrawHomographicFunction);
            generateButton.Click -= new RoutedEventHandler(DrawExponentialFunction);
            generateButton.Click -= new RoutedEventHandler(DrawLogaritmicFunction);

            switch (optionSelected)
            {
                case "Funkcja Liniowa":
                    lineFunctionEquation.Visibility = Visibility.Visible;
                    generateButton.Click += new RoutedEventHandler(DrawLineFunction);
                    break;
                case "Funkcja Kwadratowa":
                    squareFunctionEquation.Visibility = Visibility.Visible;
                    generateButton.Click += new RoutedEventHandler(DrawSquartFunction);
                    break;
                case "Funkcja Homograficzna":
                    homographicFunctionEquation.Visibility = Visibility.Visible;
                    generateButton.Click += new RoutedEventHandler(DrawHomographicFunction);
                    break;
                case "Funkcja Wykładnicza":
                    exponentialFunctionEquation.Visibility = Visibility.Visible;
                    generateButton.Click += new RoutedEventHandler(DrawExponentialFunction);
                    break;
                case "Funkcja Logarytmiczna":
                    logarithmicFunctionEquation.Visibility = Visibility.Visible;
                    generateButton.Click += new RoutedEventHandler(DrawLogaritmicFunction);
                    break;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            String a = scaleTextBox.Text;
            Regex reg = new Regex("^[0-9]+");
            if(reg.IsMatch(a))
            {
                zoom = int.Parse(a);
                DrawGrid();
            }
        }

        private void LinearAValueChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            linearFunctionAValue.Content = (sender as Slider)?.Value ?? 0.0;
            LiveGeneration(DrawLineFunction);
        }

        private void LinearBValueChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            linearFunctionBValue.Content = (sender as Slider)?.Value ?? 0.0;
            LiveGeneration(DrawLineFunction);
        }
        private void SquareAValueChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            squareFunctionAValue.Content = (sender as Slider)?.Value ?? 0.0;
            LiveGeneration(DrawSquartFunction);
        }

        private void SquareBValueChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            squareFunctionBValue.Content = (sender as Slider)?.Value ?? 0.0;
            LiveGeneration(DrawSquartFunction);
        }

        private void SquareCValueChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            squareFunctionCValue.Content = (sender as Slider)?.Value ?? 0.0;
            LiveGeneration(DrawSquartFunction);
        }

        private void HomographicAValueChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            homographicFunctionAValue.Content = (sender as Slider)?.Value ?? 0.0;
            LiveGeneration(DrawHomographicFunction);
        }
        private void HomographicBValueChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            homographicFunctionBValue.Content = (sender as Slider)?.Value ?? 0.0;
            LiveGeneration(DrawHomographicFunction);
        }
        private void HomographicCValueChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            homographicFunctionCValue.Content = (sender as Slider)?.Value ?? 0.0;
            LiveGeneration(DrawHomographicFunction);
        }
        private void HomographicDValueChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            homographicFunctionDValue.Content = (sender as Slider)?.Value ?? 0.0;
            LiveGeneration(DrawHomographicFunction);
        }

        private void ExponentialAValueChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            exponentialFunctionAValue.Content = (sender as Slider)?.Value ?? 0.0;
            LiveGeneration(DrawExponentialFunction);
        }

        private void LogarithmicAValueChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            logarithmicFunctionAValue.Content = (sender as Slider)?.Value ?? 0.0;
            LiveGeneration(DrawLogaritmicFunction);
        }

        private delegate void Draw(object sender, EventArgs e);
        private void LiveGeneration(Draw function) {
            if (isLiveGeneration) function(null, null); 
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            String a = (sender as TextBox)!.Text;
            Regex reg = new Regex("^[0-9]+");
            if (reg.IsMatch(a))
            {
                frequency = int.Parse(a);
            }
        }

        private void ChangeResolution(object sender, EventArgs e)
        {
            string resolution = (sender as ComboBox)?.Text ?? "Error";

            MatchCollection match = Regex.Matches(resolution, "[0-9]+");
            double width = double.Parse(match[0].Value);
            double height = double.Parse(match[1].Value);

            Width = width;
            Height = height;

            poleWykresu.Width = width / 2;
            poleWykresu.Height = height / 1.125;
            canvaBoxBorder.Width = width / 2 + 20;
            canvaBoxBorder.Height = Height / 1.125 + 20;
            DrawGrid();
        }

        private void ChangeTheme(object sender, EventArgs e) 
        {
            string theme = (sender as ComboBox)?.Text ?? "Error";
            Brush textBlockColor = Brushes.White;

            switch (theme)
            {
                case "jasny":
                    this.Background = Brushes.White;
                    textBlockColor = Brushes.Black;
                    gridColor = Brushes.Black;
                    axesColor = Colors.Black;
                    canvaBoxBorder.BorderBrush = Brushes.Black;
                    poleWykresu.Background = Brushes.White;
                    break;
                case "ciemny":
                    this.Background = Brushes.Black;
                    textBlockColor = Brushes.White;
                    gridColor = Brushes.White;
                    axesColor = Colors.White;
                    canvaBoxBorder.BorderBrush = Brushes.White;
                    poleWykresu.Background = Brushes.Black;
                    break;
                case "ślipek?":
                    /*ImageBrush a = new ImageBrush();
                    a.ImageSource = new BitmapImage(new Uri(@"PanProfesorKacper.png", UriKind.Relative));
                    this.Background = a;*/
                    break;
            }

            DrawGrid();

            foreach (TextBlock b in bcs.Children.OfType<TextBlock>())
            {
                b.Foreground = textBlockColor;
            }

            foreach (CheckBox b in bcs.Children.OfType<CheckBox>())
            {
                b.Foreground = textBlockColor;
            }

            foreach (StackPanel b in bcs.Children.OfType<StackPanel>())
            {
                b.Children.OfType<Label>().First().Foreground = textBlockColor;
                foreach (StackPanel c in b.Children.OfType<StackPanel>())
                {
                    c.Children.OfType<Label>().First().Foreground = textBlockColor;
                    c.Children.OfType<Label>().Last().Foreground = textBlockColor;
                }
            }
        }

        private void SetLiveGeneration(object sender, EventArgs e)
        {
            if ((sender as CheckBox)?.IsChecked ?? false)
            {
                isLiveGeneration = true;
            } 
            else
            {
                isLiveGeneration = false;
            }
        }

        private void SetAutoCleaning(object sender, EventArgs e)
        {
            if((sender as CheckBox)?.IsChecked ?? false)
            {
                isAutoCleaning= true;
            }
            else
            {
                isAutoCleaning = false;
            }
        }
    }
}
