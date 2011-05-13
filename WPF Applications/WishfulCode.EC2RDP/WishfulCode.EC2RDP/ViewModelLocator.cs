/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:WishfulCode.EC2RDP"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
  
  OR (WPF only):
  
  xmlns:vm="clr-namespace:WishfulCode.EC2RDP"
  DataContext="{Binding Source={x:Static vm:ViewModelLocator.ViewModelNameStatic}}"
*/

using WishfulCode.EC2RDP.ViewModel;

namespace WishfulCode.EC2RDP
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// <para>
    /// Use the <strong>mvvmlocatorproperty</strong> snippet to add ViewModels
    /// to this locator.
    /// </para>
    /// <para>
    /// In Silverlight and WPF, place the ViewModelLocator in the App.xaml resources:
    /// </para>
    /// <code>
    /// &lt;Application.Resources&gt;
    ///     &lt;vm:ViewModelLocator xmlns:vm="clr-namespace:WishfulCode.EC2RDP"
    ///                                  x:Key="Locator" /&gt;
    /// &lt;/Application.Resources&gt;
    /// </code>
    /// <para>
    /// Then use:
    /// </para>
    /// <code>
    /// DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
    /// </code>
    /// <para>
    /// You can also use Blend to do all this with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm/getstarted
    /// </para>
    /// <para>
    /// In <strong>*WPF only*</strong> (and if databinding in Blend is not relevant), you can delete
    /// the ViewModelName property and bind to the ViewModelNameStatic property instead:
    /// </para>
    /// <code>
    /// xmlns:vm="clr-namespace:WishfulCode.EC2RDP"
    /// DataContext="{Binding Source={x:Static vm:ViewModelLocator.ViewModelNameStatic}}"
    /// </code>
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view models
            ////}
            ////else
            ////{
            ////    // Create run time view models
            ////}
        }


        private static ViewModel.MainWindowViewModel _connectionListViewModel;

        /// <summary>
        /// Gets the ConnectionList property.
        /// </summary>
        public static ViewModel.MainWindowViewModel ConnectionListStatic
        {
            get
            {
                if (_connectionListViewModel == null)
                {
                    CreateConnectionList();
                }

                return _connectionListViewModel;
            }
        }

        /// <summary>
        /// Gets the ConnectionList property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ViewModel.MainWindowViewModel ConnectionList
        {
            get
            {
                return ConnectionListStatic;
            }
        }

        /// <summary>
        /// Provides a deterministic way to delete the ConnectionList property.
        /// </summary>
        public static void ClearConnectionList()
        {
            _connectionListViewModel.Cleanup();
            _connectionListViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the ConnectionList property.
        /// </summary>
        public static void CreateConnectionList()
        {
            if (_connectionListViewModel == null)
            {
                _connectionListViewModel = new ViewModel.MainWindowViewModel();
            }
        }

        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
            ClearConnectionList();
            ClearAccountSettings();
        }


        private static AccountSettingsViewModel _accountSettings;

        /// <summary>
        /// Gets the AccountSettings property.
        /// </summary>
        public static AccountSettingsViewModel AccountSettingsStatic
        {
            get
            {
                if (_accountSettings == null)
                {
                    CreateAccountSettings();
                }

                return _accountSettings;
            }
        }

        /// <summary>
        /// Gets the ViewModelPropertyName property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public AccountSettingsViewModel AccountSettings
        {
            get
            {
                return AccountSettingsStatic;
            }
        }

        /// <summary>
        /// Provides a deterministic way to delete the ViewModelPropertyName property.
        /// </summary>
        public static void ClearAccountSettings()
        {
            _accountSettings.Cleanup();
            _accountSettings = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the ViewModelPropertyName property.
        /// </summary>
        public static void CreateAccountSettings()
        {
            if (_accountSettings == null)
            {
                _accountSettings = new AccountSettingsViewModel();
            }
        }

   
    }
}