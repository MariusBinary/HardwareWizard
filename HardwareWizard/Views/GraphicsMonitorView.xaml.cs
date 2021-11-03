using System.Windows.Controls;
using HardwareWizard.Core;

namespace HardwareWizard.Views
{
    public partial class GraphicsMonitorView : UserControl
    {
        #region Variables
        private GraphicsMonitorData monitorData;
        #endregion

        #region Main
        public GraphicsMonitorView(GraphicsMonitorData monitorData)
        {
            InitializeComponent();
            this.monitorData = monitorData;
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Tx_MonitorModel.Text = monitorData.Model;
            Tx_MonitorManufacturer.Text = monitorData.Manufacturer;

            Table_Informations.AddTag("Manufacturer", monitorData.Manufacturer, 0);
            Table_Informations.AddTag("Model", monitorData.Model, 0);

            // Aggiungere le proprietà WMI.
            for (int i = 0; i < monitorData.WMI.Count; i++) {
                Table_WMI.AddTag(monitorData.WMI[i].Property, monitorData.WMI[i].Value, i % 2 == 0 ? 0 : 1);
            }
        }

        private void Btn_Report_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ReportHelper report = new ReportHelper("monitor");
            report.AddToReport("Informations", Table_Informations);
            report.AddToReport("WMI", Table_WMI);
            report.MakeReport();
        }
        #endregion
    }
}
