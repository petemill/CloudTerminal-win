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
            // scarta - removed error checking so we can actually be notified if the callback exploded.
            // previous line: if (e.Error == null && !e.Cancelled && e.Result != null && !String.IsNullOrWhiteSpace(e.Result as String))
            if (!e.Cancelled)
            {
                OnCompleted(new AWSAdministratorPasswordRetreiverCompleteEventArgs {
                    Password = e.Result as String,
                    Exception = e.Result as Exception
                });

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
                var getPwRequest = ec2.GetPasswordData(
                    new Amazon.EC2.Model.GetPasswordDataRequest().
                        WithInstanceId(InstanceId)
                );

                var result = getPwRequest.GetPasswordDataResult;
                if (string.IsNullOrWhiteSpace(result.PasswordData.Data))
                {
                    throw new Exception("No password data was returned.");
                }

                var decryptedPassword = result.GetDecryptedPassword(PrivateKey.Trim());
                e.Result = decryptedPassword;
            }
            catch (Exception ex)
            {
                e.Result = ex;
                return;
            }
        }
    }
}
