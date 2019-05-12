using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using FtpFileRegistry.Models;
using FtpFileRegistry.Properties;

namespace FtpFileRegistry.Utils
{
    public static class SettingsLoader
    {
        public static void SaveSettings(SettingsModel settings)
        {
            foreach (var property in typeof(SettingsModel).GetProperties())
            {
                Settings.Default[property.Name] = (string) property.GetValue(settings);
            }
        }

        public static SettingsModel LoadSettings()
        {
            var settings = new SettingsModel();
            foreach (var property in typeof(SettingsModel).GetProperties())
            {
                property.SetValue(settings, Settings.Default[property.Name]);
            }

            return settings;
        }
    }
}
