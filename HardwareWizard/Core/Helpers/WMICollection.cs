using System;

namespace HardwareWizard.Core.Helpers
{
    public class WMICollection
    {
        public string Property { get; set; }
        public string Value { get; set; }

        public WMICollection(string property, string value)
        {
            this.Property = property;
            this.Value = value;
        }
    }
}
