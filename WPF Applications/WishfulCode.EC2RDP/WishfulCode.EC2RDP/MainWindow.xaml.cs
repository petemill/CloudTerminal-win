using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WishfulCode.EC2RDP.ViewModel;

namespace WishfulCode.EC2RDP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
        }

        private void ConnectionListItem_DoubleClick(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            ConnectionViewModel data = (ConnectionViewModel)((ListBoxItem)sender).Content;
            ViewModelLocator.ConnectionListStatic.OpenConnection.Execute(data);
            openConnectionList.SelectedItem = data;
        }

        private void openConnectionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems!=null && e.AddedItems.Count>0)
            {
                var selectedItem = e.AddedItems[0] as ConnectionViewModel;
                //find item in open connections and select it
                ConnectionViews.SelectedItem = selectedItem;
            }
        }


    }
}
