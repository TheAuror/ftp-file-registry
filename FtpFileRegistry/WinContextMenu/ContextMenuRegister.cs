using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace WinContextMenu
{
    public static class ContextMenuRegister
    {
        public const string CommandName = "Register to FTP";

        public static bool Register()
        {
            UnregisterForRar();
            return RegisterForRar();
        }

        private static void UnregisterForRar()
        {
            var registryKey = Registry.ClassesRoot.OpenSubKey(".rar");
            var rarHandlerKey = registryKey?.GetValue(string.Empty) as string;

            if (rarHandlerKey == null) return;

            registryKey = Registry.ClassesRoot
                .OpenSubKey(rarHandlerKey)?
                .OpenSubKey("shell", RegistryKeyPermissionCheck.ReadWriteSubTree);

            var subKeyNames = registryKey?
                .GetSubKeyNames();

            if (subKeyNames != null && subKeyNames.Contains(CommandName))
            {
                registryKey.DeleteSubKeyTree(CommandName);
            }
        }

        private static bool RegisterForRar()
        {
            var applicationPath = System.Reflection.Assembly.GetEntryAssembly()?.Location;
            var registryKey = Registry.ClassesRoot.OpenSubKey(".rar");
            var rarHandlerKey = registryKey?.GetValue(string.Empty) as string;

            if (string.IsNullOrEmpty(rarHandlerKey)) return false;
            if (string.IsNullOrEmpty(applicationPath)) return false;

            registryKey = Registry.ClassesRoot
                .OpenSubKey(rarHandlerKey)?
                .OpenSubKey("shell", RegistryKeyPermissionCheck.ReadWriteSubTree);

            registryKey = registryKey?.CreateSubKey(CommandName);

            registryKey?.SetValue("Icon", applicationPath);
            registryKey?.CreateSubKey("command")?.SetValue(string.Empty, "\"" + applicationPath + "\" \"%1\"");

            return true;
        }
    }
}
