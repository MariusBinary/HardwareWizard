using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using HardwareWizard.Core;

namespace HardwareWizard.Views
{
    public partial class ProcessorView : UserControl
    {
        #region Variables
        private ProcessorData processorData;
        #endregion

        #region Main
        public ProcessorView(ProcessorData processorData)
        {
            InitializeComponent();
            this.processorData = processorData;
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Tx_ProcessorModel.Text = processorData.Model;
            Tx_ProcessorManufacturer.Text = processorData.Manufacturer;
            Img_ProcessorIcon.Source = new BitmapImage(new Uri(
                $"pack://application:,,,/HardwareWizard;component/Images/Shared/ic_cpu_{processorData.ManufacturerId}.png"));

            // Aggiunge le informazioni.
            Table_Informations.AddTag("Model", processorData.Model, 0);
            Table_Informations.AddTag("Manufacturer", processorData.Manufacturer, 0);
            Table_Informations.AddTag("Cores", processorData.CoresCount, 0);
            Table_Informations.AddTag("Threads", processorData.ThreadsCount, 0);
            Table_Informations.AddTag("Virtualization", processorData.IsVirtualizationEnabled ? "Enabled" : "Disabled", 0);
            Table_Informations.AddTag("L2 Cache", processorData.L2Cache, 1);
            Table_Informations.AddTag("L3 Cache", processorData.L3Cache, 1);
            Table_Informations.AddTag("Clock Speed", processorData.ClockSpeed, 1);
            Table_Informations.AddTag("Max Clock Speed", processorData.MaxClockSpeed, 1);

            // Aggiungere le proprietà WMI.
            for (int i = 0; i < processorData.WMI.Count; i++) {
                Table_WMI.AddTag(processorData.WMI[i].Property, processorData.WMI[i].Value, i % 2 == 0 ? 0 : 1);
            }
        }

        private void Btn_Report_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ReportHelper report = new ReportHelper("cpu");
            report.AddToReport("Informations", Table_Informations);
            report.AddToReport("WMI", Table_WMI);
            report.MakeReport();
        }
        #endregion
    }
}
