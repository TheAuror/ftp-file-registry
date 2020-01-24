namespace FtpFileRegistry.Models
{
    public class ProgressModel
    {
        public enum ProgressStatus { Starting, Running, Complete, Cancelled }

        public int Progress;
        public string ProgressName;
        public ProgressStatus Status;
    }
}
