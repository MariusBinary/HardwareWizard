using System.ComponentModel;

namespace HardwareWizard.Models
{
    class LegendItemModel : INotifyPropertyChanged
    {
        private string _fill = "";
        public string Fill
        {
            get { return _fill; }
            set
            {
                _fill = value;
                RaisePropertyChanged("Fill");
            }
        }

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

        private string _usage = "";
        public string Usage
        {
            get { return _usage; }
            set
            {
                _usage = value;
                RaisePropertyChanged("Usage");
            }
        }

        private string _details = "";
        public string Details
        {
            get { return _details; }
            set
            {
                _details = value;
                RaisePropertyChanged("Details");
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
