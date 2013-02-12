using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WishfulCode.EC2RDP.AWSInterface
{
    public class AWSAdministratorPasswordRetreiverCompleteEventArgs : EventArgs
    {
        public string Password { get; set; }
        public Exception Exception { get; set; }

        public AWSAdministratorPasswordRetreiverCompleteEventArgs() { }
    }
}
