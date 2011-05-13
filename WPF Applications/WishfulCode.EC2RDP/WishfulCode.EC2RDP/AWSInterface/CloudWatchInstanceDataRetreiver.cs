using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WishfulCode.mRDP.AWSInterface;
using System.ComponentModel;
using WishfulCode.EC2RDP.Model;
using Amazon.CloudWatch.Model;
using Amazon;
using Amazon.CloudWatch;

namespace WishfulCode.EC2RDP.AWSInterface
{
    public class CloudWatchInstanceDataRetreiver
    {
        public string AWSAccessKey { get; set; }
        public string AWSSecretKey { get; set; }
        public Region EC2Region { get; set; }

        public IEnumerable<string> InstancIds { get; set; }

        public delegate void CloudWatchInstanceDataRetreiveCompletedEventHandler(CloudWatchInstanceDataRetreiver sender, CloudWatchInstanceDataRetreiveCompleteEventArgs e);
        public event CloudWatchInstanceDataRetreiveCompletedEventHandler Completed;

        public virtual void OnCompleted(CloudWatchInstanceDataRetreiveCompleteEventArgs e)
        {
            if (Completed != null)
                Completed(this, e);
        }


        BackgroundWorker worker;

        public CloudWatchInstanceDataRetreiver()
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
                OnCompleted(new CloudWatchInstanceDataRetreiveCompleteEventArgs { CPUHistory = e.Result as IEnumerable<DataPoint> });
            }
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            //set date period to measure
            DateTime end = DateTime.Now.ToUniversalTime();
            DateTime start = DateTime.Now.AddMinutes(-10).ToUniversalTime();

            //build request
            var req = new GetMetricStatisticsRequest()
            {
                Dimensions = InstancIds.Select( instanceId =>
                                          new Dimension
                                              {
                                                  Name = "InstanceId",
                                                  Value = instanceId
                                              }).ToList(),
                MetricName = "CPUUtilization",
                Statistics = new List<String> { "Samples" },
                Period = Convert.ToInt32(end.Subtract(start).TotalMinutes * 60),
                Unit = "percent",
                Namespace = "AWS/EC2",
                StartTime = start,
                EndTime = end
            };
            
            //send request
            IEnumerable<DataPoint> results;
            //cloudwatch client
            var cloudWatch = AWSClientFactory.CreateAmazonCloudWatchClient(AWSAccessKey,
                                                              AWSSecretKey,
                                                              new AmazonCloudWatchConfig() { ServiceURL = RegionHelper.EC2EndpointForRegion(EC2Region) }
                                                              );

            try
            {
                var response = cloudWatch.GetMetricStatistics(req);
                if (response.GetMetricStatisticsResult == null)
                {
                    e.Result = null;
                    return;
                }

                results = response.GetMetricStatisticsResult.Datapoints.Select(datapoint => new DataPoint { Value = datapoint.SampleCount, TimeStamp = datapoint.Timestamp });
            }
            catch (Exception ex)
            {
                e.Result = null;
                return;
            }

            e.Result = results;

        }
    }
}
