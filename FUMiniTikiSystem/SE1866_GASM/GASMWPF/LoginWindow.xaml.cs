// File: GASMWPF/LoginWindow.xaml.cs (Đã sửa)
using System;
using System.Windows;
using System.Windows.Input;
using BusinessLogicLayer.Services;
using BusinessLogicLayer.DTOs;
using Microsoft.Extensions.DependencyInjection;
using GASMWPF.CustomerWindow;
using GASMWPF.Admin; // Đảm bảo đã có namespace này cho CustomerManagementWindow

namespace GASMWPF
{
    public partial class LoginWindow : Window
    {
        private readonly ICustomerService _customerService;
        private readonly IServiceProvider _serviceProvider;

        public LoginWindow(ICustomerService customerService, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _customerService = customerService;
            _serviceProvider = serviceProvider;

            this.MouseLeftButtonDown += (sender, e) => {
                if (e.ButtonState == MouseButtonState.Pressed)
                {
                    this.DragMove();
                }
            };
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text;
            string password = txtPassword.Password;

            if (string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Vui lòng nhập Email.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtEmail.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Vui lòng nhập Mật khẩu.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPassword.Focus();
                return;
            }

            try
            {
                CustomerDTO? authenticatedCustomer = await _customerService.LoginAsync(email, password);

                if (authenticatedCustomer != null)
                {
                    MessageBox.Show("Đăng nhập thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                    if (authenticatedCustomer.IsAdmin)
                    {
                        // Đối với admin, giả định MainApplicationWindow đã được cấu hình đúng
                        MainApplicationWindow mainAppWindow = _serviceProvider.GetRequiredService<MainApplicationWindow>();
                        mainAppWindow.SetLoggedInCustomer(authenticatedCustomer); // Giả sử MainApplicationWindow có phương thức này
                        mainAppWindow.Show();
                    }
                    else
                    {
                        // Lấy CustomerDashboardWindow từ DI container
                        CustomerDashboardWindow customerDashboardWindow = _serviceProvider.GetRequiredService<CustomerDashboardWindow>();

                        // Gọi phương thức SetLoggedInCustomer để truyền dữ liệu
                        customerDashboardWindow.SetLoggedInCustomer(authenticatedCustomer); // <--- ĐÃ SỬA DÒNG NÀY

                        customerDashboardWindow.Show();
                    }
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Email hoặc mật khẩu không đúng.", "Lỗi Đăng Nhập", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi đăng nhập: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RegisterLink_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow registerWindow = _serviceProvider.GetRequiredService<RegisterWindow>();
            registerWindow.Show();
            this.Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}