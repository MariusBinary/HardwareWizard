using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace HardwareWizard.Controls
{
    public class CircularProgressShape : Shape
    {
        static CircularProgressShape()
        {
            StrokeProperty.OverrideMetadata(typeof(CircularProgressShape), 
                new FrameworkPropertyMetadata(Brushes.White));

            FillProperty.OverrideMetadata(typeof(CircularProgressShape),
                new FrameworkPropertyMetadata(Brushes.Transparent));

            StrokeThicknessProperty.OverrideMetadata(typeof(CircularProgressShape),
                new FrameworkPropertyMetadata(8.0));
        }

        // Start Value
        public double StartValue
        {
            get { return (double)GetValue(StartValueProperty); }
            set { SetValue(StartValueProperty, value); }
        }

        private static FrameworkPropertyMetadata startValueMetadata =
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender,
                    null, new CoerceValueCallback(CoerceValue));

        public static readonly DependencyProperty StartValueProperty =
            DependencyProperty.Register("StartValue", typeof(double), typeof(CircularProgressShape), startValueMetadata);

        // End Angle
        public double EndValue
        {
            get { return (double)GetValue(EndValueProperty); }
            set { SetValue(EndValueProperty, value); }
        }

        private static FrameworkPropertyMetadata endValueMetadata =
                new FrameworkPropertyMetadata(0.0,FrameworkPropertyMetadataOptions.AffectsRender,
                    null, new CoerceValueCallback(CoerceValue));

        public static readonly DependencyProperty EndValueProperty =
            DependencyProperty.Register("EndValue", typeof(double), typeof(CircularProgressShape), endValueMetadata);

        private static object CoerceValue(DependencyObject depObj, object baseVal)
        {
            double val = (double)baseVal;
            val = Math.Min(val, 99.999);
            val = Math.Max(val, 0.0);
            return val;
        }

        protected override Geometry DefiningGeometry
        {
            get
            {
                double startAngle = 90.0 - ((StartValue / 100.0) * 360.0); ;
                double endAngle = startAngle - ((EndValue / 100.0) * 360.0);
                
                double maxWidth = Math.Max(0.0, RenderSize.Width - StrokeThickness);
                double maxHeight = Math.Max(0.0, RenderSize.Height - StrokeThickness);

                double xStart = maxWidth / 2.0 * Math.Cos(startAngle * Math.PI / 180.0);
                double yStart = maxHeight / 2.0 * Math.Sin(startAngle * Math.PI / 180.0);

                double xEnd = maxWidth / 2.0 * Math.Cos(endAngle * Math.PI / 180.0);
                double yEnd = maxHeight / 2.0 * Math.Sin(endAngle * Math.PI / 180.0);

                StreamGeometry geom = new StreamGeometry();
                using (StreamGeometryContext ctx = geom.Open())
                {
                    ctx.BeginFigure(new Point((RenderSize.Width / 2.0) + xStart,
                        (RenderSize.Height / 2.0) - yStart), true, false);
                    ctx.ArcTo(new Point((RenderSize.Width / 2.0) + xEnd, 
                        (RenderSize.Height / 2.0) - yEnd), new Size(maxWidth / 2.0, maxHeight / 2.0),
                        0.0, (startAngle - endAngle) > 180, SweepDirection.Clockwise, true, true);
                }

                return geom;
            }
        }
    }
}
