using System.Threading.Tasks;
using System.Windows.Controls;
using HardwareWizard.Controls;
using HardwareWizard.Core;

namespace HardwareWizard.Views
{
    public partial class StorageView : UserControl
    {
        #region Variables
        private StorageData storageDatabase;
        #endregion

        #region Main
        public StorageView(StorageData storageDatabase)
        {
            InitializeComponent();
            this.storageDatabase = storageDatabase;
            this.Loaded += (async (s, e) => {
                await Task.Delay(250);
                Panel_InternalDrives.OnChildResized();
                Panel_ExternalDrives.OnChildResized();
            });

            foreach (StorageDriveData drive in storageDatabase.Drives)
            {
                var item = (new GenericItemControl()
                {
                    Content = drive,
                    Title = drive.Model,
                    Description = drive.Manufacturer,
                    Image = $"/HardwareWizard;component/Images/Shared/ic_drive_{drive.Location.ToLower()}.png",
                    TargetView = 10
                });

                if (drive.Location.ToLower().Equals("internal")) {
                    Panel_InternalDrives.AddChild(item);
                } else {
                    Panel_ExternalDrives.AddChild(item);
                }
            }
        }
        #endregion
    }
}
