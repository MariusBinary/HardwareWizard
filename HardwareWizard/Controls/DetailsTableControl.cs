using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using HardwareWizard.Models;
using System.Windows.Input;

namespace HardwareWizard.Controls
{
    [TemplatePart(Name = "PART_LeftDetailsList", Type = typeof(ListView))]
    [TemplatePart(Name = "PART_RightDetailsList", Type = typeof(ListView))]
    public class DetailsTableControl : Control
    {
        #region Template
        private ListView m_leftDetailsList;
        private ListView m_rightDetailsList;
        #endregion

        #region Variables
        private Dictionary<string, DetailsTableItemModel> values;
        #endregion

        #region Main
        static DetailsTableControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DetailsTableControl), 
                new FrameworkPropertyMetadata(typeof(DetailsTableControl)));
        }

		public override void OnApplyTemplate()
        {
            m_leftDetailsList = Template.FindName("PART_LeftDetailsList", this) as ListView;
            m_rightDetailsList = Template.FindName("PART_RightDetailsList", this) as ListView;


            m_leftDetailsList.PreviewMouseWheel += (s, e) => {
                e.Handled = true;
                MouseWheelEventArgs e2 = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                e2.RoutedEvent = UIElement.MouseWheelEvent;
                m_leftDetailsList.RaiseEvent(e2);
            };

            m_rightDetailsList.PreviewMouseWheel += (s, e) => {
                e.Handled = true;
                MouseWheelEventArgs e2 = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                e2.RoutedEvent = UIElement.MouseWheelEvent;
                m_rightDetailsList.RaiseEvent(e2);
            };

            base.OnApplyTemplate();
        }

        public DetailsTableControl()
        {
            this.values = new Dictionary<string, DetailsTableItemModel>();
            this.Loaded += (s, e) => {
                if (m_leftDetailsList.Items.Count == 0)
                {
                    foreach (DetailsTableItemModel item in values.Values)
                    {
                        if (item.Column == 0)
                        {
                            m_leftDetailsList.Items.Add(item);
                        }
                    }
                }

                if (m_rightDetailsList.Items.Count == 0)
                {
                    foreach (DetailsTableItemModel item in values.Values)
                    {
                        if (item.Column > 0)
                        {
                            m_rightDetailsList.Items.Add(item);
                        }
                    }
                }
            };
        }
        #endregion

        #region Controls
        public void AddTag(string tag, string value, int column)
        {
            var item = new DetailsTableItemModel() {
                Name = tag,
                Value = value,
                Column = column
            };

            values.Add(tag, item);

            if (column == 0 && m_leftDetailsList != null) {
                m_leftDetailsList.Items.Add(item);
            } else if (column > 0 && m_rightDetailsList != null) {
                m_rightDetailsList.Items.Add(item);
            }
        }

        public void Update(string tag, string value)
        {
            if (values.ContainsKey(tag))
            {
                values[tag].Value = value;
            }
        }

        public Dictionary<string, DetailsTableItemModel> GetValues()
        {
            return values;
        }
        #endregion
    }
}
