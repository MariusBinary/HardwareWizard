using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using HardwareWizard.Core;
using HardwareWizard.Core.Helpers;
using HardwareWizard.Interfaces;

namespace HardwareWizard.Controls
{
    [TemplatePart(Name = "PART_Title", Type = typeof(TextBlock))]
    [TemplatePart(Name = "PART_GpuIcon", Type = typeof(Image))]
    [TemplatePart(Name = "PART_GpuUsageRing", Type = typeof(CircularProgressControl))]
    [TemplatePart(Name = "PART_GpuUsageGraph", Type = typeof(GraphControl))]
    [TemplatePart(Name = "PART_GpuMemory", Type = typeof(DetailedProgressControl))]
    [TemplatePart(Name = "PART_GpuTemperature", Type = typeof(DetailedProgressControl))]
    public class DashboardGpuControl : Control, IViewUpdate
    {
        #region Template
        private CircularProgressControl m_gpuUsageRing;
        private GraphControl m_gpuUsageGraph;
        private TextBlock m_title;
        private DetailedProgressControl m_gpuMemory;
        private DetailedProgressControl m_gpuTemperature;
        private Image m_gpuIcon;
        #endregion

        #region Variables
        private GraphicsCardData graphicsCard;
        private CoolingThermalData temperature;
        private bool hasHardware = false;
        private bool isLoaded = false;
        #endregion

        #region Main
        static DashboardGpuControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DashboardGpuControl), 
                new FrameworkPropertyMetadata(typeof(DashboardGpuControl)));
        }

        public override void OnApplyTemplate()
        {
            m_gpuUsageRing = Template.FindName("PART_GpuUsageRing", this) as CircularProgressControl;
            m_gpuUsageGraph = Template.FindName("PART_GpuUsageGraph", this) as GraphControl;
            m_title = Template.FindName("PART_Title", this) as TextBlock;
            m_gpuIcon = Template.FindName("PART_GpuIcon", this) as Image;
            m_gpuMemory = Template.FindName("PART_GpuMemory", this) as DetailedProgressControl;
            m_gpuTemperature = Template.FindName("PART_GpuTemperature", this) as DetailedProgressControl;

            base.OnApplyTemplate();
        }

        public DashboardGpuControl(GraphicsCardData graphicsCard)
        {
            this.graphicsCard = graphicsCard;

            if (graphicsCard.Hardware != null) {
                hasHardware = true;
                this.temperature = graphicsCard.Thermals.Where(
                    x => x.Name.ToLower().StartsWith("gpu core")).FirstOrDefault();
            }

            Loaded += (s, e) => {
                m_title.Text = graphicsCard.Model;
                m_gpuIcon.Source = new BitmapImage(new Uri(
                  $"pack://application:,,,/HardwareWizard;component/Images/Shared/ic_gpu_{graphicsCard.ManufacturerId}_small.png"));

                if (hasHardware) {
                    m_gpuUsageRing.AddTag("Load", "#4994F7");
                    m_gpuUsageRing.AddTag("Free", "Control.DetailedProgress.Background", true);
                    m_gpuUsageRing.Release();
                } else {
                    m_gpuUsageRing.SetEmptyMode();
                    m_gpuUsageGraph.RightDetails = "N/A";
                    m_gpuMemory.RightDetails = $"N/A";
                    m_gpuTemperature.RightDetails = "N/A";
                }

                isLoaded = true;
            };
        }
        #endregion

        #region IViewUpdate
        public void OnViewUpdate()
        {
            if (!hasHardware || !isLoaded) return;

            m_gpuUsageRing.Reset(0);
            m_gpuUsageRing.Update("Load", (double)graphicsCard.Usage);
            m_gpuUsageRing.Update("Free");
            m_gpuUsageRing.Release();

            m_gpuUsageGraph.Update(graphicsCard.UsageCollection);
            m_gpuUsageGraph.RightDetails = $"{graphicsCard.Usage}%";

            m_gpuMemory.Value = ((double)graphicsCard.Memory / (double)graphicsCard.MaximumMemory);
            m_gpuMemory.RightDetails = $"{m_gpuMemory.Value * 100}%";

            if (temperature != null)
            {
                m_gpuTemperature.Value = (double)temperature.Temperature / (double)temperature.Maximum;
                m_gpuTemperature.RightDetails = TemperatureHelper.Get(temperature.Temperature);
            }
        }
        #endregion
    }
}
