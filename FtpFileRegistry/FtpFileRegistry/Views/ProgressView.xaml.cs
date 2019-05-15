using System.Collections.Generic;
using System.Linq;
using System.Windows;
using FtpFileRegistry.Controls;
using FtpFileRegistry.Models;
using FtpFileRegistry.ViewModels;

namespace FtpFileRegistry.Views
{
    public partial class ProgressView
    {
        public ProgressViewModel ViewModel { get; }
        public Dictionary<ProgressModel, ProgressControl> ProgressLedger;

        public ProgressView()
        {
            InitializeComponent();
            ProgressLedger = new Dictionary<ProgressModel, ProgressControl>();
            ViewModel = new ProgressViewModel();
            Hide();
        }

        public void CreateProgressControl(ProgressModel progressModel)
        {
            var progressControl = new ProgressControl(progressModel);
            StackPanel.Children.Add(progressControl);
            ProgressLedger.Add(progressModel, progressControl);
            ResizeWindow();
            RepositonWindow();
            Show();
        }

        public void UpdateProgressControl(ProgressModel progressModel)
        {
            var progressControl = ProgressLedger[progressModel];
            progressControl.UpdateProgress(progressModel);
            if (progressModel.Status != ProgressModel.ProgressStatus.Running)
            {
                RemoveProgressControl(progressModel);
            }
        }

        private void RemoveProgressControl(ProgressModel progressModel)
        {
            StackPanel.Children.Remove(ProgressLedger[progressModel]);
            ProgressLedger.Remove(progressModel);
            if (!ProgressLedger.Any())
            {
                Hide();
                return;
            }
            ResizeWindow();
            RepositonWindow();
        }

        private void RepositonWindow()
        {
            var desktopWorkingArea = SystemParameters.WorkArea;
            Left = desktopWorkingArea.Right - Width;
            Top = desktopWorkingArea.Bottom - Height;
        }

        private void ResizeWindow()
        {
            Width = 250;
            Height = ProgressLedger.Count * 49;
        }
    }
}
