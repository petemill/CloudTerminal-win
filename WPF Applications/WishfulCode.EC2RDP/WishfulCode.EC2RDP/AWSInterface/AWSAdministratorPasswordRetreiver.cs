using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using WishfulCode.mRDP.AWSInterface;
using Amazon;
using Amazon.EC2;

namespace WishfulCode.EC2RDP.AWSInterface
{
    public class AWSAdministratorPasswordRetreiver
    {
        BackgroundWorker worker;
        public string InstanceId { get; set; }
        public string AWSAccessKey { get; set; }
        public string AWSSecretKey { get; set; }
        public string PrivateKey { get; set; }

        public Region EC2Region { get; set; }

        public delegate void PasswordRetreiveCompletedEventHandler(AWSAdministratorPasswordRetreiver sender, AWSAdministratorPasswordRetreiverCompleteEventArgs e);
        public event PasswordRetreiveCompletedEventHandler Completed;
        public virtual void OnCompleted(AWSAdministratorPasswordRetreiverCompleteEventArgs e)
        {
            if (Completed != null)
                Completed(this, e);
        }

        public AWSAdministratorPasswordRetreiver()
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
            if (e.Error == null && !e.Cancelled && e.Result != null && !String.IsNullOrWhiteSpace(e.Result as String))
            {
                OnCompleted(new AWSAdministratorPasswordRetreiverCompleteEventArgs { Password = e.Result as String });

            }

        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(InstanceId) || String.IsNullOrWhiteSpace(PrivateKey))
            {
                e.Result = null;
                return;
            }

            //ec2 client
            var ec2 = AWSClientFactory.CreateAmazonEC2Client(AWSAccessKey,
                                                              AWSSecretKey,
                                                              new AmazonEC2Config().WithServiceURL(RegionHelper.EC2EndpointForRegion(EC2Region))
                                                              );
            try
            {
                var decryptedPassword = ec2.GetPasswordData(new Amazon.EC2.Model.GetPasswordDataRequest().WithInstanceId(InstanceId)).GetPasswordDataResult.GetDecryptedPassword(PrivateKey);
                e.Result = decryptedPassword;
            }
            catch
            {
                e.Result = null;
                return;
            }
        }
    }
}
