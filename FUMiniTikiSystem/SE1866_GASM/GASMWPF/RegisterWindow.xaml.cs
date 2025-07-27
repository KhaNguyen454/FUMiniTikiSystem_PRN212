using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using DataAccessLayer.Entities; // Sử dụng Entities từ DataAccessLayer
using BusinessLogicLayer.Services; // Sử dụng Services từ BusinessLogicLayer
using System.Threading.Tasks;

namespace GASMWPF // Vẫn ở namespace gốc
{
    public partial class RegisterWindow : Window
    {
        private readonly ICustomerService _customerService; // Khai báo Service Layer

        public RegisterWindow()
        {
            InitializeComponent();
            this.MouseLeftButtonDown += (sender, e) => {
                if (e.ButtonState == MouseButtonState.Pressed)
                {
                    this.DragMove();
                }
            };
            _customerService = new CustomerService(); // Khởi tạo Service Layer
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string name = txtName.Text;
            string email = txtEmail.Text;
            string password = txtPassword.Password;
            string confirmPassword = txtConfirmPassword.Password;

            // 1. Validation dữ liệu (một số được xử lý ở đây, một số ở Service Layer)
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
                // 2. Tạo đối tượng Customer mới
                Customer newCustomer = new Customer
                {
                    Name = name,
                    Email = email,
                    Password = password, // LƯU Ý: Trong thực tế, bạn PHẢI hash mật khẩu ở đây hoặc trong Service Layer
                };

                // 3. GỌI SERVICE LAYER ĐỂ ĐĂNG KÝ
                bool success = await _customerService.RegisterAsync(newCustomer);

                if (success)
                {
                    MessageBox.Show("Đăng ký tài khoản thành công! Vui lòng đăng nhập.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    // Quay lại màn hình đăng nhập
                    LoginWindow loginWindow = new LoginWindow(); // Vẫn tham chiếu đến LoginWindow ở gốc
                    loginWindow.Show();
                    this.Close();
                }
                else
                {
                    // Trường hợp RegisterAsync trả về false nhưng không ném ngoại lệ
                    MessageBox.Show("Đăng ký thất bại. Vui lòng thử lại.", "Lỗi Đăng Ký", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (ArgumentException argEx) // Bắt các lỗi validation từ Service Layer
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
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
