using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Input;
using System.Windows;

namespace GASMWPF.CustomerWindow
{
    /// <summary>
    /// Interaction logic for CustomerDashboardWindow.xaml
    /// </summary>
    public partial class CustomerDashboardWindow : Window
    {
        private CustomerDTO? _loggedInCustomer; // Có thể null nếu chưa được thiết lập
        private readonly IServiceProvider _serviceProvider; // Để tạo các cửa sổ khác

        // CHỈ NHẬN CÁC DEPENDENCY TỪ DI, KHÔNG NHẬN TRỰC TIẾP CustomerDTO
        public CustomerDashboardWindow(IServiceProvider serviceProvider) // Đã bỏ CustomerDTO customerDto
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;

            // Cho phép kéo cửa sổ bằng cách giữ chuột trái
            this.MouseLeftButtonDown += (sender, e) => {
                if (e.ButtonState == MouseButtonState.Pressed)
                {
                    this.DragMove();
                }
            };
        }

        // Phương thức công khai để thiết lập CustomerDTO sau khi cửa sổ được tạo
        public void SetLoggedInCustomer(CustomerDTO customerDto)
        {
            _loggedInCustomer = customerDto;
            // Hiển thị tên khách hàng trên bảng điều khiển
            if (WelcomeTextBlock != null && _loggedInCustomer != null)
            {
                WelcomeTextBlock.Text = $"Chào mừng, {_loggedInCustomer.Name}!";
            }
        }

        private async void MyProfile_Click(object sender, RoutedEventArgs e)
        {
            if (_loggedInCustomer == null)
            {
                MessageBox.Show("Không tìm thấy thông tin người dùng. Vui lòng đăng nhập lại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Mở cửa sổ hồ sơ khách hàng
            var profileWindow = _serviceProvider.GetRequiredService<CustomerProfileWindow>();

            // Truyền DTO hiện tại. CustomerProfileWindow cần có một phương thức SetCustomerDTO.
            profileWindow.SetCustomerDTO(_loggedInCustomer);

            var result = profileWindow.ShowDialog(); // Hiển thị dưới dạng Dialog

            // Sau khi ProfileWindow đóng, nếu có thay đổi và DialogResult là true,
            // cập nhật lại thông tin hiển thị trên Dashboard
            if (result == true)
            {
                try
                {
                    // Lấy lại service để đảm bảo luôn có phiên bản mới nhất
                    var customerService = _serviceProvider.GetRequiredService<ICustomerService>();
                    // Fetch lại thông tin khách hàng để đảm bảo _loggedInCustomer được cập nhật
                    _loggedInCustomer = await customerService.GetCustomerByIdAsync(_loggedInCustomer.CustomerId) ?? _loggedInCustomer;

                    if (WelcomeTextBlock != null && _loggedInCustomer != null)
                    {
                        WelcomeTextBlock.Text = $"Chào mừng, {_loggedInCustomer.Name}!"; // Cập nhật tên
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi tải lại thông tin hồ sơ: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = _serviceProvider.GetRequiredService<LoginWindow>(); 
            loginWindow.Show();
            this.Close(); 
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}