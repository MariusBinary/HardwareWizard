using System.Collections.Generic;
using HardwareWizard.Core.Helpers;
using OpenHardwareMonitor.Hardware;

namespace HardwareWizard.Core
{
    public class GraphicsCardData
    {
        public string Model { get; set; }
        public string Manufacturer { get; set; }
        public string ManufacturerId { get; set; }
        public string VideoProcessor { get; set; }
        public string VideoArchitecture { get; set; }
        public string VideoMemoryType { get; set; }
        public string BitsPerPixel { get; set; }
        public string HorizontalResolution { get; set; }
        public string VerticalResolution { get; set; }
        public string RefreshRate { get; set; }
        public string ScanMode { get; set; }
        public List<CoolingThermalData> Thermals { get; set; }
        public List<CoolingFanData> Fans { get; set; }
        public GraphCollection UsageCollection { get; set; }
        public IHardware Hardware { get; set; }
        public ISensor UsageSensor { get; set; }
        public ISensor MemorySensor { get; set; }
        public List<WMICollection> WMI { get; set; }

        public float? Usage
        {
            get
            {
                if (UsageSensor == null) return 0;
                return UsageSensor.Value.HasValue ? UsageSensor.Value.Value : 0;
            }
        }

        public float? Memory
        {
            get
            {
                if (MemorySensor == null) return 0;
                return MemorySensor.Value.HasValue ? MemorySensor.Value.Value : 0;
            }
        }

        public float? MaximumMemory
        {
            get
            {
                if (MemorySensor == null) return 1;
                float? max = MemorySensor.Max.HasValue ? MemorySensor.Max.Value : 1;
                return max > 100 ? max : 100;
            }
        }

        public GraphicsCardData()
        {
            this.WMI = new List<WMICollection>();
            UsageCollection = new GraphCollection();
            Thermals = new List<CoolingThermalData>();
            Fans = new List<CoolingFanData>();
        }
    }
}
