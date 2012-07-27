using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace WishfulCode.EC2RDP.Foundation
{
    public class Pair : INotifyPropertyChanged
    {
        private string key = "Key";
        public string Key
        {
            get { return key; }
            set
            {
                if (this.key != value)
                {
                    key = value;
                    NotifyPropertyChanged("Key");
                }
            }
        }


        private string value = "Value";
        public string Value
        {
            get { return value; }
            set
            {
                if (this.value != value)
                {
                    this.value = value;
                    NotifyPropertyChanged("Value");
                }
            }
        }

        public Pair() { }
        public Pair(string key, string value)
            : this()
        {
            Key = key;
            Value = value;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
