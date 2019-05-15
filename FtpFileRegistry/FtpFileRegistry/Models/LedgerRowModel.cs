using System;

namespace FtpFileRegistry.Models
{
    public class LedgerRowModel
    {
        public string Username { get; set; }
        public string FileName { get; set; }
        public string FileHash { set; get; }
        public string FileIdentifier { get; set; }
        public DateTime UploadDateTime { get; set; }
        public DateTime LastDownloadDateTime { get; set; }
    }
}
