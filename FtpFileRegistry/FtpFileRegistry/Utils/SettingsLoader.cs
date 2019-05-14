using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;
using FtpFileRegistry.Models;
using FtpFileRegistry.Properties;

namespace FtpFileRegistry.Utils
{
    public static class SettingsLoader
    {
        private static readonly string SettingsLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.xml");

        public static void SaveSettings(SettingsModel settings)
        {
            using (var streamWriter = new StreamWriter(SettingsLocation))
            {
                var serializer = new XmlSerializer(typeof(SettingsModel));
                serializer.Serialize(streamWriter, settings);
            }
        }

        public static SettingsModel LoadSettings(SettingsModel settingsModel = null)
        {
            if(!File.Exists(SettingsLocation))
                SaveSettings(new SettingsModel());
            using (var streamReader = new StreamReader(SettingsLocation))
            {
                var serializer = new XmlSerializer(typeof(SettingsModel));
                return serializer.Deserialize(streamReader) as SettingsModel;
            }
        }
    }
}
