using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using HardwareWizard.Core;

namespace HardwareWizard.Controls
{
    [TemplatePart(Name = "PART_Title", Type = typeof(TextBlock))]
    [TemplatePart(Name = "PART_Container", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_Canvas", Type = typeof(Canvas))]
    [TemplatePart(Name = "PART_Mask", Type = typeof(Rectangle))]
    [TemplatePart(Name = "PART_LeftDetails", Type = typeof(TextBlock))]
    [TemplatePart(Name = "PART_RightDetails", Type = typeof(TextBlock))]
    public class GraphControl : Control
    {
        #region Properties
        // Property: Fill
        public static readonly DependencyProperty FillProperty
            = DependencyProperty.Register("Fill", typeof(SolidColorBrush), typeof(GraphControl),
                new FrameworkPropertyMetadata(Brushes.White));

        public SolidColorBrush Fill
        {
            get { return (SolidColorBrush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        // Property: FixedXPixels
        public static readonly DependencyProperty FixedXPixelsProperty
            = DependencyProperty.Register("FixedXPixels", typeof(int), typeof(GraphControl),
                new FrameworkPropertyMetadata(-1));

        public int FixedXPixels
        {
            get { return (int)GetValue(FixedXPixelsProperty); }
            set { SetValue(FixedXPixelsProperty, value); }
        }

        // Property: FixedYPixels
        public static readonly DependencyProperty FixedYPixelsProperty =
            DependencyProperty.Register("FixedYPixels", typeof(int), typeof(GraphControl),
                new FrameworkPropertyMetadata(-1));

        public int FixedYPixels
        {
            get { return (int)GetValue(FixedYPixelsProperty); }
            set { SetValue(FixedYPixelsProperty, value); }
        }

        // Property: Title
        public static readonly DependencyProperty TitleProperty
            = DependencyProperty.Register("Title", typeof(string), typeof(GraphControl),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnTitleChanged)));

        private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as GraphControl;
            if (!control.isLoaded) return;
            control.SetDetailsVisibility(control.m_title, e.NewValue.ToString());
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Property: LeftDetails
        public static readonly DependencyProperty LeftDetailsProperty
            = DependencyProperty.Register("LeftDetails", typeof(string), typeof(GraphControl),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnLeftDetailsChanged)));

        private static void OnLeftDetailsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as GraphControl;
            if (!control.isLoaded) return;
            control.SetDetailsVisibility(control.m_leftDetails, e.NewValue.ToString());
        }

        public string LeftDetails
        {
            get { return (string)GetValue(LeftDetailsProperty); }
            set { SetValue(LeftDetailsProperty, value); }
        }

        // Property: RightDetails
        public static readonly DependencyProperty RightDetailsProperty
            = DependencyProperty.Register("RightDetails", typeof(string), typeof(GraphControl),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnRightDetailsChanged)));

        private static void OnRightDetailsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as GraphControl;
            if (!control.isLoaded) return;
            control.SetDetailsVisibility(control.m_rightDetails, e.NewValue.ToString());
        }

        public string RightDetails
        {
            get { return (string)GetValue(RightDetailsProperty); }
            set { SetValue(RightDetailsProperty, value); }
        }
        #endregion

        #region Template
        private TextBlock m_title;
        private Grid m_container;
        private Canvas m_canvas;
        private Rectangle m_mask;
        private TextBlock m_leftDetails;
        private TextBlock m_rightDetails;
        #endregion

        #region Variables
        private int xPixels = 0;
        private int yPixels = 0;
        private double xSize = 0;
        private double ySize = 0;
        private double pixelSize = 3.5;
        private bool isLoaded = false;
        private double oldGraphWidth = 0;
        #endregion

        protected override Size MeasureOverride(Size constraint)
        {
            if (isLoaded) {
                bool useHalfSnap = (constraint.Width <= oldGraphWidth);
                double leftPad = constraint.Width - SnapToGrid(constraint.Width, pixelSize, useHalfSnap);
                leftPad = Double.IsNaN(leftPad) ? 0 : leftPad;
                m_container.Margin = new Thickness(leftPad, 0, 0, 0);
                oldGraphWidth = constraint.Width;
            }

            return base.MeasureOverride(constraint);
        }

        #region Main
        static GraphControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GraphControl), 
                new FrameworkPropertyMetadata(typeof(GraphControl)));
        }

		public override void OnApplyTemplate()
        {
            m_title = Template.FindName("PART_Title", this) as TextBlock;
            m_container = Template.FindName("PART_Container", this) as Grid;
            m_canvas = Template.FindName("PART_Canvas", this) as Canvas;
            m_mask = Template.FindName("PART_Mask", this) as Rectangle;
            m_leftDetails = Template.FindName("PART_LeftDetails", this) as TextBlock;
            m_rightDetails = Template.FindName("PART_RightDetails", this) as TextBlock;

            base.OnApplyTemplate();
        }

        public GraphControl()
        {
            this.Loaded += (s, e) => {

                // Calcola le dimensioni dell'area di lavoro per l'asse X.
                xPixels = FixedXPixels == -1 ? (int)Math.Round(this.ActualWidth / pixelSize) : FixedXPixels;
                xSize = pixelSize * xPixels;
                m_container.Width = xSize;
       
                // Calcola le dimensioni dell'area di lavoro per l'asse Y.
                yPixels = FixedYPixels == -1 ? (int)Math.Round(this.ActualHeight / pixelSize) : FixedYPixels;
                ySize = pixelSize * yPixels;
                m_container.Height = ySize;

                SetDetailsVisibility(m_title, Title);
                SetDetailsVisibility(m_leftDetails, LeftDetails);
                SetDetailsVisibility(m_rightDetails, RightDetails);
                isLoaded = true;
            };
        }

        private void SetDetailsVisibility(TextBlock el, string text)
        {
            if (text != null) {
                el.Visibility = Visibility.Visible;
                el.Text = text;
            } else {
                el.Visibility = Visibility.Collapsed;
            }
        }
        #endregion

        #region Controls
        /// <summary>
        /// Aggiorna il valore del grafico con i nuovi dati.
        /// </summary>
        public void Update(GraphCollection values)
        {
            m_canvas.Children.Clear();

            // Calcola i punti del grafico.
            List<Point> points = new List<Point>();
            points.Add(new Point(xSize, ySize));
            for (int i = 0; i < values.Points.Count; ++i)
            {
                double y1 = SnapToGrid((1.0 - values.Points[i]) * ySize, pixelSize);
                double y2 = y1;
                double x1 = xSize - (i * pixelSize);
                double x2 = xSize - ((i + 1) * pixelSize);

                points.Add(new Point(x1, y1));
                points.Add(new Point(x2, y2));
            }
            points.Add(new Point(points[points.Count - 1].X, ySize));

            // Determina il colore del grafico.      
            SolidColorBrush fillColor = this.Fill;
            if (values.IsFillBasedOnValue) {  
                double lastPoint = values.Points[0];
                if (lastPoint <= 0.6) {
                    fillColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6AD35E"));
                } else if (lastPoint <= 0.8) {
                    fillColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFAD0A"));
                } else if (lastPoint <= 1.0) {
                    fillColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF7325"));
                }
            }

            // Disegna il grafico.
            PathSegmentCollection lines = new PathSegmentCollection();
            lines.Add(new PolyLineSegment(points, false));
            PathFigure f = new PathFigure(new Point(xSize, ySize), lines, false);
            PathGeometry g = new PathGeometry(new PathFigure[] { f });
            Path path = new Path()
            {
                Fill = fillColor,
                StrokeThickness = 0,
                Data = g
            };

            m_canvas.Children.Add(path);
        }

        /// <summary>
        /// Cancella tutti i valori dal grafico.
        /// </summary>
        public void Clear()
        {
            m_canvas.Children.Clear();
        }

        private double SnapToGrid(double value, double snap, bool snapToHalf = true)
        {
            double xSnap = value % snap;

            if (xSnap <= snap / (snapToHalf ? 2.0 : 1.0)) xSnap *= -1;
            else xSnap = snap - xSnap;

            xSnap += value;
            return xSnap;
        }
        #endregion
    }
}
