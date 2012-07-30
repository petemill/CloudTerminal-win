using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
            this.HexIp = data.HexIp;
            this.KeyName = data.KeyName;

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

            Properties.Settings.Default.SettingsSaving += (s, e) =>
                {
                    if (!String.IsNullOrWhiteSpace(KeyName))
                        HavePrivateKeyStored = Properties.Settings.Default.PrivateKeys.ContainsKey(KeyName);
                };

        }

        public ICommand DisconnectCommand { get; set; }

        public event EventHandler DisconnectRequested;
        public event EventHandler Disconnected;

        public void OnDisconnected(EventArgs e)
        {
            if (Disconnected != null)
            {
                Disconnected(this, e);
                IsDisconnected = true;
            }
        }

        protected void OnDisconnectRequested(EventArgs e)
        {
            if (DisconnectRequested != null)
            {
                DisconnectRequested(this, e);
            }
        }

        public IEnumerable<DataPoint> CPUData
        {
            set
            {
                //right now, this is what we support for the status image, though this may change in future
                //  which is why we separate this logic from the ImageSource StatusImage property.
                
                //validate
                if (value == null)
                    return;

                //translate to google chart image url
                const string chartBase = "https://chart.googleapis.com/chart?chs=60x38&cht=ls&chm=B,76A4FB,0,0,0&chco=0077CC&chd=t:";
                var dataPart = String.Join(",", value.Select(data => data.Value));
                var googleChartUri = new Uri(String.Concat(chartBase, dataPart));
                
                //set status image to load from this url
                StatusImage = new BitmapImage(googleChartUri);

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


                //load some connection settings (perhaps put this in a layer up)
                if (Properties.Settings.Default.ConnectionSettingsRemoteDrives.ContainsKey(value))
                {
                    bool fromSettings;
                    if (Boolean.TryParse(Properties.Settings.Default.ConnectionSettingsRemoteDrives[value], out fromSettings))
                    {
                        _connectWithMappedDrives = fromSettings;
                    }
                }
                if (Properties.Settings.Default.ConnectionSettingsUseApiAdminPwd.ContainsKey(value))
                {
                    bool fromSettings;
                    if (Boolean.TryParse(Properties.Settings.Default.ConnectionSettingsUseApiAdminPwd[value], out fromSettings))
                    {
                        _useApiAdminPwd = fromSettings;
                    }
                }


            }
        }

        /// <summary>
        /// The <see cref="Id" /> property's name.
        /// </summary>
        public const string HexIpPropertyName = "HexIp";

        private string _hexIp = String.Empty;

        /// <summary>
        /// Gets the Id property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public string HexIp
        {
            get
            {
                return _hexIp;
            }

            set
            {
                if (_hexIp == value)
                {
                    return;
                }

                var oldValue = _hexIp;
                _hexIp = value;



                // Update bindings, no broadcast
                RaisePropertyChanged(HexIpPropertyName);


            }
        }

        private string _keyName = null;
        public string KeyName
        {
            get
            {
                return _keyName;
            }
            set
            {
                if (_keyName == value)
                {
                    return;
                }

                var oldValue = _keyName;
                _keyName = value;
                RaisePropertyChanged("KeyName");

                HavePrivateKeyStored = Properties.Settings.Default.PrivateKeys.ContainsKey(value);
              
            }
        }

        private bool _havePrivateKeyStored = false;
        public bool HavePrivateKeyStored
        {
            get
            {
                return _havePrivateKeyStored;
            }
            set
            {
                if (_havePrivateKeyStored == value)
                    return;

                var oldValue = _havePrivateKeyStored;
                _havePrivateKeyStored = value;
                RaisePropertyChanged("HavePrivateKeyStored");
                RaisePropertyChanged("UseApiAdminPwd");
            }
        }


        private bool _connectWithMappedDrives = false;

        public bool ConnectWithMappedDrives
        {
            get
            {
                return _connectWithMappedDrives;
            }
            set
            {
                if (_connectWithMappedDrives == value)
                {
                    return;
                }

                var oldValue = _connectWithMappedDrives;
                _connectWithMappedDrives = value;

                RaisePropertyChanged("ConnectWithMappedDrives");

                //save to config (consider putting a level up)
                Properties.Settings.Default.ConnectionSettingsRemoteDrives[Id] = value.ToString();
                Properties.Settings.Default.Save();
            }
        }

       private bool _useApiAdminPwd = false;

        public bool UseApiAdminPwd
        {
            get
            {
                return (HavePrivateKeyStored) ? _useApiAdminPwd : false;
            }
            set
            {
                if (_useApiAdminPwd == value)
                {
                    return;
                }

                var oldValue = _useApiAdminPwd;
                _useApiAdminPwd = value;

                RaisePropertyChanged("UseApiAdminPwd");

                //save to config (consider putting a level up)
                Properties.Settings.Default.ConnectionSettingsUseApiAdminPwd[Id] = value.ToString();
                Properties.Settings.Default.Save();
            }
        }


        private bool _isDisconnected = true;
        public bool IsDisconnected
        {
            get
            {
                return _isDisconnected;
            }
            set
            {
                if (_isDisconnected == value)
                {
                    return;
                }

                var oldValue = _isDisconnected;
                _isDisconnected = value;

                RaisePropertyChanged("IsDisconnected");
            }
        }

        /// <summary>
        /// The <see cref="StatusImage" /> property's name.
        /// </summary>
        public const string StatusImagePropertyName = "StatusImage";

        private ImageSource _statusImage = null;

        /// <summary>
        /// Gets the StatusImage property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public ImageSource StatusImage
        {
            get
            {
                return _statusImage;
            }

            set
            {
                if (_statusImage == value)
                {
                    return;
                }

                var oldValue = _statusImage;
                _statusImage = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(StatusImagePropertyName);
            }
        }


        public bool Equals(ConnectionViewModel other)
        {
            return other.Host.Equals(this.Host);
        }

        public override int GetHashCode()
        {
            return Host.GetHashCode();
            
        }
    }

}