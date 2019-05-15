using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using FtpFileRegistry.Utils;

namespace FtpFileRegistry.Workers
{
    public class Listener
    {
        private readonly BackgroundWorker _backgroundWorker = new BackgroundWorker();
        private readonly TcpListener _listener = new TcpListener(
                IPAddress.Loopback,
                Properties.Settings.Default.Port);

        public Listener()
        {
            _backgroundWorker.DoWork += BackgroundWorkerOnDoWork;
            _backgroundWorker.RunWorkerCompleted += BackgroundWorkerOnRunWorkerCompleted;
        }

        public void Start()
        {
            _backgroundWorker.RunWorkerAsync();
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
            var result = (string) args.Result;

            if (result.StartsWith("ftpregistry:"))
            {
                result = result.Split(':')[1];
                var ledger = LedgerManager.GetLedger();
                var settings = SettingsLoader.LoadSettings();

                var ledgerRowModel =
                    ledger.FirstOrDefault(e => String.Equals(e.FileIdentifier, result,
                        StringComparison.CurrentCultureIgnoreCase));
                if (ledgerRowModel != null)
                {
                    var ftpFullPath = settings.FtpTargetPath + "//" + ledgerRowModel.FileName;
                    new Downloader("C://", ftpFullPath).Start();
                }
            }
            else
            {
                new Uploader(result).Start();
            }
            _backgroundWorker.RunWorkerAsync();
        }
    }
}
