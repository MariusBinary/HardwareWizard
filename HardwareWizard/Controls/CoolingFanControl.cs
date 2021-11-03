using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Animation;
using HardwareWizard.Core;
using HardwareWizard.Interfaces;

namespace HardwareWizard.Controls
{
    [TemplatePart(Name = "PART_FanName", Type = typeof(TextBlock))]
    [TemplatePart(Name = "PART_FanRPM", Type = typeof(Run))]
    [TemplatePart(Name = "PART_FanBlades", Type = typeof(Image))]
    [TemplatePart(Name = "PART_Graph", Type = typeof(GraphControl))]
    public class CoolingFanControl : Control, IViewUpdate
    {
        #region Template
        private TextBlock m_fanName;
        private Run m_fanRPM;
        private GraphControl m_graph;
        private Image m_fanBlades;
        #endregion

        #region Variables
        private bool isFanAnimation = false;
        private CoolingFanData coolingData;
        private bool isLoaded = false;
        #endregion

        #region Main
        static CoolingFanControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CoolingFanControl),
                new FrameworkPropertyMetadata(typeof(CoolingFanControl)));
        }

        public override void OnApplyTemplate()
        {
            m_fanName = Template.FindName("PART_FanName", this) as TextBlock;
            m_fanRPM = Template.FindName("PART_FanRPM", this) as Run;
            m_fanBlades = Template.FindName("PART_FanBlades", this) as Image;
            m_graph = Template.FindName("PART_Graph", this) as GraphControl;

            m_graph.Title = $"Max {(int)coolingData.Maximum}";
            m_graph.LeftDetails = $"Min {(int)coolingData.Minimum}";
            m_graph.RightDetails = $"{(int)coolingData.Speed} RPM";

            base.OnApplyTemplate();
        }

        public CoolingFanControl(CoolingFanData coolingData)
        {
            this.coolingData = coolingData;

            Loaded += (s, e) => {
                m_fanName.Text = coolingData.Name;
                isLoaded = true;
            };
        }
        #endregion

        #region IViewUpdate
        public void OnViewUpdate()
        {
            if (!isLoaded) return;

            m_fanRPM.Text = ((int)coolingData.Speed).ToString();
            m_graph.Update(coolingData.SpeedData);
            m_graph.Title = $"Max {(int)coolingData.Maximum}";
            m_graph.LeftDetails = $"Min {(int)coolingData.Minimum}";
            m_graph.RightDetails = $"{(int)coolingData.Speed} RPM";

            if (coolingData.Speed.Value > 0)
            {
                if (!isFanAnimation)
                {
                    StartFanAnimation();
                }
            }
            else
            {
                if (isFanAnimation)
                {
                    StopFanAnimation();
                }
            }
        }

        private void StartFanAnimation()
        {
            (m_fanBlades.Resources["PART_FanAnimation"] as Storyboard).Begin();
            isFanAnimation = true;
        }

        private void StopFanAnimation()
        {
            (m_fanBlades.Resources["PART_FanAnimation"] as Storyboard).Stop();
            isFanAnimation = false;
        }
        #endregion
    }
}
