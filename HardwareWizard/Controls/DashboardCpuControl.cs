using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using HardwareWizard.Core;
using HardwareWizard.Core.Helpers;
using HardwareWizard.Interfaces;
using HardwareWizard.Models;

namespace HardwareWizard.Controls
{
    [TemplatePart(Name = "PART_GpuUsageRing", Type = typeof(CircularProgressControl))]
    [TemplatePart(Name = "PART_GpuUsageGraph", Type = typeof(GraphControl))]
    [TemplatePart(Name = "PART_CpuTemperature", Type = typeof(DetailedProgressControl))]
    [TemplatePart(Name = "PART_CpuUsageLegend", Type = typeof(ListView))]
    [TemplatePart(Name = "PART_CpuProcesses", Type = typeof(ListView))]
    [TemplatePart(Name = "PART_CpuName", Type = typeof(TextBlock))]
    [TemplatePart(Name = "PART_CpuIcon", Type = typeof(Image))]
    public class DashboardCpuControl : Control, IViewUpdate
    {
        #region Template
        private CircularProgressControl m_cpuUsageRing;
        private GraphControl m_cpuUsageGraph;
        private DetailedProgressControl m_cpuTemperature;
        private ListView m_cpuUsageLegend;
        private ListView m_cpuProcesses;
        private TextBlock m_cpuName;
        private Image m_cpuIcon;
        #endregion

        #region Variables
        private CoolingThermalData temperature;
        private ProcessorData processorData;
        private bool isLoaded = false;
        #endregion

        #region Main
        static DashboardCpuControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DashboardCpuControl), 
                new FrameworkPropertyMetadata(typeof(DashboardCpuControl)));
        }

        public override void OnApplyTemplate()
        {
            m_cpuUsageRing = Template.FindName("PART_CpuUsageRing", this) as CircularProgressControl;
            m_cpuUsageGraph = Template.FindName("PART_CpuUsageGraph", this) as GraphControl;
            m_cpuTemperature = Template.FindName("PART_CpuTemperature", this) as DetailedProgressControl;
            m_cpuUsageLegend = Template.FindName("PART_CpuUsageLegend", this) as ListView;
            m_cpuProcesses = Template.FindName("PART_CpuProcesses", this) as ListView;
            m_cpuName = Template.FindName("PART_CpuName", this) as TextBlock;
            m_cpuIcon = Template.FindName("PART_CpuIcon", this) as Image;

            base.OnApplyTemplate();
        }

        public DashboardCpuControl(ProcessorData processorData)
        {
            this.processorData = processorData;
            this.temperature = processorData.Thermals.Where(
                x => x.Name.ToLower().StartsWith("cpu package")).FirstOrDefault();

            this.Loaded += (s, e) => {

                if (temperature == null) {
                    m_cpuTemperature.Value = 0.0;
                    m_cpuTemperature.RightDetails = "N/A";
                }

                m_cpuName.Text = processorData.Model;
                m_cpuIcon.Source = new BitmapImage(new Uri(
                    $"pack://application:,,,/HardwareWizard;component/Images/Shared/ic_cpu_{processorData.ManufacturerId}_small.png"));

                m_cpuUsageRing.AddTag("System", "#EB4B5E");
                m_cpuUsageRing.AddTag("User", "#4994F7");
                m_cpuUsageRing.AddTag("Free", "Control.DetailedProgress.Background", true);

                for (int i = 0; i < 5; i++) {
                    m_cpuProcesses.Items.Add(new ProcessItemModel() { Name = "-", Value = "-" });
                }

                isLoaded = true;
            };
        }
        #endregion

        #region IViewUpdate
        public void OnViewUpdate()
        {
            if (!isLoaded) return;

            // Aggiorna l'anello dell'utilizzo.
            m_cpuUsageRing.Reset(0);
            m_cpuUsageRing.Update("System", processorData.PrivilegedUsage);
            m_cpuUsageRing.Update("User", processorData.UserUsage);
            m_cpuUsageRing.Update("Free");
            m_cpuUsageRing.Release();

            // Aggiorno la leggenda di utilizzo.
            (m_cpuUsageLegend.Items[0] as LegendItemModel).Usage = $"{Math.Round(processorData.PrivilegedUsage, 1)}%";
            (m_cpuUsageLegend.Items[1] as LegendItemModel).Usage = $"{Math.Round(processorData.UserUsage, 1)}%";

            // Aggiorno il grafico di utilizzo.
            m_cpuUsageGraph.Update(processorData.UsageCollection);
            m_cpuUsageGraph.RightDetails = $"{Math.Round(processorData.Usage, 1)}%";

            // Aggiorno i processi in esecuzione.
            if (processorData.Processes.Count > 0)
            {
                m_cpuProcesses.Items.Clear();
                for (int i = 0; i < 5; i++)
                {
                    m_cpuProcesses.Items.Add(new ProcessItemModel()
                    {
                        Image = processorData.Processes[i].Icon,
                        Name = processorData.Processes[i].Process.ProcessName,
                        Value = Math.Round(processorData.Processes[i].CPU, 1) + "%"
                    });
                }
            }

            // Aggiorno l'indicatore della temperatura.
            if (temperature != null)
            {
                m_cpuTemperature.Value = (double)temperature.Temperature / (double)temperature.Maximum;
                m_cpuTemperature.RightDetails = TemperatureHelper.Get(temperature.Temperature);
            }
        }
        #endregion
    }
}
