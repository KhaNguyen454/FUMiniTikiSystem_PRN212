using System.Windows;
using DataAccessLayer.Entities; // Sử dụng Entities từ DataAccessLayer
using GASMWPF.Admin;
using GASMWPF.CustomerWindow; // Thêm dòng này để tham chiếu đến CustomerProfileWindow

namespace GASMWPF // Vẫn ở namespace gốc
{
    /// <summary>
    /// Interaction logic for MainApplicationWindow.xaml
    /// </summary>
    public partial class MainApplicationWindow : Window
    {
        private Customer _loggedInCustomer; // Biến để lưu thông tin khách hàng đã đăng nhập

        public MainApplicationWindow(Customer customer) // Constructor nhận thông tin khách hàng
        {
            InitializeComponent();
            _loggedInCustomer = customer;
            // Hiển thị tên người dùng hoặc email trên màn hình chính nếu muốn
            // txtWelcomeMessage.Text = $"Chào mừng, {customer.Name ?? customer.Email}!";
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            // Logic đăng xuất: đóng màn hình chính và mở lại màn hình đăng nhập
            LoginWindow loginWindow = new LoginWindow(); // Vẫn tham chiếu đến LoginWindow ở gốc
            loginWindow.Show();
            this.Close(); // Đóng MainApplicationWindow
        }

        // Thêm sự kiện để mở CustomerProfileWindow
        private void OpenProfile_Click(object sender, RoutedEventArgs e)
        {
            if (_loggedInCustomer != null)
            {
                CustomerProfileWindow profileWindow = new CustomerProfileWindow(_loggedInCustomer); // CẬP NHẬT: Sử dụng namespace đầy đủ
                profileWindow.ShowDialog(); // ShowDialog để chặn tương tác với cửa sổ chính
                // Sau khi profileWindow đóng, bạn có thể refresh dữ liệu nếu cần
                // Ví dụ: LoadCustomerData();
            }
            else
            {
                MessageBox.Show("Không tìm thấy thông tin người dùng.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenCustomerManagement_Click(object sender, RoutedEventArgs e)
        {
            if (_loggedInCustomer != null && _loggedInCustomer.CustomerId == -1)
            {
                CustomerManagementWindow customerManagementWindow = new CustomerManagementWindow();
                customerManagementWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Bạn không có quyền truy cập tính năng này.", "Truy cập bị từ chối", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
