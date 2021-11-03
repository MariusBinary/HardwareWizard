using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HardwareWizard.Controls
{
    [TemplatePart(Name = "PART_Title", Type = typeof(TextBlock))]
    [TemplatePart(Name = "PART_ProgressBar", Type = typeof(ProgressBar))]
    [TemplatePart(Name = "PART_LeftDetails", Type = typeof(TextBlock))]
    [TemplatePart(Name = "PART_RightDetails", Type = typeof(TextBlock))]
    public class DetailedProgressControl : Control
    {
        #region Properties
        // Property: Value
        public static readonly DependencyProperty ValueProperty 
            = DependencyProperty.Register("Value", typeof(double), typeof(DetailedProgressControl),
                new FrameworkPropertyMetadata(0.0, new PropertyChangedCallback(OnValueChanged)));

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as DetailedProgressControl;
            if (!control.isLoaded) return;
            control.m_progressBar.Value = Convert.ToDouble(e.NewValue);
        }

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Property: IndicatorHeight
        public static readonly DependencyProperty IndicatorHeightProperty
            = DependencyProperty.Register("IndicatorHeight", typeof(double), typeof(DetailedProgressControl),
                new FrameworkPropertyMetadata(16.0, new PropertyChangedCallback(OnIndicatorHeightChanged)));

        private static void OnIndicatorHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as DetailedProgressControl;
            if (!control.isLoaded) return;
            control.m_progressBar.Height = Convert.ToDouble(e.NewValue);
        }

        public double IndicatorHeight
        {
            get { return (double)GetValue(IndicatorHeightProperty); }
            set { SetValue(IndicatorHeightProperty, value); }
        }

        // Property: Title
        public static readonly DependencyProperty TitleProperty
            = DependencyProperty.Register("Title", typeof(string), typeof(DetailedProgressControl),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnTitleChanged)));

        private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as DetailedProgressControl;
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
            = DependencyProperty.Register("LeftDetails", typeof(string), typeof(DetailedProgressControl),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnLeftDetailsChanged)));

        private static void OnLeftDetailsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as DetailedProgressControl;
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
            = DependencyProperty.Register("RightDetails", typeof(string), typeof(DetailedProgressControl),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnRightDetailsChanged)));

        private static void OnRightDetailsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as DetailedProgressControl;
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
        private ProgressBar m_progressBar;
        private TextBlock m_leftDetails;
        private TextBlock m_rightDetails;
        #endregion

        #region Variables
        private bool isLoaded = false;
        #endregion

        #region Main
        static DetailedProgressControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DetailedProgressControl), 
                new FrameworkPropertyMetadata(typeof(DetailedProgressControl)));
        }

        public override void OnApplyTemplate()
        {
            m_title = Template.FindName("PART_Title", this) as TextBlock;
            m_progressBar = Template.FindName("PART_ProgressBar", this) as ProgressBar;
            m_leftDetails = Template.FindName("PART_LeftDetails", this) as TextBlock;
            m_rightDetails = Template.FindName("PART_RightDetails", this) as TextBlock;

            base.OnApplyTemplate();
        }

        public DetailedProgressControl()
        {
            this.Loaded += (s, e) => {
                m_progressBar.Value = Value;
                m_progressBar.Height = IndicatorHeight;
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
    }
}
