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
using System.Diagnostics;

namespace WishfulCode.EC2RDP.Controls
{
    /// <summary>
    /// Interaction logic for ConnectionView.xaml
    /// </summary>
    public partial class ConnectionView : UserControl
    {
        public ConnectionView()
        {
            InitializeComponent();this.Focusable = true;

            this.Loaded += new RoutedEventHandler(ConnectionView_Loaded); 
        }

        protected ConnectionViewModel ViewModel
        {
            get
            {
                return this.DataContext as ConnectionViewModel;
            }
        }

        IConnectionController connectionController;




        void ConnectionView_Loaded(object sender, RoutedEventArgs e)
        {
            //initialise connection view from appropriate controller
            //TODO: get view based on connection type
            connectionController = new RDPConnectionController
            {
                DesktopResolution = new Size(this.ActualWidth, this.ActualHeight),
                Model = ViewModel
            };

            System.Windows.Forms.Integration.WindowsFormsHost.EnableWindowsFormsInterop();
            

            ViewModel.DisconnectRequested += (s, se) =>
            {
                connectionController.Disconnect();
            };
            connectionController.Disconnected += (s, se) =>
            {
                ViewModel.OnDisconnected(se);
            };

            System.Windows.Forms.Panel container = new System.Windows.Forms.Panel();
            container.Dock = System.Windows.Forms.DockStyle.Fill;
            formHost.Child = container;

            var rdpConnection = connectionController.ConnectionView;

            rdpConnection.Dock = System.Windows.Forms.DockStyle.Fill;
            rdpConnection.Parent = container;
            
            rdpConnection.CreateControl();
            while (!rdpConnection.Created)
            {
                Thread.Sleep(10);
                System.Windows.Forms.Application.DoEvents();
            }

            this.GotFocus += new RoutedEventHandler(ConnectionView_GotFocus);
            //this.GotKeyboardFocus += new KeyboardFocusChangedEventHandler(ConnectionView_GotKeyboardFocus);
            this.PreviewGotKeyboardFocus += new KeyboardFocusChangedEventHandler(ConnectionView_PreviewGotKeyboardFocus);

            rdpConnection.GotFocus+=new EventHandler(rdpConnection_GotFocus);

            connectionController.CreateControl();
            connectionController.Connect();
            FocusHelper.Focus(formHost);
        }



        void rdpConnection_GotFocus(object sender, EventArgs e)
        {
            Trace.WriteLine("rdpConnection_GotFocus");
        }
        void ConnectionView_GotFocus(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine("ConnectionView_GotFocus");
            connectionController.ConnectionView.Focus();
        }

        void ConnectionView_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            e.Handled = true;
            Trace.WriteLine("ConnectionView_PreviewGotKeyboardFocus");
        }

    }
}
