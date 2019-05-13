﻿using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Windows;
using FtpFileRegistry.Models;
using FtpFileRegistry.Utils;
using FtpFileRegistry.Workers;
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
                MessageRunningProcess(e.Args.ElementAt(0));
                Process.GetCurrentProcess().Kill();
            }

            ContextMenuRegister.Register();

            #if DEBUG
                SettingsLoader.SaveSettings(new SettingsModel
                {
                    FtpUsername = "test",
                    FtpPassword = "test",
                    FtpTargetPath = "ftp://localhost"
                });
            #endif
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
