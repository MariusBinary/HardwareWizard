using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Shapes;

namespace HardwareWizard.Controls
{
    [TemplatePart(Name = "PART_Container", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_Title", Type = typeof(Run))]
    [TemplatePart(Name = "PART_Value", Type = typeof(Run))]
    public class CircularProgressControl : Control
    {
        #region Properties
        // Property: Title
        public static readonly DependencyProperty TitleProperty
            = DependencyProperty.Register("Title", typeof(string), typeof(CircularProgressControl),
                new FrameworkPropertyMetadata("-", new PropertyChangedCallback(OnTitleChanged)));

        private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as CircularProgressControl;
            if (!control.isLoaded) return;
            control.m_title.Text = e.NewValue.ToString();
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Property: Value
        public static readonly DependencyProperty ValueProperty
            = DependencyProperty.Register("Value", typeof(int), typeof(CircularProgressControl),
                new FrameworkPropertyMetadata(0, new PropertyChangedCallback(OnValueChanged)));

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as CircularProgressControl;
            if (!control.isLoaded) return;
            control.m_value.Text = $"{e.NewValue}%";
        }

        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        #endregion

        #region Template
        private Grid m_container;
        private Run m_title;
        private Run m_value;
        #endregion

        #region Variables
        private Dictionary<string, CircularProgressShape> values;
        private bool isLoaded = false;
        private bool isEmpty = false;
        #endregion

        #region Main
        static CircularProgressControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CircularProgressControl),
                new FrameworkPropertyMetadata(typeof(CircularProgressControl)));
        }

        public override void OnApplyTemplate()
        {
            m_container = Template.FindName("PART_Container", this) as Grid;
            m_title = Template.FindName("PART_Title", this) as Run;
            m_value = Template.FindName("PART_Value", this) as Run;

            base.OnApplyTemplate();
        }

        public CircularProgressControl()
        {
            this.values = new Dictionary<string, CircularProgressShape>();
            this.Loaded += (s, e) => {
                if (m_container.Children.Count == 0)
                {
                    foreach (CircularProgressShape item in values.Values)
                    {
                        m_container.Children.Add(item);
                    }
                }

                if (isEmpty) {
                    m_title.Text = "Available";
                    m_value.Text = $"Not";
                } else {
                    m_title.Text = Title;
                    m_value.Text = $"{Value}%";
                }

                isLoaded = true;
            };
        }
        #endregion

        #region Controls
        public void AddTag(string tag, string hex = "#ffffff", bool isDynamicResource = false)
        {
            ScaleTransform myScaleTransform = new ScaleTransform();
            TransformGroup myTransformGroup = new TransformGroup();
            myTransformGroup.Children.Add(myScaleTransform);

            var item = new CircularProgressShape() { 
                RenderTransformOrigin = new Point(0.5, 0.5),
                RenderTransform = myTransformGroup
            };

            if (isDynamicResource) {
                item.SetResourceReference(Shape.StrokeProperty, hex);
            } else {
                item.Stroke = new SolidColorBrush((Color)(ColorConverter.ConvertFromString(hex)));
            }

            values.Add(tag, item);

            if (m_container != null) {
                m_container.Children.Add(item);
            }        
        }

        public void RemoveTag(string tag)
        {
            m_container.Children.Remove(values[tag]);
            if (values.ContainsKey(tag)) {
                values.Remove(tag);
            }
        }

        double lastUpdateValue = 0;
        double mainValuePercentage = 0;

        public void Reset(double value)
        {
            lastUpdateValue = value;
            mainValuePercentage = value;
        }

        public void Update(string tag, double value = -1, bool addToValue = true)
        {
            if (values.ContainsKey(tag)) {
                mainValuePercentage = addToValue ? mainValuePercentage + value : mainValuePercentage;
                value = value == -1 ? 100 - lastUpdateValue : value;
                values[tag].StartValue = lastUpdateValue;
                values[tag].EndValue = value;
                lastUpdateValue += value;
            }
        }

        public void Release()
        {
            Value = (int)Math.Ceiling(mainValuePercentage);
        }

        public void SetEmptyMode()
        {
            AddTag("Free", "Control.DetailedProgress.Background", true);
            Update("Free");
            m_title.Text = "Available";
            m_value.Text = $"Not";
            isEmpty = true;
        }
        #endregion
    }
}
