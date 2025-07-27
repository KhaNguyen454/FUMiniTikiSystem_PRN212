using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using DataAccessLayer.Entities;
using DataAccessLayer; // Dùng DataAccessLayer để truy cập IRepository và GenericRepository
using Microsoft.EntityFrameworkCore; // Cần cho .AnyAsync() và DbContextOptionsBuilder
using Microsoft.Extensions.Configuration; // Cần để đọc appsettings.json
using System.IO; // Cần để lấy đường dẫn hiện tại
using System.Linq; // Cần cho .AnyAsync()
using System.Threading.Tasks; // Cần cho async/await

namespace GASMWPF
{
    public partial class RegisterWindow : Window
    {
        // Khai báo một instance của Customer Repository
        private readonly IRepository<Customer> _customerRepository; // Đảm bảo khai báo ở đây

        public RegisterWindow()
        {
            InitializeComponent();
            this.MouseLeftButtonDown += (sender, e) => {
                if (e.ButtonState == MouseButtonState.Pressed)
                {
                    this.DragMove();
                }
            };

            // ---- KHỞI TẠO REPOSITORY VÀ DB CONTEXT ĐÚNG CÁCH ----
            // 1. Đọc Connection String từ appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory) // Đảm bảo đọc từ thư mục chạy ứng dụng WPF
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
            string connectionString = configuration.GetConnectionString("FUMiniTikiDB"); // Tên ConnectionString của bạn

            if (string.IsNullOrEmpty(connectionString))
            {
                // Fallback nếu không tìm thấy (chỉ cho dev/test)
                connectionString = "Server=(local);Database=FUMiniTikiSystem;Uid=sa;Pwd=12345;TrustServerCertificate=True";
                MessageBox.Show("Cảnh báo: Không tìm thấy ConnectionString 'FUMiniTikiDB' trong appsettings.json. Đang sử dụng chuỗi mặc định.", "Cấu hình", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            // 2. Tạo DbContextOptions
            var optionsBuilder = new DbContextOptionsBuilder<FUMiniTikiSystemDBContext>();
            optionsBuilder.UseSqlServer(connectionString);

            // 3. Khởi tạo Repository với DbContext đã cấu hình Options
            _customerRepository = new GenericRepository<Customer>(new FUMiniTikiSystemDBContext(optionsBuilder.Options));
        }

        // Đảm bảo phương thức là async void
        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string name = txtName.Text;
            string email = txtEmail.Text;
            string password = txtPassword.Password;
            string confirmPassword = txtConfirmPassword.Password;

            // 1. Validation dữ liệu
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
            if (!IsValidEmail(email))
            {
                MessageBox.Show("Email không hợp lệ.", "Lỗi Đăng Ký", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtEmail.Focus();
                return;
            }

            // THÊM: Kiểm tra email đã tồn tại chưa trong database SỬ DỤNG REPOSITORY VÀ ASYNC
            try
            {
                // Sử dụng FindByConditionAsync và sau đó .AnyAsync() để kiểm tra sự tồn tại
                // .Result ở đây là cần thiết để lấy IQueryable<TEntity> từ Task<IQueryable<TEntity>>
                // trước khi gọi .AnyAsync()
                bool emailExists = await _customerRepository.FindByConditionAsync(c => c.Email == email).Result.AnyAsync();
                if (emailExists)
                {
                    MessageBox.Show("Email này đã được đăng ký. Vui lòng sử dụng Email khác.", "Lỗi Đăng Ký", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtEmail.Focus();
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi kiểm tra Email: {ex.Message}\nChi tiết: {ex.InnerException?.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    Password = password, // LƯU Ý QUAN TRỌNG: TRONG THỰC TẾ, BẠN PHẢI HASH MẬT KHẨU Ở ĐÂY TRƯỚC KHI LƯU VÀO DB.
                };

                // 3. GỌI REPOSITORY ĐỂ THÊM VÀO DATABASE BẰNG ADDASYNC VÀ AWAIT
                await _customerRepository.AddAsync(newCustomer);

                MessageBox.Show("Đăng ký tài khoản thành công! Vui lòng đăng nhập.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                // Quay lại màn hình đăng nhập
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi đăng ký: {ex.Message}\nChi tiết: {ex.InnerException?.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                // Ghi log lỗi chi tiết hơn trong môi trường thực tế để debug dễ hơn
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