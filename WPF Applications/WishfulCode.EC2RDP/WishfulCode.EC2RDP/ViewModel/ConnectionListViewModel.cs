using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;

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
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
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
                }
            });
        }

        public ICommand OpenConnection { get; set; }

        public ObservableCollection<ConnectionViewModel> OpenConnections { get; set; }

        public ObservableCollection<ConnectionViewModel> Connections { get; set; }

        public override void Cleanup()
        {
            // Clean own resources if needed

            base.Cleanup();
        }
    }
}