using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DoublePendulumSim
{
    public class Pendulum
    {
        //Physics Constants
        private double d2Phi1 = 0;
        private double d2Phi2 = 0;
        private double dPhi1  = 0;
        private double dPhi2  = 0;
        public double Phi1    = 0*(Math.PI)/2;
        public double Phi2    = 2.3*(Math.PI)/2;
        public double m1      = 10;
        public double m2      = 10;
        private double l1     = 150;
        private double l2     = 150;
        private double X0     = 350;
        private double Y0     = 60;
        private double g      = 9.8;
        private double time   = 0.05;

        private Line myLine1 = new Line { StrokeThickness = 5, Stroke = Brushes.Red };
        private Line myLine2 = new Line { StrokeThickness = 5, Stroke = Brushes.Red };
        private Ellipse myCircle1 = new Ellipse { Fill = Brushes.Black };
        private Ellipse myCircle2 = new Ellipse { Fill = Brushes.Black };

        public Pendulum(Canvas canvas)
        {
            canvas.Children.Add(myLine1);
            canvas.Children.Add(myLine2);
            canvas.Children.Add(myCircle1);
            canvas.Children.Add(myCircle2);
        }

        public void Update()
        {
            myCircle1.Width = 2*m1;
            myCircle1.Height = 2*m1;
            myCircle2.Width = 2*m2;
            myCircle2.Height = 2*m2;
            double myCircle1x = X0+l1*Math.Sin(Phi1);
            double myCircle1y = Y0+l1*Math.Cos(Phi1);
            double myCircle2x = X0+l1*Math.Sin(Phi1)+l2*Math.Sin(Phi2);
            double myCircle2y = Y0+l1*Math.Cos(Phi1)+l2*Math.Cos(Phi2);
            Canvas.SetLeft(myCircle1, myCircle1x - m1);
            Canvas.SetTop(myCircle1, myCircle1y - m1);
            Canvas.SetLeft(myCircle2, myCircle2x - m2);
            Canvas.SetTop(myCircle2, myCircle2y - m2);
            myLine1.X1 = X0;
            myLine1.Y1 = Y0;
            myLine1.X2 = myCircle1x;
            myLine1.Y2 = myCircle1y;
            myLine2.X1 = myCircle1x;
            myLine2.Y1 = myCircle1y;
            myLine2.X2 = myCircle2x;
            myLine2.Y2 = myCircle2y;
        }

        public void Animate()
        {
            lock(this)
            {
                double mu = 1+m1/m2;
                d2Phi1    = (g*(Math.Sin(Phi2)*Math.Cos(Phi1-Phi2)-mu*Math.Sin(Phi1))-(l2*dPhi2*dPhi2+l1*dPhi1*dPhi1*Math.Cos(Phi1-Phi2))*Math.Sin(Phi1-Phi2))/(l1*(mu-Math.Cos(Phi1-Phi2)*Math.Cos(Phi1-Phi2)));
                d2Phi2    = (mu*g*(Math.Sin(Phi1)*Math.Cos(Phi1-Phi2)-Math.Sin(Phi2))+(mu*l1*dPhi1*dPhi1+l2*dPhi2*dPhi2*Math.Cos(Phi1-Phi2))*Math.Sin(Phi1-Phi2))/(l2*(mu-Math.Cos(Phi1-Phi2)*Math.Cos(Phi1-Phi2)));
                dPhi1     += d2Phi1*time;
                dPhi2     += d2Phi2*time;
                Phi1      += dPhi1*time;
                Phi2      += dPhi2*time;
                Update();
            }
        }
    }
}
