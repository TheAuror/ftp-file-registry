using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using FtpFileRegistry.Models;
using FtpFileRegistry.Properties;
using FtpFileRegistry.Utils;

namespace FtpFileRegistry.Workers
{
    public class Uploader
    {
        private readonly string _localFullPath;
        private readonly BackgroundWorker _backgroundWorker = new BackgroundWorker();
        public enum Result { Error, Success, Cancelled };

        public Uploader(string localFullPath)
        {
            _localFullPath = localFullPath;
            _backgroundWorker.DoWork += BackgroundWorkerOnDoWork;
            _backgroundWorker.ProgressChanged += BackgroundWorkerOnProgressChanged;
            _backgroundWorker.RunWorkerCompleted += BackgroundWorkerOnRunWorkerCompleted;
        }

        public void Start()
        {
            _backgroundWorker.RunWorkerAsync();
        }

        private void BackgroundWorkerOnDoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var worker = (BackgroundWorker) sender;
                var ftpRequest = CreateFtpRequest();
                var ftpStream = ftpRequest.GetRequestStream();

                using (Stream source = File.OpenRead(_localFullPath))
                {
                    var buffer = new byte[2048];
                    int bytesRead;
                    long progress = 0;
                    while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        if (worker.CancellationPending)
                        {
                            e.Result = Result.Cancelled;
                            ftpStream.Close();
                            ftpStream.Dispose();
                            return;
                        }

                        ftpStream.Write(buffer, 0, bytesRead);
                        progress += bytesRead;
                        worker.ReportProgress((int) (progress / source.Length * 100));
                    }
                }

                ftpStream.Close();
                ftpStream.Dispose();
                e.Result = Result.Success;
            }
            catch(Exception exception)
            {
                e.Result = Result.Error;
            }
        }

        private void BackgroundWorkerOnProgressChanged(object sender, ProgressChangedEventArgs progressChangedEventArgs)
        {
            throw new NotImplementedException();
        }

        private void BackgroundWorkerOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            return;
            throw new NotImplementedException();
        }

        private FtpWebRequest CreateFtpRequest()
        {
            var file = new FileInfo(_localFullPath);
            var settings = SettingsLoader.LoadSettings();
            var ftpRequest = (FtpWebRequest)WebRequest.Create(settings.FtpTargetPath + "//" + file.Name);

            ftpRequest.Credentials = new NetworkCredential(settings.FtpUsername, settings.FtpPassword);
            ftpRequest.KeepAlive = false;
            ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;

            return ftpRequest;
        }
    }
}
