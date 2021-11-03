using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using HardwareWizard.Core;
using HardwareWizard.Models;

namespace HardwareWizard.Views
{
    public partial class StorageDriveView : UserControl
    {
        #region Main
        private StorageDriveData driveData;
        #endregion

        #region Main
        public StorageDriveView(StorageDriveData driveData)
        {
            InitializeComponent();
            this.driveData = driveData;
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Tx_DriveName.Text = driveData.Model;
            Tx_DriveManufacturer.Text = driveData.Manufacturer;
            Img_DriveIcon.Source = new BitmapImage(new Uri(
                $"pack://application:,,,/HardwareWizard;component/Images/Shared/ic_drive_{driveData.Location}.png"));

            Table_Informations.AddTag("Manufacturer", driveData.Manufacturer, 0);
            Table_Informations.AddTag("Type", driveData.MediaType, 0);
            Table_Informations.AddTag("Capacity", Utils.SizeSuffix((ulong)driveData.Capacity), 0);
            Table_Informations.AddTag("Interface", driveData.Interface, 0);
            Table_Informations.AddTag("Location", driveData.Location, 0);
            Table_Informations.AddTag("Partitions", driveData.Partitions.Count.ToString(), 1);
            Table_Informations.AddTag("Serial", driveData.Serial, 1);

            foreach (StoragePartitionData partition in driveData.Partitions)
            {
                List_Contents.Items.Add(new StoragePartitionModel { 
                    Name = partition.Name,
                    Value = Utils.SizeSuffix((ulong)partition.Capacity)
                });
            }

            // Aggiungere le proprietà WMI.
            for (int i = 0; i < driveData.WMI.Count; i++) {
                Table_WMI.AddTag(driveData.WMI[i].Property, driveData.WMI[i].Value, i % 2 == 0 ? 0 : 1);
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
