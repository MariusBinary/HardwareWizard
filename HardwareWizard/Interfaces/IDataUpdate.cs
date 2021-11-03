using HardwareWizard.Core;
using OpenHardwareMonitor.Hardware;

namespace HardwareWizard.Interfaces
{
    public interface IDataUpdate
    {
        void OnHardwareAdded(IHardware hardware);
        void OnHardwareRemoved(IHardware hardware);
        void OnHardwareUpdate(HardwareUpdateType updateType);
    }
}
