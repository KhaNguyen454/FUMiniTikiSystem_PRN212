// File: GASMWPF/App.xaml.cs
using System;
using System.Configuration;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using BusinessLogicLayer.Services;
using DataAccessLayer.Repositories;
using DataAccessLayer; // Namespace chứa FUMiniTikiSystemDBContext
using GASMWPF.CustomerWindow;
using Microsoft.Extensions.Configuration;
using GASMWPF.Admin; // Thêm namespace này cho CustomerManagementWindow

namespace GASMWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost _host;

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, builder) =>
                {
                    builder.AddConfiguration(AppConfiguration.Configuration);
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddDbContext<FUMiniTikiSystemDBContext>(options =>
                        options.UseSqlServer(AppConfiguration.GetConnectionString("FUMiniTikiDB")));

                    services.AddScoped<ICustomerRepository, CustomerRepository>();
                    services.AddScoped<ICustomerService, CustomerService>();

                    services.AddTransient<LoginWindow>();
                    services.AddTransient<CustomerDashboardWindow>();
                    services.AddTransient<CustomerProfileWindow>();
                    services.AddTransient<RegisterWindow>();
                    services.AddTransient<MainApplicationWindow>();
                    services.AddTransient<CustomerManagementWindow>(); 
                })
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            await _host.StartAsync();
            var loginWindow = _host.Services.GetRequiredService<LoginWindow>();
            loginWindow.Show();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            await _host.StopAsync();
            _host.Dispose();
        }
    }
}