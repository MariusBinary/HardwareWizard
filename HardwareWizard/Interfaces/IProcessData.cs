using HardwareWizard.Core;
using System.Collections.Generic;

namespace HardwareWizard.Interfaces
{
    public interface IProcessData
    {
        List<ProcessesWatcherData> Processes { get; set; }
    }
}
