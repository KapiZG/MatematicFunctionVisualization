using System;
using System.Collections.Generic;
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
        public MainWindow()
        {
            InitializeComponent();

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
                    paths.Last().Stroke = Brushes.Black;
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
                    paths.Last().Stroke = Brushes.Black;
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

            midleXPath.Stroke = Brushes.Red;
            midleXPath.StrokeThickness = 0.5;
            midleXPath.Data = midleX;

            midleYPath.Stroke = Brushes.Red;
            midleYPath.StrokeThickness = 0.5;
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

            double y;
            double i = 0;
            double x = functionScaleWidth / 2 / (10 * zoom) * -1;
            List<LineGeometry> lines = new List<LineGeometry>();
            List<Path> paths = new List<Path>();

            while(x <= functionScaleWidth / 2 / (10 * zoom))
            {
                lines.Add(new LineGeometry());
                if (wzor(x) == -0.0) { i += 10d * (double)zoom / frequency; x += 1d / frequency; continue; };
                y = wzor(x);
                y = y * (10 * zoom);
                if (y > functionScaleHeight || y < -functionScaleHeight) { i += 10d * (double)zoom / frequency; x += 1d / frequency; continue; }
                if (y < 0) { y *= -1; y += functionScaleHeight / 2; }
                else if (y > 0) { y = functionScaleHeight / 2 - y; }
                else { y = functionScaleHeight / 2; }
                lines.Last().StartPoint = new Point(i, y);

                i += 10d * (double)zoom / frequency;
                x += 1d / frequency;
                if(wzor(x) == -0.0) { continue; };
                y = wzor(x);
                y = y * (10 * zoom);
                if (y > functionScaleHeight || y < -functionScaleHeight) { continue; }
                if (y < 0) { y *= -1; y += functionScaleHeight / 2; }
                else if (y > 0) { y = functionScaleHeight / 2 - y; }
                else { y = functionScaleHeight / 2; }
                lines.Last().EndPoint = new Point(i, y);

                paths.Add(new Path());
                paths.Last().Stroke = Brushes.Blue;
                paths.Last().StrokeThickness = 0.5;
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
                if (Double.IsNaN(Math.Log(x, a))) { return -0.0; }
                return Math.Log(x, a) ;
            });
        }

        private void OnSelectFunction(object sender, EventArgs e)
        {
            String optionSelected = (sender as ComboBox)?.Text ?? "error";
            abc.Text = optionSelected;
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
        }

        private void LinearBValueChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            linearFunctionBValue.Content = (sender as Slider)?.Value ?? 0.0;
        }
        private void SquareAValueChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            squareFunctionAValue.Content = (sender as Slider)?.Value ?? 0.0;
        }

        private void SquareBValueChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            squareFunctionBValue.Content = (sender as Slider)?.Value ?? 0.0;
        }

        private void SquareCValueChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            squareFunctionCValue.Content = (sender as Slider)?.Value ?? 0.0;
        }

        private void HomographicAValueChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            homographicFunctionAValue.Content = (sender as Slider)?.Value ?? 0.0;
        }
        private void HomographicBValueChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            homographicFunctionBValue.Content = (sender as Slider)?.Value ?? 0.0;
        }
        private void HomographicCValueChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            homographicFunctionCValue.Content = (sender as Slider)?.Value ?? 0.0;
        }
        private void HomographicDValueChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            homographicFunctionDValue.Content = (sender as Slider)?.Value ?? 0.0;
        }

        private void ExponentialAValueChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            exponentialFunctionAValue.Content = (sender as Slider)?.Value ?? 0.0;
        }

        private void LogarithmicAValueChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            logarithmicFunctionAValue.Content = (sender as Slider)?.Value ?? 0.0;
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
            DrawGrid();
        }
    }
}
