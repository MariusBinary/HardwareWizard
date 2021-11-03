using System.ComponentModel;

namespace HardwareWizard.Models
{
    public class DetailsTableItemModel : INotifyPropertyChanged
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

        private string _value = "";
        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
                RaisePropertyChanged("Value");
            }
        }

        private int _column = 0;
        public int Column
        {
            get { return _column; }
            set
            {
                _column = value;
                RaisePropertyChanged("Column");
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
