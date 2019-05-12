using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows;

namespace FtpFileRegistry
{
    public partial class MainWindow
    {
        private readonly BackgroundWorker _backgroundWorker;
        private TcpListener _listener;

        public MainWindow()
        {
            InitializeComponent();
            SubscribeForEvents();

            _backgroundWorker = new BackgroundWorker();

            WindowState = WindowState.Minimized;
            Hide();

            SetupListener();

            _backgroundWorker.DoWork += BackgroundWorkerOnDoWork;
            _backgroundWorker.RunWorkerCompleted += BackgroundWorkerOnRunWorkerCompleted;
            
            _backgroundWorker.RunWorkerAsync();
        }

        private void SetupListener()
        {
            _listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 8889);
        }

        private void BackgroundWorkerOnDoWork(object sender, DoWorkEventArgs args)
        {
            string message = null;
            _listener.Start();

            while (string.IsNullOrEmpty(message))
            {
                var client = _listener.AcceptTcpClient();
                using (var sr = new StreamReader(client.GetStream()))
                {
                    message = sr.ReadToEnd();
                }

                Thread.Sleep(100);
            }

            args.Result = message;
        }

        private void BackgroundWorkerOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs args)
        {
            MessageBox.Show(args.Result as string);
            _backgroundWorker.RunWorkerAsync();
        }

        private void SubscribeForEvents()
        {
            ExitMenuItem.Click += ExitMenuItem_OnClick;
            SettingsMenuItem.Click += SettingsMenuItem_OnClick;
        }
       

        private void SettingsMenuItem_OnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            WindowState = WindowState.Normal;
            var desktopWorkingArea = SystemParameters.WorkArea;
            Left = desktopWorkingArea.Right - Width;
            Top = desktopWorkingArea.Bottom - Height;
            Show();
        }

        private void ExitMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
