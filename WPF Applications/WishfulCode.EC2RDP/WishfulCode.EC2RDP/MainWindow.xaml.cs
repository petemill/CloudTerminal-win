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
using WishfulCode.EC2RDP.View;
using WishfulCode.EC2RDP.ViewModel;
using System.Windows.Interop;

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
            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
            this.SizeChanged += new SizeChangedEventHandler(MainWindow_SizeChanged);
            this.WindowState = WindowState.Maximized;
        }

        void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ShowGlass();
        }

        void ShowGlass()
        {

            bool showFull = (ConnectionViews.Items.Count == 0);
             try
            {
                // Obtain the window handle for WPF application
                IntPtr mainWindowPtr = new WindowInteropHelper(this).Handle;
                HwndSource mainWindowSrc = HwndSource.FromHwnd(mainWindowPtr);
                mainWindowSrc.CompositionTarget.BackgroundColor = Color.FromArgb(0, 0,0,0);

                // Get System Dpi
                System.Drawing.Graphics desktop = System.Drawing.Graphics.FromHwnd(mainWindowPtr);
                float DesktopDpiX = desktop.DpiX;
                float DesktopDpiY = desktop.DpiY;

                // Set Margins
                NonClientRegionAPI.MARGINS margins = new NonClientRegionAPI.MARGINS();

                // Extend glass frame into client area
                // Note that the default desktop Dpi is 96dpi. The  margins are
                // adjusted for the system Dpi.
                if (showFull)
                {
                    margins.cxLeftWidth = -1;
                    margins.cxRightWidth = -1;
                    margins.cyTopHeight = -1;
                    margins.cyBottomHeight = -1;
                }
                else
                {
                    margins.cxLeftWidth = Convert.ToInt32(((int)ConnectionViews.TranslatePoint(new Point(0,0),this).X) * (DesktopDpiX / 96));
                    margins.cxRightWidth = 0;
                    margins.cyTopHeight = 0;// Convert.ToInt32(5 * (DesktopDpiX / 96));
                    margins.cyBottomHeight = 0;
                }

                int hr = NonClientRegionAPI.DwmExtendFrameIntoClientArea(mainWindowSrc.Handle, ref margins);
                //
                if (hr < 0)
                {
                    //DwmExtendFrameIntoClientArea Failed
                }
            }
            // If not Vista, paint background white.
            catch (DllNotFoundException)
            {
                Application.Current.MainWindow.Background = Brushes.White;
            }
        }



        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ShowGlass();
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
            ShowGlass();
        }

        private void AccountSettings_Click(object sender, RoutedEventArgs e)
        {
            Accounts accountDialog = new Accounts();
            accountDialog.ShowDialog();
        }


    }
}
