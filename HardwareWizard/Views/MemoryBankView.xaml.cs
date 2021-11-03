using System.Windows.Controls;
using HardwareWizard.Core;

namespace HardwareWizard.Views
{
    public partial class MemoryBankView : UserControl
    {
        #region Variables
        private MemoryBankData bankData;
        #endregion

        #region Main
        public MemoryBankView(MemoryBankData bankData)
        {
            InitializeComponent();
            this.bankData = bankData;
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Tx_BankModel.Text = bankData.Model;
            Tx_BankManufacturer.Text = bankData.Manufacturer;

            Table_Informations.AddTag("Model", bankData.Model, 0);
            Table_Informations.AddTag("Manufacturer", bankData.Manufacturer, 0);
            Table_Informations.AddTag("Memory Type", bankData.MemoryType, 0);
            Table_Informations.AddTag("Form Factor", bankData.FormFactor, 0);
            Table_Informations.AddTag("Position", bankData.Position, 1);
            Table_Informations.AddTag("Channel", bankData.Channel, 1);
            Table_Informations.AddTag("Speed", bankData.Speed, 1);
            Table_Informations.AddTag("Capacity", bankData.Capacity, 1);

            // Aggiungere le proprietà WMI.
            for (int i = 0; i < bankData.WMI.Count; i++) {
                Table_WMI.AddTag(bankData.WMI[i].Property, bankData.WMI[i].Value, i % 2 == 0 ? 0 : 1);
            }
        }

        private void Btn_Report_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ReportHelper report = new ReportHelper("ram");
            report.AddToReport("Informations", Table_Informations);
            report.AddToReport("WMI", Table_WMI);
            report.MakeReport();
        }
        #endregion
    }
}
