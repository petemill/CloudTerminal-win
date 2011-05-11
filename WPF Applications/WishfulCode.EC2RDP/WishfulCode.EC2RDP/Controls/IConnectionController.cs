using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows;
using WishfulCode.EC2RDP.ViewModel;

namespace WishfulCode.EC2RDP.Controls
{
    public interface IConnectionController
    {
        Control ConnectionView { get; }
        void Connect();
        void Disconnect();
        ConnectionViewModel Model { set; }
        Size DesktopResolution { set; }
        void CreateControl();
        event EventHandler Disconnected;
    }
}
