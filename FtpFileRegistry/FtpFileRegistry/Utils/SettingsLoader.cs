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
            CreateSettingsProperties();
            foreach (var property in typeof(SettingsModel).GetProperties())
            {
                var settingsProperty = Settings.Default.Properties[property.Name];
                if (settingsProperty != null)
                    settingsProperty.DefaultValue = (string) property.GetValue(settings);
            }
            Settings.Default.Save();
        }

        public static SettingsModel LoadSettings(SettingsModel settingsModel = null)
        {
            CreateSettingsProperties();
            if (settingsModel == null)
                settingsModel = new SettingsModel();

            foreach (var property in typeof(SettingsModel).GetProperties())
            {
                property.SetValue(settingsModel, Settings.Default.Properties[property.Name]?.DefaultValue);
            }

            return settingsModel;
        }

        private static void CreateSettingsProperties()
        {
            foreach (var property in typeof(SettingsModel).GetProperties())
            {
                var settingsProperty = Settings.Default.Properties[property.Name];
                if (settingsProperty == null)
                    CreateSettingsProperty(property.Name);
            }
        }

        private static void CreateSettingsProperty(string name)
        {
            var property = new SettingsProperty(name)
            {
                PropertyType = typeof(string),
                Provider = Settings.Default.Providers["LocalFileSettingsProvider"]
            };
            property.Attributes.Add(typeof(UserScopedSettingAttribute), new UserScopedSettingAttribute());
            Settings.Default.Properties.Add(property);
        }
    }
}
