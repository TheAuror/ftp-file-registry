using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Windows;
using FtpFileRegistry.Models;
using FtpFileRegistry.Utils;

namespace FtpFileRegistry.Workers
{
    public class Downloader : WorkerWithProgress
    {
        private readonly string _localFolderPath;
        private readonly string _ftpFullPath;
        private readonly BackgroundWorker _backgroundWorker = new BackgroundWorker();
        public enum Result { InProgress, Error, Success, Cancelled }
        public Result DownloadResult = Result.InProgress;

        public Downloader(string localFolderPath, string ftpFullPath)
        {
            _localFolderPath = localFolderPath;
            _ftpFullPath = ftpFullPath;
            _backgroundWorker.DoWork += BackgroundWorkerOnDoWork;
            _backgroundWorker.ProgressChanged += BackgroundWorkerOnProgressChanged;
            _backgroundWorker.RunWorkerCompleted += BackgroundWorkerOnRunWorkerCompleted;
            _backgroundWorker.WorkerReportsProgress = true;

            ProgressModel.ProgressName = "Downloading";
            CreateProgress();
        }

        public void Start()
        {
            _backgroundWorker.RunWorkerAsync();
        }

        private void BackgroundWorkerOnDoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var worker = (BackgroundWorker)sender;
                var ftpRequest = CreateFtpRequest();
                using (var ftpStream = ftpRequest.GetResponse().GetResponseStream())
                using (Stream fileStream = File.OpenWrite(Path.Combine(_localFolderPath, Path.GetFileName(_ftpFullPath))))
                {
                    var buffer = new byte[1024 * 1024 * 2];
                    int bytesRead;
                    long progress = 0;
                    double fileSize = ftpRequest.ContentLength;
                    if (ftpStream == null) throw new Exception("ftpStream was null");
                    while ((bytesRead = ftpStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        if (worker.CancellationPending)
                        {
                            e.Result = Result.Cancelled;
                            ftpStream.Close();
                            ftpStream.Dispose();
                            return;
                        }

                        fileStream.Write(buffer, 0, bytesRead);
                        progress += bytesRead;
                        var percentProgress = (int)(progress / fileSize * 100);
                        worker.ReportProgress(percentProgress);
                    }
                    e.Result = Result.Success;
                }
            }
            catch (Exception exception)
            {
                e.Result = exception;
            }
        }

        private void BackgroundWorkerOnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (ProgressModel.Progress != e.ProgressPercentage)
            {
                ProgressModel.Progress = e.ProgressPercentage;
                UpdateProgress();
            }
        }

        private void BackgroundWorkerOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result is Result)
            {
                var result = (Result)(e.Result ?? Result.Error);
                if (result == Result.Success)
                {
                    ProgressModel.Status = ProgressModel.ProgressStatus.Complete;
                    ProgressModel.Progress = 100;
                    ProgressModel.ProgressName = "Complete";
                }
                DownloadResult = Result.Success;
            }
            var exception = e.Result as WebException;
            if (exception != null)
            {
                var messageBoxText = ((FtpWebResponse)exception.Response).StatusDescription;
                if (messageBoxText != null)
                {
                    MessageBox.Show(messageBoxText, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                ProgressModel.Status = ProgressModel.ProgressStatus.Cancelled;
                ProgressModel.Progress = 100;
                ProgressModel.ProgressName = "Error";
                DownloadResult = Result.Error;
            }

            UpdateProgress();
        }

        private FtpWebRequest CreateFtpRequest()
        {
            var settings = SettingsLoader.LoadSettings();
            var ftpRequest = (FtpWebRequest)WebRequest.Create(_ftpFullPath);

            ftpRequest.Credentials = new NetworkCredential(settings.FtpUsername, settings.FtpPassword);
            ftpRequest.KeepAlive = false;
            ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;

            return ftpRequest;
        }
    }
}
