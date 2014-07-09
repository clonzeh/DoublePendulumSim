using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DoublePendulumSim
{
    // original code: https://github.com/micaeloliveira/physics-sandbox/blob/feature/new-styling/assets/javascripts/pendulum.js
    public class Pendulum
    {
        //Physics Constants
        private double d2Phi1 = 0;
        private double d2Phi2 = 0;
        private double dPhi1  = 0;
        private double dPhi2  = 0;
        public double Phi1   = 0*(Math.PI)/2;
        public double Phi2   = 2.3*(Math.PI)/2;
        public double m1     = 10;
        public double m2     = 10;
        private double l1     = 150;
        private double l2     = 150;
        private double X0     = 350;
        private double Y0     = 60;
        private double g      = 9.8;
        private double time   = 0.05;

        public Line myLine1;
        public Line myLine2;
        public Ellipse myCircle1;
        public Ellipse myCircle2;

        public Pendulum(Canvas canvas, double m1, double m2, double Phi1, double Phi2)
        {
            this.m1 = m1;
            this.m2 = m2;
            this.Phi1 = Phi1;
            this.Phi2 = Phi2;
            Create(canvas);
        }

        public void Create(Canvas canvas)
        {
            myLine1 = new Line { X1 = X0, Y1 = Y0, X2 = 0, Y2 = 0, StrokeThickness = 5, Stroke = Brushes.Red };
            myLine2 = new Line { X1 = 0, Y1 = 0, X2 = 0, Y2 = 0, StrokeThickness = 5, Stroke = Brushes.Red };
            myCircle1 = new Ellipse { Width = 2*m1, Height = 2*m1, Fill = Brushes.Black };
            myCircle2 = new Ellipse { Width = 2*m2, Height = 2*m2, Fill = Brushes.Black };
            Canvas.SetLeft(myCircle1, X0+l1*Math.Sin(Phi1) - m1);
            Canvas.SetTop(myCircle1, Y0+l1*Math.Cos(Phi1) - m1);
            Canvas.SetLeft(myCircle2, X0+l1*Math.Sin(Phi1)+l2*Math.Sin(Phi2) - m2);
            Canvas.SetTop(myCircle2, Y0+l1*Math.Cos(Phi1)+l2*Math.Cos(Phi2) - m2);
            canvas.Children.Add(myLine1);
            canvas.Children.Add(myLine2);
            canvas.Children.Add(myCircle1);
            canvas.Children.Add(myCircle2);
        }

        public void Animate() 
        {
              double mu = 1+m1/m2;
              d2Phi1  = (g*(Math.Sin(Phi2)*Math.Cos(Phi1-Phi2)-mu*Math.Sin(Phi1))-(l2*dPhi2*dPhi2+l1*dPhi1*dPhi1*Math.Cos(Phi1-Phi2))*Math.Sin(Phi1-Phi2))/(l1*(mu-Math.Cos(Phi1-Phi2)*Math.Cos(Phi1-Phi2)));
              d2Phi2  = (mu*g*(Math.Sin(Phi1)*Math.Cos(Phi1-Phi2)-Math.Sin(Phi2))+(mu*l1*dPhi1*dPhi1+l2*dPhi2*dPhi2*Math.Cos(Phi1-Phi2))*Math.Sin(Phi1-Phi2))/(l2*(mu-Math.Cos(Phi1-Phi2)*Math.Cos(Phi1-Phi2)));
              dPhi1   += d2Phi1*time;
              dPhi2   += d2Phi2*time;
              Phi1    += dPhi1*time;
              Phi2    += dPhi2*time;

              double myCircle1x = X0+l1*Math.Sin(Phi1);
              double myCircle1y = Y0+l1*Math.Cos(Phi1);
              double myCircle2x = X0+l1*Math.Sin(Phi1)+l2*Math.Sin(Phi2);
              double myCircle2y = Y0+l1*Math.Cos(Phi1)+l2*Math.Cos(Phi2);

              Canvas.SetLeft(myCircle1, myCircle1x - m1);
              Canvas.SetTop(myCircle1, myCircle1y - m1);
              Canvas.SetLeft(myCircle2, myCircle2x - m2);
              Canvas.SetTop(myCircle2, myCircle2y - m2);

              myLine1.X2  = myCircle1x;
              myLine1.Y2  = myCircle1y;
              myLine2.X1 = myCircle1x;
              myLine2.Y1 = myCircle1y;
              myLine2.X2  = myCircle2x;
              myLine2.Y2  = myCircle2y;
        }
    }

    public partial class MainWindow : Window
    {
        private System.Threading.Timer timer;

        public MainWindow()
        {
            InitializeComponent();
            var pendulum = new Pendulum(this.myCanvas, m1: 10, m2: 10, Phi1: 0*(Math.PI)/2, Phi2: 2.3*(Math.PI)/2);
            this.Loaded += (sender, e) =>  this.timer = new System.Threading.Timer((state) => Dispatcher.Invoke(() => pendulum.Animate()), null, TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(5));
            this.Closed += (sender , e) => timer.Dispose();
        }
    }
}
