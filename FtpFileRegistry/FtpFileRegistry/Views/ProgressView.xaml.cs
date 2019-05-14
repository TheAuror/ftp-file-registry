using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using FtpFileRegistry.Controls;

namespace FtpFileRegistry.Views
{
    public partial class ProgressWindow : Window
    {
        private static Dictionary<BackgroundWorker, ProgressControl> _progressControls = new Dictionary<BackgroundWorker, ProgressControl>();
        private static ProgressWindow _instance;

        public ProgressWindow()
        {
            InitializeComponent();
            _instance = this;
        }

    }
}
