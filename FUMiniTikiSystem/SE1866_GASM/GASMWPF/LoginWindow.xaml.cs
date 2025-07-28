using System;
using System.Windows;
using System.Windows.Input;
using DataAccessLayer.Entities; // Sử dụng Entities từ DataAccessLayer
using BusinessLogicLayer.Services; // Sử dụng Services từ BusinessLogicLayer
using System.Threading.Tasks;
using GASMWPF.CustomerWindow;

using CustomerEntity = DataAccessLayer.Entities.Customer; // Tạo alias để tránh xung đột tên

namespace GASMWPF // Vẫn ở namespace gốc
{
    public partial class LoginWindow : Window
    {
        private readonly ICustomerService _customerService; // Khai báo Service Layer

        public LoginWindow()
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
                // Gọi Service Layer để đăng nhập
                // Sử dụng alias CustomerEntity để rõ ràng là kiểu dữ liệu
                CustomerEntity? authenticatedCustomer = await _customerService.LoginAsync(email, password);

                // Kiểm tra null an toàn.
                if (authenticatedCustomer != null)
                {
                    MessageBox.Show("Đăng nhập thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Truyền thông tin khách hàng đã đăng nhập sang màn hình chính
                    // authenticatedCustomer đã được đảm bảo không null trong khối if này.
                    MainApplicationWindow mainAppWindow = new MainApplicationWindow(authenticatedCustomer);
                    mainAppWindow.Show();
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
            RegisterWindow registerWindow = new RegisterWindow(); // Vẫn tham chiếu đến RegisterWindow ở gốc
            registerWindow.Show();
            this.Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
