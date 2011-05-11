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
                Connections.Add(new ConnectionViewModel { Id="i-xxxxxx", Name = "test1", Host = "test1.fake.com" });
                Connections.Add(new ConnectionViewModel { Id="i-yyyyyy", Name = "test2", Host = "test2.fake.com" });
            }
            else
            {
                //begin retreival of instances
                var ec2worker = new AWSInstanceRetreiver()
                {
                    //TODO: get details from UI
                    AWSAccessKey = "",
                    AWSSecretKey = "",
                    EC2Region = Region.EU
                };
                ec2worker.Completed += (sender, e) =>
                    {
                        Dispatcher.CurrentDispatcher.BeginInvoke((Action)(() =>
                        {
                            e.Result.ToList().ForEach(data =>
                                  Connections.Add(new ConnectionViewModel().WithConnection(data))
                                );
                        }));
                    };
                ec2worker.FetchAsync();
            }

            OpenConnection = new RelayCommand<ConnectionViewModel>(item =>
            {
                if (!OpenConnections.Contains(item))
                {
                    OpenConnections.Add(item);
                    // Update bindings, no broadcast
                }
            });

            
        }

        void Connections_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //notify that the closed collection list is changed
            RaisePropertyChanged(ClosedConnectionsPropertyName);

            //whenever a new connectionview is added, set up the event so that when it is disconnected, it is removed from the open connection list
            e.NewItems.Cast<ConnectionViewModel>().ToList().ForEach(cn =>
                {
                    cn.Disconnected += (s, se) => OpenConnections.Remove(s as ConnectionViewModel);
                });
        }

        void OpenConnections_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //notify that the closed collection list is changed
            RaisePropertyChanged(ClosedConnectionsPropertyName);
        }

        public ICommand OpenConnection { get; set; }

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
                return Connections.Except(OpenConnections).OrderBy(con => con.Name);
            }

    
        }

        public override void Cleanup()
        {
            // Clean own resources if needed

            base.Cleanup();
        }
    }
}