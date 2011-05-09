using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WishfulCode.EC2RDP.Model
{
    public class Connection
    {
        public string Host { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
        public ConnectionProtocol Protocol { get; set; }
    }
}
