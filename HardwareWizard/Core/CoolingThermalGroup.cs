using System.Collections.Generic;

namespace HardwareWizard.Core
{
    public class CoolingThermalGroup
    {
        public string Name { get; set; }
        public int HeaderIndex { get; set; }
        public bool IsHeaderVisible { get; set; }
        public bool HasHeader { get; set; }
        public bool IsGroup { get; set; }
        public CoolingThermalData[] ThermalDatas { get; set; }

        public CoolingThermalGroup()
        {
            //ThermalDatas = new List<CoolingThermalData>();
        }
    }
}
