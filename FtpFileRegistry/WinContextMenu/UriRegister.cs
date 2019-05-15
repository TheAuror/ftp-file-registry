using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace WinContextMenu
{
    public static class UriRegister
    {
        private const string UriName = "ftpRegistry";
        public static void Register()
        {
            UnregisterUri();
            RegisterUri();
        }

        private static void UnregisterUri()
        {
            Registry.ClassesRoot.DeleteSubKeyTree(UriName, false);
        }

        private static void RegisterUri()
        {
            var applicationPath = System.Reflection.Assembly.GetEntryAssembly()?.Location;

            var registryKey = Registry.ClassesRoot.CreateSubKey(UriName);
            registryKey?.SetValue("URL Protocol", string.Empty);
            registryKey = registryKey?
                .CreateSubKey("shell")?
                .CreateSubKey("open")?
                .CreateSubKey("command");
            registryKey?.SetValue(string.Empty, "\"" + applicationPath + "\" \"%1\"");
        }
    }
}
