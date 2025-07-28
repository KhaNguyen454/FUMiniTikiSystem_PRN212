using System;
using System.Windows;
using System.Windows.Input;
using DataAccessLayer.Entities; 
using BusinessLogicLayer.Services; 
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace GASMWPF.CustomerWindow 
{
    public partial class CustomerProfileWindow : Window
    {
        private readonly ICustomerService _customerService;
        private DataAccessLayer.Entities.Customer _currentCustomer; 

        public CustomerProfileWindow(DataAccessLayer.Entities.Customer customer) 
        {
            InitializeComponent();
            this.MouseLeftButtonDown += (sender, e) => {
                if (e.ButtonState == MouseButtonState.Pressed)
                {
                    this.DragMove();
                }
            };
            _customerService = new CustomerService(); // Khởi tạo Service Layer
            _currentCustomer = customer; // Nhận thông tin khách hàng từ cửa sổ trước
            LoadCustomerData();
        }

        private void LoadCustomerData()
        {
            if (_currentCustomer != null) // Kiểm tra null an toàn
            {
                txtCustomerId.Text = _currentCustomer.CustomerId.ToString();
                txtName.Text = _currentCustomer.Name;
                txtEmail.Text = _currentCustomer.Email;
                // Không hiển thị mật khẩu ở đây vì lý do bảo mật
            }
        }

        private async void UpdateProfile_Click(object sender, RoutedEventArgs e)
        {
            if (_currentCustomer == null)
            {
                MessageBox.Show("Không có thông tin khách hàng để cập nhật.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string newName = txtName.Text;
            string newEmail = txtEmail.Text;

            // Validation
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
                MessageBox.Show("Email không hợp lệ.", "Lỗi Cập Nhật", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtEmail.Focus();
                return;
            }

            // Tạo một đối tượng Customer tạm thời để cập nhật
            DataAccessLayer.Entities.Customer updatedCustomer = new DataAccessLayer.Entities.Customer 
            {
                CustomerId = _currentCustomer.CustomerId,
                Name = newName,
                Email = newEmail,
                Password = _currentCustomer.Password // Giữ nguyên mật khẩu cũ, không cập nhật ở đây
            };

            try
            {
                bool success = await _customerService.UpdateCustomerProfileAsync(updatedCustomer);

                if (success)
                {
                    _currentCustomer.Name = newName; // Cập nhật lại đối tượng trong bộ nhớ
                    _currentCustomer.Email = newEmail;
                    MessageBox.Show("Cập nhật hồ sơ thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Cập nhật hồ sơ thất bại. Vui lòng thử lại.", "Lỗi Cập Nhật", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (ArgumentException argEx)
            {
                MessageBox.Show(argEx.Message, "Lỗi Cập Nhật", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi cập nhật hồ sơ: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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
