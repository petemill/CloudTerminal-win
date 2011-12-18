using System.Threading;
using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Linq;
using WishfulCode.EC2RDP.AWSInterface;
using WishfulCode.mRDP.AWSInterface;
using System.Windows.Threading;
using System;
using WishfulCode.EC2RDP.Properties;
using WishfulCode.EC2RDP.Model;


namespace WishfulCode.EC2RDP.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm/getstarted
    /// </para>
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        CloudWatchInstanceDataRetreiver dataWorker;
        DispatcherTimer watcherTimer;

        /// <summary>
        /// Initializes a new instance of the MainWindowViewModel class.
        /// </summary>
        public MainWindowViewModel()
        {


            Connections = new ObservableCollection<ConnectionViewModel>();
            OpenConnections = new ObservableCollection<ConnectionViewModel>();

            Connections.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Connections_CollectionChanged);
            OpenConnections.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(OpenConnections_CollectionChanged);

            

            if (IsInDesignMode)
            {
                // Code runs "for real": Connect to service, etc...
                Connections.Add(new ConnectionViewModel { Id="i-xxxxxx", Name = "test1", Host = "test1.fake.com", HexIp = "IP-0A0A0A0A"});
                Connections.Add(new ConnectionViewModel { Id = "i-yyyyyy", Name = "test2 as dasd asdas das d", Host = "test2.fake.com", HexIp = "IP-0A0A0A0A" });
            }
            else
            {
                RefreshInstanceDataAsync();
            }

            OpenConnection = new RelayCommand<ConnectionViewModel>(item =>
            {
                if (!OpenConnections.Contains(item))
                {
                    OpenConnections.Add(item);
                    // Update bindings, no broadcast
                }
            });

            //dataworker registration
            dataWorker = new CloudWatchInstanceDataRetreiver
            {
                AWSAccessKey = Settings.Default.AWSAccessKey,
                AWSSecretKey = Settings.Default.AWSSecretKey,
                EC2Region = Region.EU //TODO: from settings
            };
            //dataworker events
            dataWorker.Completed += (sender, e) =>
                                        {
                                            Dispatcher.CurrentDispatcher.BeginInvoke(new  ThreadStart(() =>
                                                                                                                                   {
                                            //for each cpu history chart, add to relevant instance VM
                                            e.CPUHistory.ToList().ForEach(cpuHistory =>
                                                                              {
                                                                                  var connectionVM = Connections.Where(con => con.Id.Equals(cpuHistory.Key, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                                                                                  if (connectionVM != null)
                                                                                  {
                                                                                      
                                                                                                                                       connectionVM.CPUData = cpuHistory.Value;
                                                                                                                            
                                                                                  }
                                                                              });
                                            //set timer to refresh data (use a DispatcherTimer as Timer causes DependencySource and DependencyObject issues, even when using Dispatcher.Invoke)
                                            watcherTimer = new DispatcherTimer(new TimeSpan(0, 0, 0, 10), DispatcherPriority.Normal,(dsender,de)=> RefreshInstanceMonitorDataAsync(), Dispatcher.CurrentDispatcher);
                                            watcherTimer.Start();
                                                                                                                                   }));
                                          
                                        };

            RefreshWatchData = new RelayCommand(() =>
            {
                RefreshInstanceDataAsync();
            });

        }
    
        void RefreshInstanceMonitorDataAsync()
        {
            if (Connections != null && Connections.Count > 0)
            {
                //begin retreiving instance data (eg: cpu utilization)
                dataWorker.InstanceIds = Connections.Select(instance => instance.Id);
                dataWorker.FetchAsync();
            }
            if (watcherTimer != null)
                watcherTimer.Stop();
        }

        void RefreshInstanceDataAsync()
        {
            //stop watcher data retreival
            if (watcherTimer!=null)
                watcherTimer.Stop();

            //begin retreival of instances
            var ec2worker = new AWSInstanceRetreiver()
            {
                AWSAccessKey = Settings.Default.AWSAccessKey,
                AWSSecretKey = Settings.Default.AWSSecretKey,
                EC2Region = Region.EU
            };

            ec2worker.Completed += (sender, e) =>
            {
                Dispatcher.CurrentDispatcher.BeginInvoke((Action)(() =>
                                                                                                        {

                                                                                                            UpdateInstanceDataUI(e.Result);
                                                                                                            //fetch data on a timer
                                                                                                            RefreshInstanceMonitorDataAsync();

                                                                                                        }
                                                                                                   ));
                
            };

            ec2worker.FetchAsync();
        }

        void UpdateInstanceDataUI(IList<Connection> instanceData)
        {
            //display status, and add connections retreived to list
            if (instanceData != null)
            {
                //set status
                InstanceDataStatus = String.Concat(instanceData.Count, " Instance", (instanceData.Count > 1) ? "s" : String.Empty, " retreived.");

                //set connection list to retreived result
                //don't clear, check if instance is already in list, only add new item if not (and remove if not in list, even if open)
                var incomingAsVM = instanceData.Select(conn => new ConnectionViewModel().WithConnection(conn));

                var newConnections = incomingAsVM.Except(Connections);
                var removedConnections = Connections.Except(incomingAsVM);

                newConnections.ToList().ForEach(Connections.Add);
                removedConnections.ToList().ForEach(item => Connections.Remove(item));
            }
            else
            {
                InstanceDataStatus = "Error retreiving instances.";
            }
        }



        void Connections_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //notify that the closed collection list is changed
            RaisePropertyChanged(ClosedConnectionsPropertyName);

            //whenever a new connectionview is added, set up the event so that when it is disconnected, it is removed from the open connection list
            if (e.NewItems!=null && e.NewItems.Count > 0)
            {
                e.NewItems.Cast<ConnectionViewModel>().ToList().ForEach(cn =>
                    {
                        cn.Disconnected += (s, se) => OpenConnections.Remove(s as ConnectionViewModel);
                    });
            }
        }

        void OpenConnections_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //notify that the closed collection list is changed
            RaisePropertyChanged(ClosedConnectionsPropertyName);
        }

        public ICommand OpenConnection { get; set; }
        public ICommand RefreshWatchData { get; set; }

        public ObservableCollection<ConnectionViewModel> OpenConnections { get; set; }

        public ObservableCollection<ConnectionViewModel> Connections { get; set; }

        /// <summary>
        /// The <see cref="ClosedConnections" /> property's name.
        /// </summary>
        public const string ClosedConnectionsPropertyName = "ClosedConnections";

       
        /// <summary>
        /// Gets the ClosedConnections property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public IEnumerable<ConnectionViewModel> ClosedConnections
        {
            get
            {
                return Connections.Except(OpenConnections).OrderBy(con => con.Name).Where(con => con.Name.Contains(ClosedConnectionsFilter) || con.HexIp.ToLower().Contains(ClosedConnectionsFilter.ToLower()) );
            }
        }


        public override void Cleanup()
        {
            // Clean own resources if needed

            base.Cleanup();
        }


        #region BindableProperties

        /// <summary>
        /// The <see cref="InstanceDataStatus" /> property's name.
        /// </summary>
        public const string InstanceDataStatusPropertyName = "InstanceDataStatus";

        private string _instanceDataStatus = "No Instances Retreived.";

        /// <summary>
        /// Gets the InstanceDataStatus property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public string InstanceDataStatus
        {
            get
            {
                return _instanceDataStatus;
            }

            set
            {
                if (_instanceDataStatus == value)
                {
                    return;
                }

                var oldValue = _instanceDataStatus;
                _instanceDataStatus = value;


                // Update bindings, no broadcast
                RaisePropertyChanged(InstanceDataStatusPropertyName);

            }
        }

        /// <summary>
        /// The <see cref="InstanceDataStatus" /> property's name.
        /// </summary>
        public const string ClosedConnectionsFilterPropertyName = "ClosedConnectionsFilter";

        private string _closedConnectionsFilter = string.Empty;

        /// <summary>
        /// Gets the InstanceDataStatus property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public string ClosedConnectionsFilter
        {
            get
            {
                return _closedConnectionsFilter;
            }

            set
            {
                if (_closedConnectionsFilter == value)
                {
                    return;
                }

                var oldValue = _closedConnectionsFilter;
                _closedConnectionsFilter = value;


                // Update bindings, no broadcast
                RaisePropertyChanged(ClosedConnectionsFilterPropertyName);
                RaisePropertyChanged(ClosedConnectionsPropertyName);
            }
        }


        #endregion BindableProperties
    }
}