using System;
using System.ComponentModel;
using System.Windows;
using FtpFileRegistry.Workers;

namespace FtpFileRegistry.Views
{
    public partial class SettingsWindow
    {
        public SettingsWindow()
        {
            InitializeComponent();
            SubscribeForEvents();
            
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
            Show();
        }

        private void ExitMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
