﻿using System.Configuration;
using System.Data;
using System.Windows;

namespace GASMWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Khởi tạo và hiển thị LoginWindow
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
        }
    }

}
