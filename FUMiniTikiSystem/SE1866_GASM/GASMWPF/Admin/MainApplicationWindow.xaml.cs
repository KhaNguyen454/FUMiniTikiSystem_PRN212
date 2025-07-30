// File: GASMWPF/Admin/MainApplicationWindow.xaml.cs
using System;
using System.Windows;
using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.Services;
using GASMWPF.CustomerWindow; // Vẫn cần nếu bạn muốn truy cập CustomerProfileWindow
using Microsoft.Extensions.DependencyInjection; // Thêm để dùng GetRequiredService
// Không cần using GASMWPF.Admin ở đây vì bạn đã ở trong namespace GASMWPF.Admin

namespace GASMWPF.Admin
{
    /// <summary>
    /// Interaction logic for MainApplicationWindow.xaml
    /// </summary>
    public partial class MainApplicationWindow : Window
    {
        private CustomerDTO _loggedInCustomer; // Biến để lưu thông tin khách hàng đã đăng nhập (DTO)
        private readonly ICustomerService _customerService; // Tiêm CustomerService
        private readonly IServiceProvider _serviceProvider; // Thêm IServiceProvider

        // Constructor nhận ICustomerService và IServiceProvider thông qua Dependency Injection
        public MainApplicationWindow(ICustomerService customerService, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _customerService = customerService; // Gán service được inject
            _serviceProvider = serviceProvider; // Gán service provider
            this.Loaded += MainApplicationWindow_Loaded; // Đảm bảo _loggedInCustomer được set trước khi dùng
        }

        private void MainApplicationWindow_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateUIBasedOnCustomerRole();
        }

        // Phương thức để thiết lập thông tin khách hàng đã đăng nhập
        public void SetLoggedInCustomer(CustomerDTO customer)
        {
            _loggedInCustomer = customer;
            UpdateUIBasedOnCustomerRole();
        }

        private void UpdateUIBasedOnCustomerRole()
        {
            if (_loggedInCustomer != null)
            {
                // Các nút này chỉ admin mới thấy
                CustomerManagementButton.Visibility = _loggedInCustomer.IsAdmin ? Visibility.Visible : Visibility.Collapsed;
                ProductManagementButton.Visibility = _loggedInCustomer.IsAdmin ? Visibility.Visible : Visibility.Collapsed; // HIỂN THỊ NÚT SẢN PHẨM CHO ADMIN

                ProfileButton.Visibility = Visibility.Visible; // Luôn hiển thị nút profile nếu có người đăng nhập
                LogoutButton.Visibility = Visibility.Visible; // Luôn hiển thị nút logout nếu có người đăng nhập
            }
            else
            {
                // Nếu chưa có khách hàng đăng nhập, ẩn tất cả các nút liên quan
                CustomerManagementButton.Visibility = Visibility.Collapsed;
                ProductManagementButton.Visibility = Visibility.Collapsed;
                ProfileButton.Visibility = Visibility.Collapsed;
                LogoutButton.Visibility = Visibility.Collapsed;
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            // Logic đăng xuất: đóng màn hình chính và mở lại màn hình đăng nhập
            // Do LoginWindow không nằm trong namespace Admin, cần phải dùng namespace đầy đủ
            GASMWPF.LoginWindow loginWindow = _serviceProvider.GetRequiredService<GASMWPF.LoginWindow>();
            loginWindow.Show();
            this.Close(); // Đóng MainApplicationWindow
        }

        private void OpenProfile_Click(object sender, RoutedEventArgs e)
        {
            if (_loggedInCustomer != null)
            {
                CustomerProfileWindow profileWindow = _serviceProvider.GetRequiredService<CustomerProfileWindow>();
                // Truyền DTO cho CustomerProfileWindow
                profileWindow.SetCustomerDTO(_loggedInCustomer); // Giả định có phương thức SetCustomerDTO
                profileWindow.ShowDialog();
                // Sau khi profileWindow đóng, bạn có thể refresh dữ liệu nếu cần
                // Ví dụ: LoadCustomerData(); (nếu MainApplicationWindow có hiển thị thông tin KH)
            }
            else
            {
                MessageBox.Show("Không tìm thấy thông tin người dùng.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenCustomerManagement_Click(object sender, RoutedEventArgs e)
        {
            if (_loggedInCustomer != null && _loggedInCustomer.IsAdmin)
            {
                CustomerManagementWindow customerManagementWindow = _serviceProvider.GetRequiredService<CustomerManagementWindow>();
                customerManagementWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Bạn không có quyền truy cập tính năng này.", "Truy cập bị từ chối", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        
        private void ManageProducts_Click(object sender, RoutedEventArgs e)
        {
            if (_loggedInCustomer != null && _loggedInCustomer.IsAdmin)
            {
                try
                {
                    // Lấy ProductWindow từ DI container (nó đã nằm trong GASMWPF.Admin)
                    ProductWindow productWindow = _serviceProvider.GetRequiredService<ProductWindow>();
                    productWindow.Show(); // Hiển thị cửa sổ quản lý sản phẩm
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi mở cửa sổ quản lý sản phẩm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Bạn không có quyền truy cập tính năng này.", "Truy cập bị từ chối", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}