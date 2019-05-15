using FtpFileRegistry.Models;

namespace FtpFileRegistry.Controls
{
    /// <summary>
    /// Interaction logic for ProgressControl.xaml
    /// </summary>
    public partial class ProgressControl
    {
        public ProgressModel ProgressModel;
        private string _dots = "";
        
        public ProgressControl(ProgressModel progressModel)
        {
            ProgressModel = progressModel;
            InitializeComponent();
            DataContext = ProgressModel;
        }

        public void UpdateProgress(ProgressModel progressModel)
        {
            ProgressModel = progressModel;
            ProgressBar.Value = ProgressModel.Progress;
            ProgressLabel.Content = ProgressModel.ProgressName+GetDots();
        }

        private string GetDots()
        {
            _dots += ".";
            if (_dots == "....")
                _dots = "";
            return _dots;
        }
    }
}
