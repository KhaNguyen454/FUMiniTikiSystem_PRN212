using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using BusinessLogicLayer.Services;
using BusinessLogicLayer.DTOs; // Sử dụng DTOs
using Microsoft.Extensions.DependencyInjection; // Thêm để dùng GetRequiredService

namespace GASMWPF
{
    public partial class RegisterWindow : Window
    {
        private readonly ICustomerService _customerService;
        private readonly IServiceProvider _serviceProvider; // Thêm IServiceProvider

        // Constructor nhận ICustomerService và IServiceProvider thông qua Dependency Injection
        public RegisterWindow(ICustomerService customerService, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _customerService = customerService;
            _serviceProvider = serviceProvider; // Gán service provider

            this.MouseLeftButtonDown += (sender, e) => {
                if (e.ButtonState == MouseButtonState.Pressed)
                {
                    this.DragMove();
                }
            };
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string name = txtName.Text;
            string email = txtEmail.Text;
            string password = txtPassword.Password;
            string confirmPassword = txtConfirmPassword.Password;

            // ... (các kiểm tra hợp lệ khác, không thay đổi)
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Vui lòng nhập Tên của bạn.", "Lỗi Đăng Ký", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtName.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Vui lòng nhập Email.", "Lỗi Đăng Ký", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtEmail.Focus();
                return;
            }
            if (!IsValidEmail(email)) // Validation định dạng email ở UI
            {
                MessageBox.Show("Email không hợp lệ.", "Lỗi Đăng Ký", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtEmail.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Vui lòng nhập Mật khẩu.", "Lỗi Đăng Ký", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPassword.Focus();
                return;
            }
            if (password.Length < 6)
            {
                MessageBox.Show("Mật khẩu phải có ít nhất 6 ký tự.", "Lỗi Đăng Ký", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPassword.Focus();
                return;
            }
            if (password != confirmPassword)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp.", "Lỗi Đăng Ký", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtConfirmPassword.Focus();
                return;
            }

            try
            {
                // Tạo CustomerDTO từ dữ liệu nhập vào
                var newCustomerDto = new CustomerDTO
                {
                    Name = name,
                    Email = email,
                    Password = password // Tạm thời để ở đây cho ví dụ đăng ký
                };

                // Gọi Service Layer để đăng ký, phương thức RegisterAsync giờ trả về bool
                bool success = await _customerService.RegisterAsync(newCustomerDto);

                if (success)
                {
                    MessageBox.Show($"Đăng ký tài khoản thành công! Vui lòng đăng nhập.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Sau khi đăng ký thành công, chuyển về màn hình đăng nhập
                    LoginWindow loginWindow = _serviceProvider.GetRequiredService<LoginWindow>(); // Lấy từ ServiceProvider
                    loginWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Đăng ký tài khoản thất bại. Vui lòng thử lại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (ArgumentException argEx) // Bắt các lỗi validation từ Service Layer (như email đã tồn tại)
            {
                MessageBox.Show(argEx.Message, "Lỗi Đăng Ký", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi đăng ký: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        private void BackToLogin_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = _serviceProvider.GetRequiredService<LoginWindow>(); // Lấy từ ServiceProvider
            loginWindow.Show();
            this.Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}