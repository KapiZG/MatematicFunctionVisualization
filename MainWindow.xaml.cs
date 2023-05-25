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
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            LineGeometry myLineGeometry = new LineGeometry();
            myLineGeometry.StartPoint = new Point(0, 0);
            myLineGeometry.EndPoint = new Point(100, 130);

            Path myPath = new Path();
            myPath.Stroke = Brushes.Black;
            myPath.StrokeThickness = 1;
            myPath.Data = myLineGeometry;

            poleWykresu.Children.Add(myPath);

            LineGeometry myLineGeometry2 = new LineGeometry();
            myLineGeometry2.EndPoint = new Point(600, 300);
            myLineGeometry2.StartPoint = new Point(100, 130);

            Path myPath2 = new Path();
            myPath2.Stroke = Brushes.Black;
            myPath2.StrokeThickness = 1;
            myPath2.Data = myLineGeometry2;

            poleWykresu.Children.Add(myPath2);

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

        private void DrawLineFunction(object sender, EventArgs e)
        {
            Regex reg = new Regex("[^0-9]+");
            if (reg.IsMatch(linearFunctionA.Text) && reg.IsMatch(linearFunctionB.Text))
            {
                abc.Text = "Muszą być lidzby dzbanie";
                return;
            }
            int a = int.Parse(linearFunctionA.Text);
            int b = int.Parse(linearFunctionB.Text);
            abc.Text =  (a + b).ToString();
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

    }
}
