using System.Windows;
using System.Windows.Controls;
using HardwareWizard.Core;
using HardwareWizard.Interfaces;
using HardwareWizard.Models;

namespace HardwareWizard.Controls
{
    [TemplatePart(Name = "PART_StorageDrives", Type = typeof(ListView))]
    public class DashboardStorageControl : Control, IViewUpdate
    {
        #region Template
        private ListView m_storageDrives;
        #endregion

        #region Variables
        private StorageData storageDatabase;
        private bool isLoaded = false;
        #endregion

        #region Main
        static DashboardStorageControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DashboardStorageControl), 
                new FrameworkPropertyMetadata(typeof(DashboardStorageControl)));
        }

        public override void OnApplyTemplate()
        {
            m_storageDrives = Template.FindName("PART_StorageDrives", this) as ListView;
            base.OnApplyTemplate();
        }

        public DashboardStorageControl(StorageData storageDatabase)
        {
            this.storageDatabase = storageDatabase;
            this.Loaded += (s, e) => {
                foreach (StorageDriveData drive in storageDatabase.Drives)
                {
                    m_storageDrives.Items.Add(new DashboardDriveItemModel()
                    {
                        DriveData = drive,
                        Name = drive.Model,
                        TotalBytes = drive.Capacity,
                        UsedBytes = drive.UsedSpace,
                        Image = $"/HardwareWizard;component/Images/Shared/ic_drive_{drive.Location.ToLower()}_small.png",
                    });
                }
                isLoaded = true;
            };
        }
        #endregion

        #region IViewUpdate
        public void OnViewUpdate()
        {
            if (!isLoaded) return;

            foreach (StorageDriveData drive in storageDatabase.Drives)
            {
                foreach (DashboardDriveItemModel item in m_storageDrives.Items)
                {
                    if (item.Name == drive.Model)
                    {
                        item.UsedBytes = drive.UsedSpace;
                    }
                }
            }
        }
        #endregion
    }
}
