using GalaSoft.MvvmLight;
using System.Windows.Input;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Command;
using System;
using WishfulCode.EC2RDP.Model;

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
        public ConnectionViewModel WithConnection(Connection data)
        {
            this.Host = data.Host;
            this.Name = data.Name;
            this.Id = data.Id;
            
            return this;
        }


        /// <summary>
        /// Initializes a new instance of the ConnectionViewModel class.
        /// </summary>
        public ConnectionViewModel()
        {
            DisconnectCommand = new RelayCommand(() =>
            {
                //disconnect has been requested, fire event
                OnDisconnectRequested(null);
                //if there is a handler for disconnected-requested, let it fire disconnect
                if (DisconnectRequested == null)
                {
                    OnDisconnected(null);
                }
            }
            );
        }

        public ICommand DisconnectCommand { get; set; }

        public event EventHandler DisconnectRequested;
        public event EventHandler Disconnected;

        public void OnDisconnected(EventArgs e)
        {
            if (Disconnected != null)
            {
                Disconnected(this, e);
            }
        }

        protected void OnDisconnectRequested(EventArgs e)
        {
            if (DisconnectRequested != null)
            {
                DisconnectRequested(this, e);
            }
        }

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

        /// <summary>
        /// The <see cref="Id" /> property's name.
        /// </summary>
        public const string IdPropertyName = "Id";

        private string _id = String.Empty;

        /// <summary>
        /// Gets the Id property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public string Id
        {
            get
            {
                return _id;
            }

            set
            {
                if (_id == value)
                {
                    return;
                }

                var oldValue = _id;
                _id = value;

              

                // Update bindings, no broadcast
                RaisePropertyChanged(IdPropertyName);

              
            }
        }

        public bool Equals(ConnectionViewModel other)
        {
            return other.Host.Equals(this.Host);
        }
    }
}