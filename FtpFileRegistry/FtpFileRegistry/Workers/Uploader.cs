using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Windows;
using FtpFileRegistry.Helpers;
using FtpFileRegistry.Models;
using FtpFileRegistry.Utils;

namespace FtpFileRegistry.Workers
{
    public class Uploader : WorkerWithProgress
    {
        private readonly string _localFullPath;
        private readonly bool _storeInLedger;
        private readonly BackgroundWorker _backgroundWorker = new BackgroundWorker();
        public enum Result { InProgress, Error, Success, Cancelled }
        public Result UploadResult = Result.InProgress;

        public Uploader(string localFullPath, bool storeInLedger = true)
        {
            _localFullPath = localFullPath;
            _storeInLedger = storeInLedger;
            _backgroundWorker.DoWork += BackgroundWorkerOnDoWork;
            _backgroundWorker.ProgressChanged += BackgroundWorkerOnProgressChanged;
            _backgroundWorker.RunWorkerCompleted += BackgroundWorkerOnRunWorkerCompleted;
            _backgroundWorker.WorkerReportsProgress = true;

            FileInfo file = new FileInfo(localFullPath);
            ProgressModel.ProgressName = "Uploading " + file.Name.Substring(0, Math.Min(file.Name.Length, 20));
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
                var worker = (BackgroundWorker) sender;
                var ftpRequest = CreateFtpRequest();
                var ftpStream = ftpRequest.GetRequestStream();

                using (Stream fileStream = File.OpenRead(_localFullPath).WainUntilReady())
                {
                    var buffer = new byte[1024*1024*2];
                    int bytesRead;
                    long progress = 0;
                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
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
                        var percentProgress = (int) ((double)progress / fileStream.Length * 100);
                        worker.ReportProgress(percentProgress);
                    }
                }

                ftpStream.Close();
                ftpStream.Dispose();
                e.Result = Result.Success;
            }
            catch(Exception exception)
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
                var result = (Result) (e.Result ?? Result.Error);
                if (result == Result.Success)
                {
                    ProgressModel.Status = ProgressModel.ProgressStatus.Complete;
                    ProgressModel.Progress = 100;
                    ProgressModel.ProgressName = "Complete";
                }
                if(_storeInLedger)
                    UpdateLedger();
                UploadResult = Result.Success;
            }
            var exception = e.Result as WebException;
            if (exception != null)
            {
                var messageBoxText = ((FtpWebResponse) exception.Response).StatusDescription;
                if (messageBoxText != null)
                    MessageBox.Show(messageBoxText, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                ProgressModel.Status = ProgressModel.ProgressStatus.Cancelled;
                ProgressModel.Progress = 100;
                ProgressModel.ProgressName = "Error";
                UploadResult = Result.Error;
            }

            UpdateProgress();
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

        private void UpdateLedger()
        {
            FileInfo file = new FileInfo(_localFullPath);
            var settings = SettingsLoader.LoadSettings();
            var ledgerRowModel = new LedgerRowModel
            {
                UploadDateTime = DateTime.Now,
                FileIdentifier = FileIdGenerator.GetIdentifier(),
                FileName = file.Name,
                LastDownloadDateTime = DateTime.MinValue,
                Username = settings.FtpUsername
            };
            LedgerManager.GetManager().AddDatabaseToLedger(ledgerRowModel);
            LedgerManager.SaveLedger();
        }
    }
}
