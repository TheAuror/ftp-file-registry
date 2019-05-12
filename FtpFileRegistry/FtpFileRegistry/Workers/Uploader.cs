using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FtpFileRegistry.Workers
{
    public class Uploader
    {
        private readonly BackgroundWorker _backgroundWorker = new BackgroundWorker();

        public Uploader()
        {
            _backgroundWorker.DoWork += BackgroundWorkerOnDoWork;
            _backgroundWorker.RunWorkerCompleted += BackgroundWorkerOnRunWorkerCompleted;
        }

        public void Start()
        {
            

        }

        private void BackgroundWorkerOnDoWork(object sender, DoWorkEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BackgroundWorkerOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
