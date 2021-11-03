using System.Collections.Generic;
using HardwareWizard.Core.Helpers;
using OpenHardwareMonitor.Hardware;

namespace HardwareWizard.Core
{
    public class StorageDriveData
    {
        public string Model { get; set; }
        public string Manufacturer { get; set; }
        public string Type { get; set; }
        public string Interface { get; set; }
        public string MediaType { get; set; }
        public string Location { get; set; }
        public string Identifier { get; set; }
        public string Serial { get; set; }
        public List<StoragePartitionData> Partitions { get; set; }
        public bool Ejectabled { get; set; }
        public long BlockSize { get; set; }
        public long Capacity { get; set; } 
        public long UsedSpace { get; set; } 
        public int Index { get; set; } 

        public List<WMICollection> WMI { get; set; }
        public List<CoolingThermalData> Thermals { get; set; }
        public List<CoolingFanData> Fans { get; set; }
        public IHardware Hardware { get; set; }

        public StorageDriveData()
        {
            this.WMI = new List<WMICollection>();
            this.Thermals = new List<CoolingThermalData>();
            this.Fans = new List<CoolingFanData>();
        }
    }
}
