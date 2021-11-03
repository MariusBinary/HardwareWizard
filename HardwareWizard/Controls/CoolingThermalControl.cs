using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HardwareWizard.Core;
using HardwareWizard.Core.Helpers;
using HardwareWizard.Interfaces;

namespace HardwareWizard.Controls
{
    [TemplatePart(Name = "PART_GraphsPanel", Type = typeof(GraphsGridPanel))]
    [TemplatePart(Name = "PART_Title", Type = typeof(TextBlock))]
    public class CoolingThermalControl : Control, IViewUpdate
    {
        #region Template
        private GraphsGridPanel m_graphsPanel;
        private TextBlock m_title;
        #endregion

        #region Variables
        private CoolingThermalGroup thermalGroup;
        private bool isLoaded = false;
        #endregion

        #region Main
        static CoolingThermalControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CoolingThermalControl),
                new FrameworkPropertyMetadata(typeof(CoolingThermalControl)));
        }

        public override void OnApplyTemplate()
        {
            m_graphsPanel = Template.FindName("PART_GraphsPanel", this) as GraphsGridPanel;
            m_title = Template.FindName("PART_Title", this) as TextBlock;

            m_title.Text = thermalGroup.Name;
            if (thermalGroup.IsGroup)
            {
                foreach (CoolingThermalData thermalData in thermalGroup.ThermalDatas)
                {
                    m_graphsPanel.AddTag(thermalData.Name);
                    m_graphsPanel.GetGraph(thermalData.Name).Title = thermalData.Name;
                    m_graphsPanel.GetGraph(thermalData.Name).LeftDetails = "-";
                    m_graphsPanel.GetGraph(thermalData.Name).RightDetails = "-";
                }
            }

            base.OnApplyTemplate();
        }

        public CoolingThermalControl(CoolingThermalGroup thermalGroup)
        {
            this.thermalGroup = thermalGroup;
            this.Loaded += CoolingCpuControl_Loaded;
        }

        private void CoolingCpuControl_Loaded(object sender, RoutedEventArgs e)
        { 
            isLoaded = true;
        }
        #endregion

        #region IViewUpdate
        public void OnViewUpdate()
        {
            if (!isLoaded) return;

            // Aggiornamento grafico.
            foreach (CoolingThermalData thermalData in thermalGroup.ThermalDatas) {
                m_graphsPanel.GetGraph(thermalData.Name).Update(thermalData.TemperatureData);
                m_graphsPanel.GetGraph(thermalData.Name).LeftDetails = $"Min {TemperatureHelper.Get(thermalData.Minimum)}";
                m_graphsPanel.GetGraph(thermalData.Name).RightDetails = TemperatureHelper.Get(thermalData.Temperature);
            }
        }
        #endregion
    }
}
