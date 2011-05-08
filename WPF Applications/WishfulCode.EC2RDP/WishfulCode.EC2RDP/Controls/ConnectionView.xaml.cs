using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using WishfulCode.EC2RDP.ViewModel;

namespace WishfulCode.EC2RDP.Controls
{
    /// <summary>
    /// Interaction logic for ConnectionView.xaml
    /// </summary>
    public partial class ConnectionView : UserControl
    {
        public ConnectionView()
        {
            InitializeComponent();
            //create rdp object
            this.Loaded += new RoutedEventHandler(ConnectionView_Loaded);
        }

        protected ConnectionViewModel ViewModel
        {
            get
            {
                return this.DataContext as ConnectionViewModel;
            }
        }

        private void DisconnectRDP()
        {
            if (rdpConnection!=null && rdpConnection.Connected==1)
                rdpConnection.Disconnect();
        }

        AxMSTSCLib.AxMsRdpClient6 rdpConnection;

        void ConnectionView_Loaded(object sender, RoutedEventArgs e)
        {
            //create rdp control and setup view
            rdpConnection = new AxMSTSCLib.AxMsRdpClient6();
            ViewModel.DisconnectRequested += (s, se) =>
            {
                DisconnectRDP();
                (s as ConnectionViewModel).OnDisconnected(se);
            };
            System.Windows.Forms.Panel container = new System.Windows.Forms.Panel();
            container.Dock = System.Windows.Forms.DockStyle.Fill;
            formHost.Child = container;
            rdpConnection.Dock = System.Windows.Forms.DockStyle.Fill;
            rdpConnection.Parent = container;
            rdpConnection.CreateControl();
            
            while (!rdpConnection.Created)
            {
                Thread.Sleep(10);
                System.Windows.Forms.Application.DoEvents();
            }

            rdpConnection.FullScreenTitle = ViewModel.Name; ;

            rdpConnection.ConnectingText = "Connecting. to " + ViewModel.Name + " (" + ViewModel.Host + ")...";

            //set visual properties
            rdpConnection.Name = ViewModel.Name;
            rdpConnection.AdvancedSettings2.SmartSizing = true;
            rdpConnection.DesktopWidth = Convert.ToInt32(this.ActualWidth);
            rdpConnection.DesktopHeight = Convert.ToInt32(this.ActualHeight);
          

            rdpConnection.AdvancedSettings4.EnableAutoReconnect = true;
            rdpConnection.AdvancedSettings4.MaxReconnectAttempts = 3;
            //rdpConnection.AdvancedSettings.BitmapPeristence = true;
            rdpConnection.AdvancedSettings4.keepAliveInterval = 60000;

            //server properties
            rdpConnection.Server = ViewModel.Host;
            rdpConnection.UserName = "Administrator";
            rdpConnection.AdvancedSettings3.ClearTextPassword = "0nl1ne";

            rdpConnection.Connect();
        }
    }
}
