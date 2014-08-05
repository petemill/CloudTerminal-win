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
using Amazon.ElasticLoadBalancing;
using System.IO;
using Amazon.ElasticLoadBalancing.Model;

namespace WishfulCode.EC2RDP.AWSInterface
{
    public class AWSLoadBalancerServersRetreiver
    {
        private BackgroundWorker worker;

        public string AWSAccessKey { get; set; }
        public string AWSSecretKey { get; set; }
        public Region EC2Region { get; set; }
        
        
        public delegate void AWSLoadBalancerServersRetreiverCompletedEventHandler(AWSLoadBalancerServersRetreiver sender, AWSLoadBalancerServersRetreiverCompleteEventArgs e);
        public event AWSLoadBalancerServersRetreiverCompletedEventHandler Completed;
        public virtual void OnCompleted(AWSLoadBalancerServersRetreiverCompleteEventArgs e)
        {
            if (Completed != null)
                Completed(this, e);
        }


        public AWSLoadBalancerServersRetreiver()
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
                OnCompleted(new AWSLoadBalancerServersRetreiverCompleteEventArgs { Result = e.Result as IList<AWSLoadBalancerServersRetreiverCompleteEventArgs.LoadBalancer> });
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

            var elbClient = AWSClientFactory.CreateAmazonElasticLoadBalancingClient(AWSAccessKey,
                                                                               AWSSecretKey,
                                                                               new AmazonElasticLoadBalancingConfig() {
                                                                                    
                                                                                    ServiceURL = RegionHelper.ELBEndpointForRegion(EC2Region).Replace("http:","https:") 
                                                                               });

            DescribeLoadBalancersResponse loadBalancers=null;
            try
            {
                loadBalancers = elbClient.DescribeLoadBalancers();
            }
            catch (WebException wex)
            {
                var response = wex.Response as HttpWebResponse;
                var responsveBody = new StreamReader(response.GetResponseStream()).ReadToEnd();
                int a=5;
            }

            e.Result = loadBalancers.DescribeLoadBalancersResult.LoadBalancerDescriptions.
                Select(lb => new AWSLoadBalancerServersRetreiverCompleteEventArgs.LoadBalancer {
                    LoadBalancerName = lb.LoadBalancerName,
                    InstanceNames = lb.Instances.Select(instance=>instance.InstanceId).ToList()
                }).
                ToList();

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
