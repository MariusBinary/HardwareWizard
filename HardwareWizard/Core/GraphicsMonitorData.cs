using HardwareWizard.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareWizard.Core
{
    public class GraphicsMonitorData
    {
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string SerialNumber { get; set; }
        public List<WMICollection> WMI { get; set; }

        public GraphicsMonitorData()
        {
            this.WMI = new List<WMICollection>();
        }
    }
}
