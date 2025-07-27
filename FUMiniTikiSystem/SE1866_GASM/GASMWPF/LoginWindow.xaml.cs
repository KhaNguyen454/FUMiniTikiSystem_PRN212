using System;
using System.Windows;
using System.Windows.Input;
using DataAccessLayer.Entities;
using DataAccessLayer; // Dùng DataAccessLayer để truy cập IRepository và GenericRepository
using Microsoft.EntityFrameworkCore; // Cần cho .FirstOrDefaultAsync() và DbContextOptionsBuilder
using Microsoft.Extensions.Configuration; // Cần để đọc appsettings.json
using System.IO; // Cần để lấy đường dẫn hiện tại
using System.Linq; // Cần cho .FirstOrDefaultAsync()
using System.Threading.Tasks; // Cần cho async/await

namespace GASMWPF
{
    public partial class LoginWindow : Window
    {
        private readonly IRepository<Customer> _customerRepository; // Đảm bảo khai báo ở đây

        public LoginWindow()
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
        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text;
            string password = txtPassword.Password;

            // 1. Kiểm tra dữ liệu nhập vào (Validation cơ bản)
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

            // 2. Logic xác thực đăng nhập (SỬ DỤNG REPOSITORY VÀ ASYNC)
            try
            {
                Customer authenticatedCustomer = null;

                // Kiểm tra tài khoản admin đặc biệt từ appsettings.json
                // LƯU Ý: Trong môi trường thực tế, tài khoản admin cũng nên được quản lý trong DB và hash mật khẩu.
                // Việc hardcode ở đây chỉ để phục vụ mục đích test nhanh.
                if (email == "admin@FUMiniTikiSystem.com" && password == "@@abc123@@")
                {
                    authenticatedCustomer = new Customer { Email = email, Name = "Admin", Password = password };
                }
                else
                {
                    // Lấy Customer từ database bằng Email và Password
                    // QUAN TRỌNG: Trong thực tế, bạn sẽ hash mật khẩu được nhập vào
                    // và so sánh với mật khẩu đã hash trong DB.
                    authenticatedCustomer = await _customerRepository
                                            .FindByConditionAsync(c => c.Email == email && c.Password == password)
                                            .Result.FirstOrDefaultAsync();
                }

                if (authenticatedCustomer != null)
                {
                    MessageBox.Show("Đăng nhập thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                    // TODO: Nếu có vai trò (Role), bạn có thể kiểm tra authenticatedCustomer.Role ở đây
                    // Ví dụ: if (authenticatedCustomer.Role == "Admin") { /* Mở Admin Dashboard */ }

                    // Chuyển sang MainApplicationWindow và đóng LoginWindow
                    MainApplicationWindow mainAppWindow = new MainApplicationWindow();
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
                MessageBox.Show($"Đã xảy ra lỗi khi đăng nhập: {ex.Message}\nChi tiết: {ex.InnerException?.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RegisterLink_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.Show();
            this.Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}