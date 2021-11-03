using OpenHardwareMonitor.Hardware;

namespace HardwareWizard.Core
{
    public class CoolingFanData
    {
        public string Name { get; set; }
        public ISensor Sensor { get; set; }
        public GraphCollection SpeedData { get; set; } = new GraphCollection();

        public float? Speed 
        { 
            get 
            {
                return Sensor.Value.HasValue ? Sensor.Value.Value : 0;
            }
        }

        public float? Maximum
        {
            get
            {
                float? max = Sensor.Max.HasValue ? Sensor.Max.Value : 0;
                return max > 3000 ? max : 3000;
            }
        }

        public float? Minimum
        {
            get
            {
                return Sensor.Min.HasValue ? Sensor.Min.Value : 0;
            }
        }

        public CoolingFanData()
        {
            SpeedData = new GraphCollection();
        }
    }
}
