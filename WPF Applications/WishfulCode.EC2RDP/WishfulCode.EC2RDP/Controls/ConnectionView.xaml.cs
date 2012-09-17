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
            InitializeComponent();
            this.Focusable = false;

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

            var displaySize = (Size)PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice.Transform(new Vector(this.ActualWidth, this.ActualHeight));
            connectionController = new RDPConnectionController
            {
                DesktopResolution = new Size(displaySize.Width,displaySize.Height),
                Model = ViewModel
            };

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

            this.GotFocus += new RoutedEventHandler(ConnectionView_GotFocus);
       
            connectionController.CreateControl();
            connectionController.Connect(); 

            //make sure all events fire
            System.Windows.Forms.Application.DoEvents();
        }


        void ConnectionView_GotFocus(object sender, RoutedEventArgs e)
        {
            //ensure our ConnectionView has focus
            connectionController.ConnectionView.Focus();
        }





    }
}
