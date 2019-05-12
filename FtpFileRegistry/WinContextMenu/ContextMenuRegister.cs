using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;

namespace WinContextMenu
{
    public static class ContextMenuRegister
    {
        private const string CommandName = "Register to FTP";
        private static readonly List<string> SupportedExtensions = 
            new List<string> {".rar", ".zip"};

        public static bool Register()
        {
            var successfulRegistration = true;

            foreach (var extension in SupportedExtensions)
            {
                UnregisterForExtension(extension);
                successfulRegistration &= RegisterForExtension(extension);
            }

            return successfulRegistration;
        }

        private static void UnregisterForExtension(string extension)
        {
            var registryKey = Registry.ClassesRoot.OpenSubKey(extension);
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

        private static bool RegisterForExtension(string extension)
        {
            var applicationPath = System.Reflection.Assembly.GetEntryAssembly()?.Location;
            var registryKey = Registry.ClassesRoot.OpenSubKey(extension);
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
