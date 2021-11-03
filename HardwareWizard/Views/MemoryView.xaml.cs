using System.Threading.Tasks;
using System.Windows.Controls;
using HardwareWizard.Controls;
using HardwareWizard.Core;

namespace HardwareWizard.Views
{
    public partial class MemoryView : UserControl
    {
        #region Variables
        private MemoryData memoryData;
        #endregion

        #region Main
        public MemoryView(MemoryData memoryData)
        {
            InitializeComponent();
            this.memoryData = memoryData;
            this.Loaded += (async (s, e) => {
                await Task.Delay(250);
                Panel_Slots.OnChildResized();
            });

            foreach (MemoryBankData bank in memoryData.Banks)
            {
                Panel_Slots.AddChild(new GenericItemControl()
                {
                    Title = bank.Model,
                    Description = bank.Manufacturer,
                    Content = bank,
                    TargetView = 5,
                    Image = $"/HardwareWizard;component/Images/Shared/ic_ram_slot.png",
                });
            }
        }
        #endregion
    }
}
