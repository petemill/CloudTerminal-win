using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Linq;


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
    public class ConnectionListViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the ConnectionListViewModel class.
        /// </summary>
        public ConnectionListViewModel()
        {
            Connections = new ObservableCollection<ConnectionViewModel>();
            OpenConnections = new ObservableCollection<ConnectionViewModel>();

            Connections.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Connections_CollectionChanged);
            OpenConnections.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(OpenConnections_CollectionChanged);

            if (IsInDesignMode)
            {
                // Code runs "for real": Connect to service, etc...
                Connections.Add(new ConnectionViewModel { Name = "test1", Host = "test1.fake.com" });
                Connections.Add(new ConnectionViewModel { Name = "test2", Host = "test2.fake.com" });
            }
            else
            {
                // Code runs "for real": Connect to service, etc...
               Connections.Add(new ConnectionViewModel { Name = "test1", Host = "test1.fake.com" });
               Connections.Add(new ConnectionViewModel { Name = "test2", Host = "test2.fake.com" });
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
                return Connections.Except(OpenConnections);
            }

    
        }

        public override void Cleanup()
        {
            // Clean own resources if needed

            base.Cleanup();
        }
    }
}