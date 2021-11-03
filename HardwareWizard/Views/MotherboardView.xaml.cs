using System.Windows.Controls;
using HardwareWizard.Core;

namespace HardwareWizard.Views
{
    public partial class MotherboardView : UserControl
    {
        #region Variables
        private MotherboardData motherboardData;
        #endregion

        #region Main
        public MotherboardView(MotherboardData motherboardData)
        {
            InitializeComponent();
            this.motherboardData = motherboardData;
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Tx_MotherboardModel.Text = motherboardData.Model;
            Tx_MotherboardManufacturer.Text = motherboardData.Manufacturer;

            Table_Informations.AddTag("Manufacturer", motherboardData.Manufacturer, 0);
            Table_Informations.AddTag("Model", motherboardData.Model, 0);
            Table_Informations.AddTag("SerialNumber", motherboardData.SerialNumber, 0);
            Table_Informations.AddTag("Version", motherboardData.Version, 0);
            Table_Informations.AddTag("BIOS Brand", motherboardData.BIOSBrand, 1);
            Table_Informations.AddTag("BIOS Version", motherboardData.BIOSVersion, 1);
            Table_Informations.AddTag("BIOS Date", motherboardData.BIOSDate, 1);

            // Aggiungere le proprietà WMI.
            for (int i = 0; i < motherboardData.WMI.Count; i++) {
                Table_WMI.AddTag(motherboardData.WMI[i].Property, motherboardData.WMI[i].Value, i % 2 == 0 ? 0 : 1);
            }
        }

        private void Btn_Report_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ReportHelper report = new ReportHelper("drive");
            report.AddToReport("Informations", Table_Informations);
            report.AddToReport("WMI", Table_WMI);
            report.MakeReport();
        }
        #endregion
    }
}
