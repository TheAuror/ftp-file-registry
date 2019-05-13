using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using FtpFileRegistry.Models;
using FtpFileRegistry.Utils;
using FtpFileRegistry.Workers;

namespace FtpFileRegistry.Views
{
    public partial class SettingsWindow
    {
        private readonly SettingsModel _settingsModel = SettingsLoader.LoadSettings();

        public SettingsWindow()
        {
            InitializeComponent();
            SubscribeForEvents();
            DataContext = _settingsModel;
            Hide();
            new Listener().Start();
        }

        private void SubscribeForEvents()
        {
            Closing += OnClosing;
            ExitMenuItem.Click += ExitMenuItem_OnClick;
            SettingsMenuItem.Click += SettingsMenuItem_OnClick;
        }

        private void OnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            Hide();
            cancelEventArgs.Cancel = true;
        }

        private void SettingsMenuItem_OnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            var desktopWorkingArea = SystemParameters.WorkArea;
            Left = desktopWorkingArea.Right - Width;
            Top = desktopWorkingArea.Bottom - Height;
            SettingsLoader.LoadSettings(_settingsModel);
            Show();
        }

        private void ExitMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            SettingsLoader.SaveSettings(_settingsModel);
            Hide();
        }
    }
}
