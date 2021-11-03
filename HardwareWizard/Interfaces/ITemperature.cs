using HardwareWizard.Core;
using System.Collections.Generic;

namespace HardwareWizard.Interfaces
{
    public interface ITemperature
    {
        CoolingFanData[] GetHardwareFans();
        CoolingThermalGroup[] GetHardwareThermal();
    }
}
