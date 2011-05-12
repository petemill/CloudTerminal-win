using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;
using WishfulCode.EC2RDP.Properties;

namespace WishfulCode.EC2RDP.ViewModel
{
    public class AccountSettingsViewModel : ViewModelBase
    {
        public AccountSettingsViewModel()
        {
            AWSAccessKey = Settings.Default.AWSAccessKey;
            AWSSecretKey = Settings.Default.AWSSecretKey;
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

                var oldValue = _awsSecretKey;
                _awsSecretKey = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(AWSSecretKeyPropertyName);
            }
        }
    }
}
