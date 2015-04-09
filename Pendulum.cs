// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
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

        private Canvas _canvas;
        private InkCanvas _ink;
        public StylusPointCollection col;
        public Stroke stroke;

        public Pendulum(Canvas canvas, InkCanvas ink)
        {
            _canvas = canvas;
            _ink = ink;

            canvas.Children.Add(myLine1);
            canvas.Children.Add(myLine2);
            canvas.Children.Add(myCircle1);
            canvas.Children.Add(myCircle2);
        }

        private void SetSize(FrameworkElement element, double size)
        {
            element.Width = size;
            element.Height = size;
        }

        public void SetPosition(UIElement element, double x, double y)
        {
            Canvas.SetLeft(element, x);
            Canvas.SetTop(element, y);
        }

        public void SetPosition(Line line, double x1, double y1, double x2, double y2)
        {
            line.X1 = x1;
            line.Y1 = y1;
            line.X2 = x2;
            line.Y2 = y2;
        }

        public void Update()
        {
            double myCircle1x = X0 + l1 * Math.Sin(Phi1);
            double myCircle1y = Y0 + l1 * Math.Cos(Phi1);
            double myCircle2x = myCircle1x + l2 * Math.Sin(Phi2);
            double myCircle2y = myCircle1y + l2 * Math.Cos(Phi2);
            SetSize(myCircle1, 2 * m1);
            SetSize(myCircle2, 2 * m2);
            SetPosition(myCircle1, myCircle1x - m1, myCircle1y - m1);
            SetPosition(myCircle2, myCircle2x - m2, myCircle2y - m2);
            SetPosition(myLine1, X0, Y0, myCircle1x, myCircle1y);
            SetPosition(myLine2, myCircle1x, myCircle1y, myCircle2x, myCircle2y);

            var sp = new StylusPoint(myCircle2x, myCircle2y);
            if (col == null || col.Count >= 1000)
            {
                var ncol = new StylusPointCollection();
                if (col != null)
                    ncol.Add(col.Last());
                ncol.Add(sp);
                stroke = new Stroke(ncol); 
                stroke.DrawingAttributes.Color = Colors.Gray;
                _ink.Strokes.Add(stroke);
                col = ncol;
            }
            else
            {
                col.Add(sp);
            }
        }

        public void Animate()
        {
            lock(this)
            {
                double mu = 1+m1/m2;
                d2Phi1 = Calc_d2Phi1(mu);
                d2Phi2 = Calc_d2Phi2(mu);
                dPhi1 += d2Phi1*time;
                dPhi2 += d2Phi2*time;
                Phi1  += dPhi1*time;
                Phi2  += dPhi2*time;
                Update();
            }
        }

        private double Calc_d2Phi1(double mu)
        {
            return 
                (
                    g 
                    * (Math.Sin(Phi2) * Math.Cos(Phi1 - Phi2) - mu * Math.Sin(Phi1)) 
                    - (l2 * dPhi2 * dPhi2 + l1 * dPhi1 * dPhi1 * Math.Cos(Phi1 - Phi2)) 
                    * Math.Sin(Phi1 - Phi2)
                ) 
                / 
                (
                    l1 
                    * (mu - Math.Cos(Phi1 - Phi2) * Math.Cos(Phi1 - Phi2))
                );
        }

        private double Calc_d2Phi2(double mu)
        {
            return 
                (   
                    mu 
                    * g 
                    * (Math.Sin(Phi1) * Math.Cos(Phi1 - Phi2) - Math.Sin(Phi2)) 
                    + (mu * l1 * dPhi1 * dPhi1 + l2 * dPhi2 * dPhi2 * Math.Cos(Phi1 - Phi2)) 
                    * Math.Sin(Phi1 - Phi2)
                ) 
                / 
                (
                    l2 
                    * (mu - Math.Cos(Phi1 - Phi2) * Math.Cos(Phi1 - Phi2))
                );
        }
    }
}
