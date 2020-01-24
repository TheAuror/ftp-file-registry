using System;
using System.Collections.Generic;
using FtpFileRegistry.Models;
using FtpFileRegistry.Views;

namespace FtpFileRegistry.Utils
{

    public static class ProgressManager
    {
        private static bool _initialized;
        private static ProgressView _progressView;
        private static List<ProgressModel> _progressModels;

        [STAThread]
        private static void Initialize()
        {
            if (_initialized) return;

            _progressModels = new List<ProgressModel>();
            _progressView = new ProgressView();
            _initialized = true;
        }

        [STAThread]
        public static void CreateNewProgress(ProgressModel progress)
        {
            Initialize();
            _progressView.CreateProgressControl(progress);
        }

        [STAThread]
        public static void UpdateProgress(ProgressModel progress)
        {
            Initialize();
            _progressView.UpdateProgressControl(progress);
        }
    }
}
