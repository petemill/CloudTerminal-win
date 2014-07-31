using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WishfulCode.EC2RDP.Model;

namespace WishfulCode.EC2RDP.AWSInterface
{
    public class AWSLoadBalancerServersRetreiverCompleteEventArgs : EventArgs
    {
        public IList<LoadBalancer> Result { get; set; }

        public AWSLoadBalancerServersRetreiverCompleteEventArgs()
        {
            
        }

        public class LoadBalancer {
            public string LoadBalancerName {get; set;}
            public List<string> InstanceNames {get; set;}
        }
    }
}
