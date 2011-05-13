using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

        public IEnumerable<string> InstanceIds { get; set; }

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
            //set up worker events
            worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
        }

        public void FetchAsync()
        {
            //run background worker and return
            worker.RunWorkerAsync();
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //
            if (e.Error == null && !e.Cancelled)
            {
                OnCompleted(new CloudWatchInstanceDataRetreiveCompleteEventArgs { CPUHistory = e.Result as Dictionary<string,IEnumerable<DataPoint>>  });
            }
        }

        IEnumerable<DataPoint> GetCPUUtilizationForInstance(string instanceId, DateTime start, DateTime end)
        {
            //build request
             var req = new GetMetricStatisticsRequest()
            {
                Dimensions = {
                                    new Dimension
                                              {
                                                  Name = "InstanceId",
                                                  Value = instanceId
                                              }
                                },
                MetricName = "CPUUtilization",
                Statistics = new List<String> { "Average" },
                Period = 60,
                Unit = "Percent",
                Namespace = "AWS/EC2",
                StartTime = start,
                EndTime = end
            };
            
            //send request
            //cloudwatch client
            var cloudWatch = AWSClientFactory.CreateAmazonCloudWatchClient(AWSAccessKey,
                                                              AWSSecretKey,
                                                              new AmazonCloudWatchConfig() { ServiceURL = RegionHelper.CloudWatchEndpointForRegion(EC2Region) }
                                                              );

            try
            {
                var response = cloudWatch.GetMetricStatistics(req);
                if (response.GetMetricStatisticsResult == null)
                {
                    return null;
                }

                return response.GetMetricStatisticsResult.Datapoints.Select(datapoint => new DataPoint { Value = datapoint.Average, TimeStamp = datapoint.Timestamp });
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            //set date period to measure
            DateTime end = DateTime.Now.ToUniversalTime();
            DateTime start = DateTime.Now.AddMinutes(-20).ToUniversalTime();

            //create object to return data in
            var results = new Dictionary<string, IEnumerable<DataPoint>>();
            
            //must do single call for each instance to retreive separate data
            Parallel.ForEach<string,KeyValuePair<string,IEnumerable<DataPoint>>>(InstanceIds,
                                              () => new KeyValuePair<string, IEnumerable<DataPoint>>(), 
                                              (instanceId,state,s) =>
                                              {
                                                  //get data
                                                  var result = GetCPUUtilizationForInstance(instanceId, start, end);

                                                  //add to collection against instance id
                                                  if (result != null)
                                                  {
                                                      return new KeyValuePair<string, IEnumerable<DataPoint>>(instanceId, result);
                                                  }
                                                  
                                                  return new KeyValuePair<string, IEnumerable<DataPoint>>();
                                              },
                                              (finalResult) =>
                                                  {
                                                      if ( finalResult.Key!=null && finalResult.Value!=null)
                                                     results.Add(finalResult.Key,finalResult.Value);    
                                                  }
                                              
                                              );
            //set return value
            e.Result = results;
        }

        
    }
}
