using System;
using FtpFileRegistry.Models;
using FtpFileRegistry.Utils;

namespace FtpFileRegistry.Workers
{
    public abstract class WorkerWithProgress
    {
        internal readonly ProgressModel ProgressModel;

        protected WorkerWithProgress()
        {
            ProgressModel = new ProgressModel
            {
                Progress = 0,
                Status = ProgressModel.ProgressStatus.Running
            };
        }

        [STAThread]
        internal void CreateProgress()
        {
            ProgressManager.CreateNewProgress(ProgressModel);
        }

        internal void UpdateProgress()
        {
            ProgressManager.UpdateProgress(ProgressModel);
        }
    }
}