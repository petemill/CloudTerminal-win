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

        void ConnectionView_Loaded(object sender, RoutedEventArgs e)
        {
           
            var rdpConnection = new AxMSTSCLib.AxMsRdpClient6();
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

            rdpConnection.FullScreenTitle = "Connection-Name";

            rdpConnection.ConnectingText = "Connecting...";

            //set visual properties
            // rdpConnection.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Top;
            rdpConnection.Name = "Test";
            rdpConnection.AdvancedSettings2.SmartSizing = true;
            rdpConnection.DesktopWidth = Convert.ToInt32(this.ActualWidth);
            rdpConnection.DesktopHeight = Convert.ToInt32(this.ActualHeight);
            //rdpConnection.Width = 800;
            //rdpConnection.Height = 600;
            rdpConnection.FullScreenTitle = "Test";

            rdpConnection.AdvancedSettings4.EnableAutoReconnect = true;
            rdpConnection.AdvancedSettings4.MaxReconnectAttempts = 3;
            //rdpConnection.AdvancedSettings.BitmapPeristence = true;
            rdpConnection.AdvancedSettings4.keepAliveInterval = 60000;

            //server properties
            rdpConnection.Server = "ec2-46-51-161-165.eu-west-1.compute.amazonaws.com";
            rdpConnection.UserName = "Administrator";
            rdpConnection.AdvancedSettings3.ClearTextPassword = "0nl!ne";

            rdpConnection.Connect();
        }
    }
}
