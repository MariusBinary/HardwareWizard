using OpenHardwareMonitor.Hardware;

namespace HardwareWizard.Core
{
    public class CoolingThermalData
    {
        public string Name { get; set; }
        public GraphCollection TemperatureData { get; set; }
        public ISensor Sensor { get; set; }

        public float? Temperature
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
                return max > 100 ? max : 100;
            }
        }

        public float? Minimum
        {
            get
            {
                return Sensor.Min.HasValue ? Sensor.Min.Value : 0;
            }
        }

        public CoolingThermalData()
        {
            TemperatureData = new GraphCollection();
            TemperatureData.IsFillBasedOnValue = true;
        }
    }
}
