using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Windows;
using FtpFileRegistry.Models;
using FtpFileRegistry.Properties;
using WinContextMenu;

namespace FtpFileRegistry
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private string _applicationName;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var applicationPath = System.Reflection.Assembly.GetEntryAssembly()?.Location;
            _applicationName = System.IO.Path.GetFileNameWithoutExtension(applicationPath);
            var applicationAlreadyRunning = System.Diagnostics.Process.GetProcessesByName(_applicationName).Length > 1;

            if (applicationAlreadyRunning)
            {
                MessageRunningProcess(e.Args.ElementAt(0));
                Process.GetCurrentProcess().Kill();
            }

            ContextMenuRegister.Register();
        }

        private void MessageRunningProcess(string filePath)
        {
            var client = new TcpClient("localhost", 8889);
            using (var sw = new StreamWriter(client.GetStream()))
            {
                sw.Write(filePath);
            }
        }
    }
}
