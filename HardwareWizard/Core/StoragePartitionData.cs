using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareWizard.Core
{
    public class StoragePartitionData
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string DeviceID { get; set; }
        public string PartitionName { get; set; }
        public long Capacity { get; set; }
        public long UsedSpace { get; set; }
    }
}
