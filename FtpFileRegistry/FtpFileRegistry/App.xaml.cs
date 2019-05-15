using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Windows;
using FtpFileRegistry.Properties;
using FtpFileRegistry.Utils;
using WinContextMenu;

namespace FtpFileRegistry
{
    public partial class App
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var applicationPath = System.Reflection.Assembly.GetEntryAssembly()?.Location;
            var applicationName = Path.GetFileNameWithoutExtension(applicationPath);
            var isAlreadyRunning = Process.GetProcessesByName(applicationName).Length > 1;

            if (isAlreadyRunning)
            {
                if (e.Args.Length == 0) return;
                MessageRunningProcess(e.Args.ElementAt(0));
                Process.GetCurrentProcess().Kill();
            }
            ContextMenuRegister.Register();
            UriRegister.Register();
            LedgerManager.LoadLedger();
        }

        private static void MessageRunningProcess(string filePath)
        {
            var client = new TcpClient("localhost", 8889);
            using (var sw = new StreamWriter(client.GetStream()))
            {
                sw.Write(filePath);
            }
        }
    }
}
