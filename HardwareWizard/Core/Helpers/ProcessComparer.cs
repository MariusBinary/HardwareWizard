using System.Collections.Generic;

namespace HardwareWizard.Core.Helpers
{
    public class ProcessComparer : IEqualityComparer<ProcessesWatcherData>
    {
        public bool Equals(ProcessesWatcherData x, ProcessesWatcherData y)
        {
            return x.Process.Id == y.Process.Id;
        }
        public int GetHashCode(ProcessesWatcherData obj)
        {
            return obj.Process.Id.GetHashCode();
        }
    }
}
