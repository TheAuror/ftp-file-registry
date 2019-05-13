using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
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
                var settingsProperty = Settings.Default.Properties[property.Name];
                if(settingsProperty == null)
                    CreateSettingsProperty(property.Name);

                settingsProperty = Settings.Default.Properties[property.Name];
                if (settingsProperty != null)
                    settingsProperty.DefaultValue = (string) property.GetValue(settings);
            }
            Settings.Default.Save();
            Settings.Default.Reload();
        }

        public static SettingsModel LoadSettings()
        {
            var settings = new SettingsModel();
            foreach (var property in typeof(SettingsModel).GetProperties())
            {
                property.SetValue(settings, Settings.Default.Properties[property.Name]?.DefaultValue);
            }

            return settings;
        }

        public static void CreateSettingsProperty(string name)
        {
            var property = new SettingsProperty(name)
            {
                DefaultValue = "",
                IsReadOnly = false,
                PropertyType = typeof(string),
                Provider = Settings.Default.Providers["LocalFileSettingsProvider"]
            };
            property.Attributes.Add(typeof(UserScopedSettingAttribute), new UserScopedSettingAttribute());
            Settings.Default.Properties.Add(property);

            Settings.Default.Reload();
        }
    }
}
