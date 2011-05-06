using GalaSoft.MvvmLight;
using System.Windows.Input;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Command;
using System;

namespace WishfulCode.EC2RDP.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm/getstarted
    /// </para>
    /// </summary>
    public class ConnectionViewModel : ViewModelBase, IEquatable<ConnectionViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the ConnectionViewModel class.
        /// </summary>
        public ConnectionViewModel()
        {
            TestCommand = new RelayCommand(() => Name = "Enter");
        }

        public ICommand TestCommand { get; set; }


        /// <summary>
        /// The <see cref="Name" /> property's name.
        /// </summary>
        public const string NamePropertyName = "Name";

        private string _name = null;

        /// <summary>
        /// Gets the Name property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                if (_name == value)
                {
                    return;
                }

                var oldValue = _name;
                _name = value;

                // Update bindings, no broadcast
                //RaisePropertyChanged(NamePropertyName);

                // Update bindings and broadcast change using GalaSoft.MvvmLight.Messenging
                RaisePropertyChanged(NamePropertyName, oldValue, value, true);
            }
        }

        /// <summary>
        /// The <see cref="Host" /> property's name.
        /// </summary>
        public const string HostPropertyName = "Host";

        private string _host = null;

        /// <summary>
        /// Gets the Host property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public string Host
        {
            get
            {
                return _host;
            }

            set
            {
                if (_host == value)
                {
                    return;
                }

                var oldValue = _host;
                _host = value;



                // Update bindings, no broadcast
                //RaisePropertyChanged(HostPropertyName);

                // Update bindings and broadcast change using GalaSoft.MvvmLight.Messenging
                RaisePropertyChanged(HostPropertyName, oldValue, value, true);
            }
        }

        public bool Equals(ConnectionViewModel other)
        {
            return other.Host.Equals(this.Host);
        }
    }
}