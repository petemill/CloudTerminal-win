using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WishfulCode.EC2RDP.ViewModel;
using System.ComponentModel;
using System.Windows;
using System.Diagnostics;
using WishfulCode.EC2RDP.AWSInterface;
using WishfulCode.EC2RDP.Properties;
using WishfulCode.mRDP.AWSInterface;

namespace WishfulCode.EC2RDP.Controls
{
    public class RDPConnectionController : IConnectionController
    {
        AxMSTSCLib.AxMsRdpClient5 rdpConnection;


        public System.Windows.Forms.Control ConnectionView
        {
            get
            {
                if (rdpConnection == null)
                    rdpConnection = new AxMSTSCLib.AxMsRdpClient5();
                return rdpConnection;
            }
        
        }

        public void Connect()
        {
            //get admin pwd if needed and available
            if (Model.UseApiAdminPwd)
            {
                var privateKey = Settings.Default.PrivateKeys[Model.KeyName];
                if (privateKey != null)
                {
                    var pwdWorker = new AWSAdministratorPasswordRetreiver { AWSAccessKey = Settings.Default.AWSAccessKey, AWSSecretKey = Settings.Default.AWSSecretKey, EC2Region = Region.EU, InstanceId = Model.Id, PrivateKey = privateKey };
                    pwdWorker.Completed += (s, e) =>
                    {
                        rdpConnection.BeginInvoke(new Action(() =>
                        {
                            rdpConnection.UserName = "Administrator";
                            rdpConnection.AdvancedSettings2.ClearTextPassword = e.Password;
                            rdpConnection.Connect();
                        }));
                    };
                    pwdWorker.FetchAsync();
                    return;
                }
            }
            rdpConnection.BeginInvoke(new Action(() =>
            {
                rdpConnection.Connect();
            }));
        }

        public void Disconnect()
        {
            if (rdpConnection != null && rdpConnection.Connected == 1)
            {
                try
                {
                    rdpConnection.Disconnect();
                }
                catch (Exception ex)
                {
                    ErrorHandling.LogError(ex,"RDPConnectionController_Disconnect");
                }
            }
        }

        public ConnectionViewModel Model {get;set;}

        public System.Windows.Size DesktopResolution {get;set;}

        public void CreateControl()
        {
            if (rdpConnection == null)
                rdpConnection = new AxMSTSCLib.AxMsRdpClient5();

            rdpConnection.OnDisconnected += new AxMSTSCLib.IMsTscAxEvents_OnDisconnectedEventHandler(rdpConnection_OnDisconnected);

            rdpConnection.FullScreenTitle = Model.Name;
            rdpConnection.ConnectingText = "Connecting to " + Model.Name + " (" + Model.Host + ")...";

            //set visual properties
            rdpConnection.AdvancedSettings6.SmartSizing = true;
            rdpConnection.DesktopWidth = Convert.ToInt32(DesktopResolution.Width);
            rdpConnection.DesktopHeight = Convert.ToInt32(DesktopResolution.Height);


            //set connection properties
            rdpConnection.AdvancedSettings4.EnableAutoReconnect = true;
            rdpConnection.AdvancedSettings4.MaxReconnectAttempts = 3;
            rdpConnection.AdvancedSettings4.keepAliveInterval = 60000;

            //performance flags
            rdpConnection.AdvancedSettings2.PerformanceFlags = 74;

            rdpConnection.AdvancedSettings2.GrabFocusOnConnect = true;
            rdpConnection.AdvancedSettings3.EnableAutoReconnect = true;
            rdpConnection.AdvancedSettings5.AuthenticationLevel = 0;
            rdpConnection.AdvancedSettings5.EncryptionEnabled = 1;
            rdpConnection.AdvancedSettings2.overallConnectionTimeout = 20;
            rdpConnection.AdvancedSettings2.BitmapPeristence = -1;
            rdpConnection.AdvancedSettings2.RedirectDrives = Model.ConnectWithMappedDrives;

            rdpConnection.ColorDepth = 24;
            rdpConnection.SecuredSettings2.AudioRedirectionMode = 2;

            //server properties
            rdpConnection.Server = Model.Host;
            rdpConnection.UserName = Properties.Settings.Default.DefaultRemoteUsername ?? "Administrator";
        }

        void rdpConnection_OnDisconnected(object sender, AxMSTSCLib.IMsTscAxEvents_OnDisconnectedEvent e)
        {
            OnDisconnected(null);
        }


        public enum RDPColors
        {
            [Description("16 Bit")]
            Colors16Bit = 0x10,
            [Description("24 Bit")]
            Colors24Bit = 0x18,
            [Description("256 Colors")]
            Colors256 = 8,
            [Description("32 Bit")]
            Colors32Bit = 0x20
        }

        private enum RDPPerformanceFlags
        {
            [Description("Disable Cursor blinking")]
            DisableCursorBlinking = 40,
            [Description("Disable Cursor Shadow")]
            DisableCursorShadow = 20,
            [Description("Disable Full Window drag")]
            DisableFullWindowDrag = 2,
            [Description("Disable Menu Animations")]
            DisableMenuAnimations = 4,
            [Description("Disable Themes")]
            DisableThemes = 8,
            [Description("Disable Wallpaper")]
            DisableWallpaper = 1
        }


        public event EventHandler Disconnected;
        protected virtual void OnDisconnected(EventArgs e)
        {
            if (Disconnected != null)
                Disconnected(this, e);
        }
    }
}
