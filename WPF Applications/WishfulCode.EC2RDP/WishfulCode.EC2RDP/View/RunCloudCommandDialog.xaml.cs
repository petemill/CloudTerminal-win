using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WishfulCode.EC2RDP.AWSInterface;
using WishfulCode.EC2RDP.Properties;
using WishfulCode.mRDP.AWSInterface;

namespace WishfulCode.EC2RDP.View
{
    /// <summary>
    /// Interaction logic for Accounts.xaml
    /// </summary>
    public partial class RunCloudCommandDialog : Window
    {
        RunCloudCommandViewModel ViewModel;

        public RunCloudCommandDialog()
        {
            AllocConsole();
            InitializeComponent();
            ViewModel = new RunCloudCommandViewModel();
            ViewModel.ServerGroups = new ObservableCollection<CheckedTreeGroup>();
            /*ViewModel.Servers.Add(new CheckedListItem("badger"));
            ViewModel.Servers.Add(new CheckedListItem("wsdf"));
            ViewModel.Servers.Add(new CheckedListItem("sdafds"));*/
            this.DataContext = ViewModel;

            //begin retreival of instances
            var ec2worker = new AWSInstanceRetreiver()
            {
                AWSAccessKey = Settings.Default.AWSAccessKey,
                AWSSecretKey = Settings.Default.AWSSecretKey,
                EC2Region = Region.EU
            };

            var lbWorker = new AWSLoadBalancerServersRetreiver()
            {

                AWSAccessKey = Settings.Default.AWSAccessKey,
                AWSSecretKey = Settings.Default.AWSSecretKey,
                EC2Region = Region.EU
            };

            ec2worker.Completed += (sender, e) =>
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    _Connections = e.Result;
                    UpdateTreeView();

                }
                                                                                                   ));

            };

            lbWorker.Completed += (sender, e) =>
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    _LoadBalancerGroupings = e.Result;
                    UpdateTreeView();

                }));

            };
            ec2worker.FetchAsync();
            lbWorker.FetchAsync();

        }



        IList<Model.Connection> _Connections;
        IList<AWSLoadBalancerServersRetreiverCompleteEventArgs.LoadBalancer> _LoadBalancerGroupings;
        void UpdateTreeView()
        {
            if (_Connections == null) return;

            ViewModel.ServerGroups.Clear();

            Dictionary<string, string> instanceToLoadBalancer = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (_LoadBalancerGroupings != null)
            {
                foreach (var loadBalancer in _LoadBalancerGroupings)
                {
                    foreach (var instance in loadBalancer.InstanceNames)
                    {
                        instanceToLoadBalancer[instance] = loadBalancer.LoadBalancerName;
                    }
                }
            }


            var data = _Connections.
                        Select(c =>
                        {
                            string groupName;
                            if (!instanceToLoadBalancer.TryGetValue(c.Id, out groupName))
                            {
                                groupName = "Unknown group";
                            }
                            return new
                            {
                                DisplayName = "(" + c.AvailabilityZone + ") " + c.Name + " [" + c.HexIp + "]",
                                ConnectionInfo = c,
                                Grouping = groupName
                            };
                        }).
                        GroupBy(g => g.Grouping, StringComparer.OrdinalIgnoreCase);

            foreach (var group in data.OrderBy(g => g.Key))
            {
                CheckedTreeGroup groupTreeItem = new CheckedTreeGroup();
                groupTreeItem.Name = group.Key;
                foreach (var blah in group.OrderBy(item => item.DisplayName).Select(item => new CheckedTreeItem(item.DisplayName, item.ConnectionInfo)))
                {
                    groupTreeItem.Children.Add(blah);
                }
                ViewModel.ServerGroups.Add(groupTreeItem);
            }
        }


        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            FreeConsole();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Run_Click(object sender, RoutedEventArgs e)
        {
            var servers = ViewModel.ServerGroups.SelectMany(group => group.Children).Where(c => c.IsChecked).Select(c => new { HostName = c.ConnectionInfo.Host, FriendlyName = c.Name });
            if (!servers.Any())
            {
                MessageBox.Show("No servers ticked", "oi!", MessageBoxButton.OK);
                return;
            }
            if (servers.Any(c => string.IsNullOrWhiteSpace(c.HostName)))
            {
                MessageBox.Show("Some servers had blank host names", "oi!", MessageBoxButton.OK);
                return;
            }

            if (ViewModel.Command == null || ViewModel.Command.Trim().Length == 0)
            {
                MessageBox.Show("No command to run", "oi!", MessageBoxButton.OK);
                return;
            }

            RunPowershellSync(new[] { "net start WinRM", "exit" });
            var trustedHostsResult = RunPowershellSync(new[] { "(Get-Item WSMan:\\localhost\\Client\\TrustedHosts).Value", "exit" });
            if (trustedHostsResult.Item1.Trim() != "*")
            {
                if (MessageBox.Show("You need to allow powershell to send credentials objects to others hosts. Do you want to do this now?", "oi!", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Process.Start("powershell", "-command Set-Item WSMan:\\localhost\\Client\\TrustedHosts -value *");
                }
                return;
            }

            string[] commands = ViewModel.Command.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            List<string> commandsToSendToPowershell = new List<string>();
            commandsToSendToPowershell.Add("\"##--------STARTING CLOUD RUN COMMAND-------##\"");
            commandsToSendToPowershell.Add("\"**Please find the credentials window and enter a password**\"");
            commandsToSendToPowershell.Add("$cred = get-credential Administrator");
            foreach (var server in servers)
            {
                commandsToSendToPowershell.Add("icm -Credential $cred " + server.HostName + " -scriptblock { ");
                commandsToSendToPowershell.Add("\"---Running command on " + server.FriendlyName + "---\"");
                commandsToSendToPowershell.AddRange(commands);
                commandsToSendToPowershell.Add("}");
                commandsToSendToPowershell.Add(""); // blank line to confirm
                commandsToSendToPowershell.Add("\"---Command finished on " + server.FriendlyName + "---\"");
            }
            commandsToSendToPowershell.Add("\"##--------FINISHED CLOUD RUN COMMAND-------##\"");
            commandsToSendToPowershell.Add("exit");

            RunButton.IsEnabled = false;
            RunPowershell(commandsToSendToPowershell);


        }

        object threadCountLock = new object();
        int RunningThreads = 0;
        void RunPowershell(IEnumerable<string> commands)
        {
            if (RunningThreads != 0)
            {
                MessageBox.Show("Cannot run two commands at the same time.");
                return;
            }
            string args = "-command -"; //make it read from standard input

            ProcessStartInfo startInfo = new ProcessStartInfo("powershell", args);
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardInput = true;

            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;

            string output = null;
            string errors = null;

            Process p = Process.Start(startInfo);
            {
                foreach (var powerShellCommand in commands)
                {
                    p.StandardInput.WriteLine(powerShellCommand);
                }

                Thread outputWatcherThread = new Thread(() =>
                {
                    string line = null;
                    while ((line = p.StandardOutput.ReadLine()) != null)
                    {
                        WriteToOutput(line);
                    }
                    ThreadFinished(p);
                });

                Thread errorWatcherThread = new Thread(() =>
                {
                    string line = null;
                    while ((line = p.StandardError.ReadLine()) != null)
                    {
                        WriteToOutput("ERROR: " + line);
                    }
                    ThreadFinished(p);

                });

                RunningThreads = 2;
                outputWatcherThread.Start();
                errorWatcherThread.Start();
            }
        }

        void ThreadFinished(Process p)
        {
            lock (threadCountLock)
            {
                RunningThreads--;
            }
            if (RunningThreads == 0)
            {
                p.Dispose();
                Dispatcher.Invoke(new Action(() =>
                {
                    RunButton.IsEnabled = true;
                }));
                //FreeConsole();
            }

        }

        Tuple<string, string> RunPowershellSync(IEnumerable<string> commands)
        {
            string args = "-command -"; //make it read from standard input

            ProcessStartInfo startInfo = new ProcessStartInfo("powershell", args);
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardInput = true;

            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;

            string output = null;
            string errors = null;

            Process p = Process.Start(startInfo);
            {
                foreach (var powerShellCommand in commands)
                {
                    p.StandardInput.WriteLine(powerShellCommand);
                }

                output = p.StandardOutput.ReadToEnd();
                errors = p.StandardError.ReadToEnd();

            }
            return Tuple.Create(output, errors);
        }

        void WriteToOutput(string line)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                //OutputTextBox.Text+= Environment.NewLine + line;
                Console.WriteLine(line);
                //OutputTextBox.ScrollToEnd();
            }));
        }

        [DllImport("Kernel32")]
        public static extern void AllocConsole();

        [DllImport("Kernel32")]
        public static extern void FreeConsole();


        class RunCloudCommandViewModel
        {
            public string Command { get; set; }

            public ObservableCollection<CheckedTreeGroup> ServerGroups { get; set; }
            //public static readonly DependencyProperty TopicListProperty = DependencyProperty.Register("TopicList", typeof(ObservableCollection<CheckedListItem>), typeof(MainWindow), new UIPropertyMetadata(null));
        }

        public class TreeViewItemBase : INotifyPropertyChanged
        {
            private bool isSelected;
            public bool IsSelected
            {
                get { return this.isSelected; }
                set
                {
                    if (value != this.isSelected)
                    {
                        this.isSelected = value;
                        NotifyPropertyChanged("IsSelected");
                    }
                }
            }

            private bool isExpanded;
            public bool IsExpanded
            {
                get { return this.isExpanded; }
                set
                {
                    if (value != this.isExpanded)
                    {
                        this.isExpanded = value;
                        NotifyPropertyChanged("IsExpanded");
                    }
                }
            }

            private bool isChecked;
            public bool IsChecked
            {
                get { return this.isChecked; }
                set
                {
                    if (value != this.isChecked)
                    {
                        this.isChecked = value;
                        NotifyPropertyChanged("IsChecked");
                    }
                }
            }


            public event PropertyChangedEventHandler PropertyChanged;

            public void NotifyPropertyChanged(string propName)
            {
                if (this.PropertyChanged != null)
                    this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
        class CheckedTreeGroup : TreeViewItemBase
        {

            public string Name { get; set; }
            public IList<CheckedTreeItem> Children { get; set; }

            public CheckedTreeGroup()
            {
                Children = new List<CheckedTreeItem>();
            }
        }

        class CheckedTreeItem : TreeViewItemBase
        {
            
            public CheckedTreeItem(string text, Model.Connection connectionInfo)
            {
                Name = text;
                ConnectionInfo = connectionInfo;
            }

            public string Name { get; set; }
            public Model.Connection ConnectionInfo { get; set; }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var checkbox = (System.Windows.Controls.CheckBox)e.Source;

            foreach (var grouping in ViewModel.ServerGroups.ToArray())
            {
                if (grouping.Name == checkbox.Content)
                {
                    foreach (var item in grouping.Children.ToArray())
                    {
                        item.IsChecked = checkbox.IsChecked.GetValueOrDefault();
                    }
                }
            }

            RunButton.Content = "Run on " + ViewModel.ServerGroups.Sum(group => group.Children.Where(instance => instance.IsChecked).Count()) + " servers";

            //var items = ViewModel.ServerGroups[0];
            //ViewModel.ServerGroups.RemoveAt(0);
            //ViewModel.ServerGroups.Insert(0, items);
        }
    }
}
