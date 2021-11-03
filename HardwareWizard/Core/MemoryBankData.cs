using System.Collections.Generic;
using HardwareWizard.Core.Helpers;

namespace HardwareWizard.Core
{
    public class MemoryBankData
    {
        public string Model { get; set; }
        public string Manufacturer { get; set; }
        public string MemoryType { get; set; }
        public string FormFactor { get; set; }
        public string Position { get; set; }
        public string Speed { get; set; }
        public string Capacity { get; set; }
        public string Channel { get; set; }
        public List<WMICollection> WMI { get; set; }

        public MemoryBankData()
        {
            this.WMI = new List<WMICollection>();
        }
    }
}
