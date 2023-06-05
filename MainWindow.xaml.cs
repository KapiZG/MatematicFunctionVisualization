using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace GeneratorWykresow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        private int scale = 1;
        private int zoom = 1;
        private double functionScaleHeight;
        private double functionScaleWidth;
        public MainWindow()
        {
            InitializeComponent();

            functionScaleHeight = poleWykresu.Height;
            functionScaleWidth = poleWykresu.Width;

            DrawGrid();

/*            LineGeometry myLineGeometry2 = new LineGeometry();
            myLineGeometry2.EndPoint = new Point(600, 300);
            myLineGeometry2.StartPoint = new Point(100, 130);

            Path myPath2 = new Path();
            myPath2.Stroke = Brushes.Black;
            myPath2.StrokeThickness = 1;
            myPath2.Data = myLineGeometry2;

            poleWykresu.Children.Add(myPath2);*/

        }

        private void DrawGrid()
        {
            poleWykresu.Children.Clear();

            List<LineGeometry> lines = new List<LineGeometry>();
            List<Path> paths = new List<Path>();

            for (int ifPlus = 1; ifPlus > -2; ifPlus -= 2)
            {
                for (int i = (int)(functionScaleHeight / 2); i < functionScaleHeight && i > 0; i += 10 * zoom * ifPlus)
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

                for (int i = (int)(functionScaleWidth / 2); i < functionScaleWidth && i > 0; i += 10 * zoom * ifPlus)
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

                y = wzor(x);
                y = y * (10 * zoom);
                if (y > functionScaleHeight || y < -functionScaleHeight) { i += 10 * zoom; x++; continue; }
                if (y < 0) { y *= -1; y += functionScaleHeight / 2; }
                else if (y > 0) { y = functionScaleHeight / 2 - y; }
                else { y = functionScaleHeight / 2; }
                lines.Last().StartPoint = new Point(i, y);

                i += 10 * zoom;
                x++;
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
            Regex reg = new Regex("^[0-9]+");
            String aString = linearFunctionA.Text;
            String bString = linearFunctionB.Text;
            int a = 1;
            int b = 0;
           
            if (!reg.IsMatch(aString) || !reg.IsMatch(bString))
            {
                abc.Text = "Muszą być lidzby dzbanie";
                return;
            }

            if(!String.IsNullOrEmpty(aString))
            {
                a = int.Parse(aString);             
            }

            if(!String.IsNullOrEmpty(bString))
            {
                b = int.Parse(bString);
            }

            ConvertGridValueToCanvaboxValue(x => a * x + b);

        }

        private void DrawSquartFunction(object sender, EventArgs e)
        {
            Regex reg = new Regex("^[0-9]+");
            String aString = squareFunctionA.Text;
            String bString = squareFunctionB.Text;
            String cString = squareFunctionB.Text;
            int a = 1;
            int b = 0;
            int c = 0;

            if (!reg.IsMatch(aString) || !reg.IsMatch(bString) || !reg.IsMatch(cString))
            {
                abc.Text = "Muszą być lidzby dzbanie";
                return;
            }

            if (!String.IsNullOrEmpty(aString))
            {
                a = int.Parse(aString);
            }

            if (!String.IsNullOrEmpty(bString))
            {
                b = int.Parse(bString);
            }

            if (!String.IsNullOrEmpty(cString))
            {
                c = int.Parse(cString);
            }

            ConvertGridValueToCanvaboxValue(x => a * x * x + b * x + c);

        }

        private void OnSelectFunction(object sender, EventArgs e)
        {
            String optionSelected = (sender as ComboBox)?.Text ?? "error";
            abc.Text = optionSelected;
            lineFunctioneEquation.Visibility = Visibility.Hidden;
            squareFunctioneEquation.Visibility = Visibility.Hidden;
            switch (optionSelected)
            {
                case "Funkcja Liniowa":
                    lineFunctioneEquation.Visibility = Visibility.Visible;
                    generateButton.Click += new RoutedEventHandler(DrawLineFunction);
                    break;
                case "Funkcja Kwadratowa":
                    squareFunctioneEquation.Visibility = Visibility.Visible;
                    generateButton.Click += new RoutedEventHandler(DrawSquartFunction);
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
    }
}
