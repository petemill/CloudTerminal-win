using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.ComponentModel;
using WishfulCode.mRDP.AWSInterface;
using Amazon.EC2;
using Amazon;
using Amazon.EC2.Model;
using WishfulCode.EC2RDP.Model;
using Region = WishfulCode.mRDP.AWSInterface.Region;

namespace WishfulCode.EC2RDP.AWSInterface
{
    public class AWSInstanceRetreiver
    {
        private BackgroundWorker worker;

        public string AWSAccessKey { get; set; }
        public string AWSSecretKey { get; set; }
        public Region EC2Region { get; set; }
        
        
        public delegate void InstanceRetreiveCompletedEventHandler(AWSInstanceRetreiver sender, AWSInstanceRetreiverCompleteEventArgs e);
        public event InstanceRetreiveCompletedEventHandler Completed;
        public virtual void OnCompleted(AWSInstanceRetreiverCompleteEventArgs e)
        {
            if (Completed != null)
                Completed(this, e);
        }

        
        public AWSInstanceRetreiver()
        {
            worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            
        }

        public void FetchAsync()
        {
            worker.RunWorkerAsync();
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error == null && !e.Cancelled && e.Result != null)
            {
                OnCompleted(new AWSInstanceRetreiverCompleteEventArgs { Result = e.Result as IList<Connection> });
            }
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            //validate args
            if (String.IsNullOrEmpty(AWSAccessKey) || String.IsNullOrEmpty(AWSSecretKey))
            {
                e.Result = null;
                return;
            }

            //ec2 client
            var ec2 = AWSClientFactory.CreateAmazonEC2Client(AWSAccessKey,
                                                              AWSSecretKey,
                                                              new AmazonEC2Config().WithServiceURL(RegionHelper.EC2EndpointForRegion(EC2Region))
                                                              );

            //get instances
            IEnumerable<RunningInstance> instances;
            try
            {
                var response = ec2.DescribeInstances(new Amazon.EC2.Model.DescribeInstancesRequest());

                //handle no data
                if (!response.IsSetDescribeInstancesResult())
                {
                    e.Result = null;
                    return;
                }

                instances = response.DescribeInstancesResult.Reservation.SelectMany(res => res.RunningInstance);
            }
            catch (Exception ex)
            {
                //handle error retreiving data
                e.Result = null;
                //TODO: log exception

                return;
            }

            //parse data
            e.Result = instances.Select(instance => new Connection
                {
                    Host = instance.PublicDnsName,
                    Id = instance.InstanceId,
                    Name = GetInstanceFriendlyName(instance),
                    Protocol = String.Equals(instance.Platform, "windows", StringComparison.InvariantCultureIgnoreCase) ? ConnectionProtocol.RDP : ConnectionProtocol.SSH,
                    HexIp = instance.InstanceState.Name != "running" ? "" : GetHexIPAddress(instance.PrivateIpAddress)
                }).ToList();

        }

        public static string GetInstanceFriendlyName(RunningInstance ins)
        {
            Tag nameTag = ins.Tag.Where(tag => tag.Key.ToLower() == "name").FirstOrDefault();
            if (nameTag != null)
                return nameTag.Value;
            else
            {
                return ins.InstanceId;
            }
        }

        private static string GetHexIPAddress(string ip)
        {
            var hexIp = "IP-";
            foreach (var value in ip.Split('.'))
            {
                hexIp += int.Parse(value).ToString("X2");
            }
            return hexIp;
        }
        
    }
}
