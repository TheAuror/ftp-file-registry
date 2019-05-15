using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
