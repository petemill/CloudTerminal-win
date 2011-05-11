using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WishfulCode.mRDP.AWSInterface;

namespace WishfulCode.EC2RDP.AWSInterface
{
    public class CloudWatchInstanceDataRetreiver
    {
        public string AWSAccessKey { get; set; }
        public string AWSSecretKey { get; set; }
        public Region EC2Region { get; set; }
        

    }
}
