using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;
using WishfulCode.EC2RDP.Properties;
using System.Collections.ObjectModel;
using WishfulCode.EC2RDP.Foundation;
using System.ComponentModel;

namespace WishfulCode.EC2RDP.ViewModel
{
    public class AccountSettingsViewModel : ViewModelBase
    {
        public AccountSettingsViewModel()
        {
            AWSAccessKey = Settings.Default.AWSAccessKey;
            AWSSecretKey = Settings.Default.AWSSecretKey;
            DefaultRemoteUsername = Settings.Default.DefaultRemoteUsername ?? _defaultRemoteUsername;
            if (Settings.Default.PrivateKeys.Count != 0)
            {
                gridData = new BindingList<Pair>(Settings.Default.PrivateKeys.Select(item => new Pair(item.Key, item.Value)).ToList());
            }
            else
            {
                gridData  = new BindingList<Pair>(new Pair[] { new Pair()  });
            }
            gridData.ListChanged += (s, e) =>
                {
                    Settings.Default.PrivateKeys.Clear();
                    gridData.ToList().ForEach(item => {
                        if (!Settings.Default.PrivateKeys.ContainsKey(item.Key))
                        Settings.Default.PrivateKeys.Add(item.Key, item.Value);
                    });
                    Settings.Default.Save();
                };

        }

      

        /// <summary>
        /// The <see cref="AWSAccessKey" /> property's name.
        /// </summary>
        public const string AWSAccessKeyPropertyName = "AWSAccessKey";

        private string _awsAccessKey = String.Empty;

        /// <summary>
        /// Gets the AWSAccessKey property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public string AWSAccessKey
        {
            get
            {
                return _awsAccessKey;
            }

            set
            {
                if (_awsAccessKey == value)
                {
                    return;
                }

                Settings.Default.AWSAccessKey = value;
                Settings.Default.Save();
                var oldValue = _awsAccessKey;
                _awsAccessKey = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(AWSAccessKeyPropertyName);

            }
        }

        /// <summary>
        /// The <see cref="AWSSecretKey" /> property's name.
        /// </summary>
        public const string AWSSecretKeyPropertyName = "AWSSecretKey";

        private string _awsSecretKey = String.Empty;

        /// <summary>
        /// Gets the AWSSecretKey property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public string AWSSecretKey
        {
            get
            {
                return _awsSecretKey;
            }

            set
            {
                if (_awsSecretKey == value)
                {
                    return;
                }

                Settings.Default.AWSSecretKey = value;
                Settings.Default.Save();
                var oldValue = _awsSecretKey;
                _awsSecretKey = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(AWSSecretKeyPropertyName);
            }
        }

        private string _defaultRemoteUsername = "Administrator";
        public string DefaultRemoteUsername
        {
            get
            {
                return _defaultRemoteUsername;
            }
            set
            {
                if (_defaultRemoteUsername == value)
                {
                    return;
                }

                Settings.Default.DefaultRemoteUsername = value;
                Settings.Default.Save();
                var oldValue = _defaultRemoteUsername;
                _defaultRemoteUsername = value;

                RaisePropertyChanged("DefaultRemoteUsername");
            }
        }

        private bool _defaultUseApiAdminPwd = false;
        public bool DefaultUseApiAdminPwd
        {
            get
            {
                return _defaultUseApiAdminPwd;
            }
            set
            {
                Settings.Default.DefaultUseApiAdminPwd = value;
                Settings.Default.Save();
                var oldValue = _defaultUseApiAdminPwd;
                _defaultUseApiAdminPwd = value;
                RaisePropertyChanged("DefaultUseApiAdminPwd");
            }
        }

        private BindingList<Pair> gridData;
        public BindingList<Pair> GridData
        {
            get { return gridData; }
        }
    }
}
