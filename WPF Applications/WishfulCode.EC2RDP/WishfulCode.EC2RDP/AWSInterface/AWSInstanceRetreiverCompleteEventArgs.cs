using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WishfulCode.EC2RDP.Model;

namespace WishfulCode.EC2RDP.AWSInterface
{
    public class AWSInstanceRetreiverCompleteEventArgs : EventArgs
    {
        public IList<Connection> Result { get; set; }

        public AWSInstanceRetreiverCompleteEventArgs()
        {
            
        }
    }
}
