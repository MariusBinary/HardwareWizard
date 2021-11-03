using System.Windows;
using System.Windows.Controls;
using HardwareWizard.Core;
using HardwareWizard.Models;
using HardwareWizard.Interfaces;
using System;

namespace HardwareWizard.Controls
{
    [TemplatePart(Name = "PART_RamUsageRing", Type = typeof(CircularProgressControl))]
    [TemplatePart(Name = "PART_RamUsageGraph", Type = typeof(GraphControl))]
    [TemplatePart(Name = "PART_RamUsageLegend", Type = typeof(ListView))]
    [TemplatePart(Name = "PART_RamProcesses", Type = typeof(ListView))]
    public class DashboardRamControl : Control, IViewUpdate
    {
        #region Template
        private CircularProgressControl m_ramUsageRing;
        private GraphControl m_ramUsageGraph;
        private ListView m_ramUsageLegend;
        private ListView m_ramProcesses;
        #endregion

        #region Variables
        public MemoryData memoryData;
        private bool isLoaded = false;
        #endregion

        #region Main
        static DashboardRamControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DashboardRamControl), 
                new FrameworkPropertyMetadata(typeof(DashboardRamControl)));
        }

        public override void OnApplyTemplate()
        {
            m_ramUsageRing = Template.FindName("PART_RamUsageRing", this) as CircularProgressControl;
            m_ramUsageGraph = Template.FindName("PART_RamUsageGraph", this) as GraphControl;
            m_ramUsageLegend = Template.FindName("PART_RamUsageLegend", this) as ListView;
            m_ramProcesses = Template.FindName("PART_RamProcesses", this) as ListView;

            base.OnApplyTemplate();
        }

        public DashboardRamControl(MemoryData memoryData)
        {
            this.memoryData = memoryData;
            this.Loaded += (s, e) => {
                m_ramUsageRing.AddTag("Available", "Control.DetailedProgress.Background", true);
                m_ramUsageRing.AddTag("Hardware Reserved", "#4994F7");
                m_ramUsageRing.AddTag("Modified", "#F0A13C");
                m_ramUsageRing.AddTag("Standby", "#DADADA");
                m_ramUsageRing.AddTag("In Use", "#EB4B5E");

                for (int i = 0; i < 5; i++) {
                    m_ramProcesses.Items.Add(new ProcessItemModel() { Name = "-", Value = "-" });
                }

                isLoaded = true;
            };
        }
        #endregion

        #region IViewUpdate
        public void OnViewUpdate()
        {
            if (!isLoaded) return;

            m_ramUsageRing.Reset(0);
            m_ramUsageRing.Update("Hardware Reserved", (memoryData.HardwareReserved * 100) / memoryData.osTotalMemory, false);
            m_ramUsageRing.Update("In Use", ((memoryData.InUseBytes) * 100) / memoryData.osTotalMemory, true);
            m_ramUsageRing.Update("Modified", (memoryData.ModifiedBytes * 100) / memoryData.osTotalMemory, true);
            m_ramUsageRing.Update("Standby", (memoryData.StandbyBytes * 100) / memoryData.osTotalMemory, false);
            m_ramUsageRing.Update("Available", -1, false);
            m_ramUsageRing.Release();

            // Aggiorno i processi in esecuzione.
            if (memoryData.Processes.Count > 0)
            {
                m_ramProcesses.Items.Clear();
                for (int i = 0; i < 5; i++)
                {
                    m_ramProcesses.Items.Add(new ProcessItemModel()
                    {
                        Image = memoryData.Processes[i].Icon,
                        Name = memoryData.Processes[i].Process.ProcessName,
                        Value = Utils.SizeSuffix((ulong)memoryData.Processes[i].RAM)
                    });
                }
            }


            (m_ramUsageLegend.Items[0] as LegendItemModel).Usage = Utils.SizeSuffix(memoryData.HardwareReserved);
            (m_ramUsageLegend.Items[1] as LegendItemModel).Usage = Utils.SizeSuffix(memoryData.InUseBytes);
            (m_ramUsageLegend.Items[2] as LegendItemModel).Usage = Utils.SizeSuffix(memoryData.ModifiedBytes);
            (m_ramUsageLegend.Items[3] as LegendItemModel).Usage = Utils.SizeSuffix(memoryData.StandbyBytes);
            (m_ramUsageLegend.Items[4] as LegendItemModel).Usage = Utils.SizeSuffix(memoryData.FreeBytes);

            m_ramUsageGraph.Update(memoryData.UsageCollection);
            m_ramUsageGraph.RightDetails = $"{m_ramUsageRing.Value}%";
        }
        #endregion
    }
}
