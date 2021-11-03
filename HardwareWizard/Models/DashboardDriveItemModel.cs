using HardwareWizard.Core;
using System.ComponentModel;

namespace HardwareWizard.Models
{
    class DashboardDriveItemModel : INotifyPropertyChanged
    {
        private string _name = "";
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged("Name");
           }
        }

        private int _index = 0;
        public int Index
        {
            get { return _index; }
            set
            {
                _index = value;
                RaisePropertyChanged("Index");
            }
        }

        private string _link = "";
        public string Link
        {
            get { return _link; }
            set
            {
                _link = value;
                RaisePropertyChanged("Link");
            }
        }

        private string _image = "";
        public string Image
        {
            get { return _image; }
            set
            {
                _image = value;
                RaisePropertyChanged("Image");
            }
        }


        private long _usedBytes = 0;
        public long UsedBytes
        {
            get { return _usedBytes; }
            set
            {
                _usedBytes = value;
                Value = ((double)UsedBytes / (double)TotalBytes);
                Percentage = $"{(UsedBytes * 100) / TotalBytes}%";
                UsageDetails = $"{Core.Utils.SizeSuffix((ulong)UsedBytes)} used out of {Core.Utils.SizeSuffix((ulong)TotalBytes)}";
                RaisePropertyChanged("UsedBytes");
            }
        }

        public StorageDriveData DriveData { get; set; }
        public int TargetView { get { return 10; } }

        private long _totalBytes = 0;
        public long TotalBytes
        {
            get { return _totalBytes; }
            set
            {
                _totalBytes = value;
                RaisePropertyChanged("TotalBytes");
            }
        }

        private double _value = 0.0;
        public double Value
        {
            get { return _value; }
            set
            {
                _value = value;
                RaisePropertyChanged("Value");
            }
        }
        private string _percentage = "-";
        public string Percentage
        {
            get { return _percentage; }
            set
            {
                _percentage = value;
                RaisePropertyChanged("Percentage");
            }
        }

        private string _usageDetails = "-";
        public string UsageDetails
        {
            get { return _usageDetails; }
            set
            {
                _usageDetails = value;
                RaisePropertyChanged("UsageDetails");
            }
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
