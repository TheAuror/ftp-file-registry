using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FtpFileRegistry.Helpers
{
    public class FileIdGenerator
    {
        private static readonly Random Random = new Random();
        public static string GetIdentifier()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 6).Select(s => s[Random.Next(s.Length)]).ToArray());
        }
    }
}
