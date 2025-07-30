// File: GASMWPF/CustomerWindow/CustomerProfileWindow.xaml.cs
using System;
using System.Text.RegularExpressions; // Thêm để dùng Regex cho email validation
using System.Windows;
using System.Windows.Input; // Thêm để xử lý kéo cửa sổ
using System.Xml.Linq; // Không sử dụng XElement, có thể bỏ nếu không cần
using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GASMWPF.CustomerWindow
{
    public partial class CustomerProfileWindow : Window
    {
        private CustomerDTO _currentCustomer;
        private readonly ICustomerService _customerService;
        private readonly IServiceProvider _serviceProvider;

        public CustomerProfileWindow(ICustomerService customerService, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _customerService = customerService;
            _serviceProvider = serviceProvider;

            // Cho phép kéo cửa sổ bằng cách giữ chuột trái
            this.MouseLeftButtonDown += (sender, e) => {
                if (e.ButtonState == MouseButtonState.Pressed)
                {
                    this.DragMove();
                }
            };
        }

        public void SetCustomerDTO(CustomerDTO customerDto)
        {
            _currentCustomer = customerDto;
            DisplayCustomerInfo();
        }

        private void DisplayCustomerInfo()
        {
            if (_currentCustomer != null)
            {
                txtCustomerId.Text = _currentCustomer.CustomerId.ToString();
                txtName.Text = _currentCustomer.Name;
                txtEmail.Text = _currentCustomer.Email;
            }
        }

        private async void UpdateProfile_Click(object sender, RoutedEventArgs e)
        {
            if (_currentCustomer == null)
            {
                MessageBox.Show("Không có thông tin khách hàng để cập nhật.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // --- Bắt đầu Validation ---
            string newName = txtName.Text.Trim();
            string newEmail = txtEmail.Text.Trim();

            if (string.IsNullOrWhiteSpace(newName))
            {
                MessageBox.Show("Tên không được để trống.", "Lỗi Cập Nhật", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(newEmail))
            {
                MessageBox.Show("Email không được để trống.", "Lỗi Cập Nhật", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtEmail.Focus();
                return;
            }

            if (!IsValidEmail(newEmail))
            {
                MessageBox.Show("Định dạng Email không hợp lệ.", "Lỗi Cập Nhật", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtEmail.Focus();
                return;
            }
            // --- Kết thúc Validation ---

            // Cập nhật thông tin trong DTO
            _currentCustomer.Name = newName;
            _currentCustomer.Email = newEmail;

            try
            {
                // Gọi service để cập nhật thông tin khách hàng
                // Phương thức UpdateCustomerProfileAsync trả về bool
                bool success = await _customerService.UpdateCustomerProfileAsync(_currentCustomer);

                if (success)
                {
                    // Lấy lại thông tin khách hàng sau khi cập nhật để đảm bảo dữ liệu mới nhất
                    // (Tùy chọn, nếu service không trả về DTO đã cập nhật ngay lập tức)
                    CustomerDTO? refreshedCustomer = await _customerService.GetCustomerByIdAsync(_currentCustomer.CustomerId);
                    if (refreshedCustomer != null)
                    {
                        _currentCustomer = refreshedCustomer;
                    }
                    DisplayCustomerInfo(); // Hiển thị thông tin đã cập nhật lên UI

                    MessageBox.Show("Cập nhật thông tin thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true; // Báo hiệu đã có thay đổi nếu cửa sổ được mở bằng ShowDialog
                }
                else
                {
                    MessageBox.Show("Cập nhật thông tin thất bại. Vui lòng thử lại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (ArgumentException ex) // Bắt các lỗi validation từ Business Logic Layer
            {
                MessageBox.Show(ex.Message, "Lỗi Cập Nhật", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi cập nhật thông tin: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}