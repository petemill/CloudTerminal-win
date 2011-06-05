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
    /// <summary>
    /// Retreives sample data for testing. Data is random each time
    /// </summary>
    public class SampleCloudWatchInstanceDataRetreiver
    {
        public string AWSAccessKey { get; set; }
        public string AWSSecretKey { get; set; }
        public Region EC2Region { get; set; }

        public IEnumerable<string> InstanceIds { get; set; }

        public delegate void CloudWatchInstanceDataRetreiveCompletedEventHandler(SampleCloudWatchInstanceDataRetreiver sender, CloudWatchInstanceDataRetreiveCompleteEventArgs e);
        public event CloudWatchInstanceDataRetreiveCompletedEventHandler Completed;

        public virtual void OnCompleted(CloudWatchInstanceDataRetreiveCompleteEventArgs e)
        {
            if (Completed != null)
                Completed(this, e);
        }


        BackgroundWorker worker;

        public SampleCloudWatchInstanceDataRetreiver()
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
            var result = new List<DataPoint>();

            var periods = (end - start).TotalSeconds/30;
        

            for (int i = 0; i < periods; i++)
            {
                result.Add(new DataPoint{TimeStamp=start.AddSeconds((i+1)*30),Value=r.Next(0,101)});
            }

            return result;

        }


        Random r = new Random();

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            
            //set date period to measure
            DateTime end = DateTime.Now.ToUniversalTime();
            DateTime start = DateTime.Now.AddMinutes(-20).ToUniversalTime();

            //must do single call for each instance to retreive separate data
            var results = InstanceIds
                .Select(instanceId => new KeyValuePair<string, IEnumerable<DataPoint>>(instanceId, GetCPUUtilizationForInstance(instanceId, start, end)))
                .ToList()
                .ToDictionary(val => val.Key, val => val.Value);

            
            
            //set return value
            e.Result = results;
        }

        
    }
}
