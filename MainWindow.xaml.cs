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
            LineGeometry midleX = new LineGeometry();
            LineGeometry midleY = new LineGeometry();

            midleX.StartPoint = new Point(functionScaleHeight / 2, 0);
            midleY.StartPoint = new Point(0, functionScaleWidth / 2);
            midleX.EndPoint = new Point(functionScaleHeight / 2, functionScaleWidth);
            midleY.EndPoint = new Point(functionScaleHeight, functionScaleWidth /2);

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

            lines.Clear();
            paths.Clear();
        }

        public void OnGenerateClick()
        {

        }
        private void DrawMatematicFunction(Point startPoint, int x, int y)
        {

        }
        public void GetYWalue() 
        {
            

        }

        private void DrawQuadraticFunction(object sender, EventArgs e)
        {

        }

        private void DrawLineFunction(object sender, EventArgs e)
        {
            Regex reg = new Regex("^[0-9]+");
            String aString = linearFunctionA.Text;
            String bString = linearFunctionB.Text;
            int a = 1;
            int b = 0;
            double y, i;
           
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


            i = 0;
            List<LineGeometry> lines = new List<LineGeometry>();
            List<Path> paths = new List<Path>();

            for (double x = functionScaleWidth / 2 / (10 * zoom) * -1; x <= functionScaleWidth / 2 / (10 * zoom);)
            {
                lines.Add(new LineGeometry());

                y = a * x + b;
                y = y * (10 * zoom);
                if(y > functionScaleHeight || y < -functionScaleHeight) { i += 10 * zoom; x++; continue; }
                if(y < 0) { y *= -1; y += 210; }
                else if(y > 0) { y = 210 - y; }
                else { y = 210; }
                lines.Last().StartPoint = new Point(i, y);

                i += 10 * zoom;
                x++;
                y = a * x + b;
                y = y * (10 * zoom);
                if (y > functionScaleHeight || y < -functionScaleHeight) { continue; }
                if (y < 0) { y *= -1; y += 210; }
                else if (y > 0) { y = 210 - y; }
                else { y = 210; }
                lines.Last().EndPoint = new Point(i, y);

                paths.Add(new Path());
                paths.Last().Stroke = Brushes.Blue;
                paths.Last().StrokeThickness = 0.5;
                paths.Last().Data = lines.Last();

                poleWykresu.Children.Add(paths.Last());
            }
        }

        private void OnSelectFunction(object sender, EventArgs e)
        {
            String optionSelected = (sender as ComboBox)?.Text ?? "error";
            abc.Text = optionSelected;
            switch (optionSelected)
            {
                case "Funkcja Liniowa":
                    lineFunctioneEquation.Visibility = Visibility;
                    generateButton.Click += new RoutedEventHandler(DrawLineFunction);
                    break;
                case "Funkcja Kwadratowa":
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
