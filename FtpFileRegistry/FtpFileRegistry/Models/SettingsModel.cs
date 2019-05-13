using System.Security;

namespace FtpFileRegistry.Models
{
    public class SettingsModel
    {
        public string FtpUsername { get; set; }
        public string FtpPassword { get; set; }
        public string FtpTargetPath { get; set; }
    }
}
