using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BusinessLogicLayer.Services;
using DataAccessLayer.Entities;

namespace GASMWPF.Admin
{
    public partial class CustomerManagementWindow : Window
    {
        private readonly ICustomerService _customerService;

        public CustomerManagementWindow()
        {
            InitializeComponent();
            _customerService = new CustomerService();
            this.MouseLeftButtonDown += (sender, e) => {
                if (e.ButtonState == MouseButtonState.Pressed)
                {
                    this.DragMove();
                }
            };
            LoadCustomers(); // Tải dữ liệu khi cửa sổ được khởi tạo
        }

        private async void LoadCustomers_Click(object sender, RoutedEventArgs e)
        {
            await LoadCustomers();
        }

        private async Task LoadCustomers()
        {
            try
            {
                IEnumerable<Customer> customers = await _customerService.GetAllCustomersAsync();
                dgCustomers.ItemsSource = customers.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi tải danh sách khách hàng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearFields_Click(object sender, RoutedEventArgs e)
        {
            ClearInputFields();
        }

        private void ClearInputFields()
        {
            txtCustomerId.Text = string.Empty;
            txtName.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtPassword.Password = string.Empty; // Vẫn xóa nội dung hiển thị
            txtSearchEmail.Text = string.Empty; // Xóa trường tìm kiếm
            dgCustomers.SelectedItem = null;
        }

        private void DgCustomers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgCustomers.SelectedItem is Customer selectedCustomer)
            {
                txtCustomerId.Text = selectedCustomer.CustomerId.ToString();
                txtName.Text = selectedCustomer.Name;
                txtEmail.Text = selectedCustomer.Email;
                txtPassword.Password = string.Empty; // Không hiển thị mật khẩu vì lý do bảo mật
            }
            else
            {
                ClearInputFields();
            }
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Logic khi double click vào một hàng (nếu cần, ví dụ mở cửa sổ chi tiết)
            // Hiện tại, SelectionChanged đã xử lý việc điền dữ liệu vào các trường
        }

        private async void AddCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput(isNew: true)) return;

            Customer newCustomer = new Customer
            {
                Name = txtName.Text,
                Email = txtEmail.Text,
                Password = txtPassword.Password // Mật khẩu chỉ được thiết lập khi thêm mới
            };

            try
            {
                bool success = await _customerService.RegisterAsync(newCustomer);

                if (success)
                {
                    MessageBox.Show("Thêm khách hàng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearInputFields();
                    await LoadCustomers();
                }
                else
                {
                    MessageBox.Show("Thêm khách hàng thất bại. Vui lòng thử lại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (ArgumentException argEx)
            {
                MessageBox.Show(argEx.Message, "Lỗi Thêm Khách Hàng", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi thêm khách hàng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void UpdateCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCustomerId.Text))
            {
                MessageBox.Show("Vui lòng chọn khách hàng để cập nhật.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!ValidateInput(isNew: false)) return;

            if (!int.TryParse(txtCustomerId.Text, out int customerId))
            {
                MessageBox.Show("ID khách hàng không hợp lệ.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // Tải khách hàng hiện có từ database để Entity Framework theo dõi đúng đối tượng
                Customer? existingCustomer = await _customerService.GetCustomerByIdAsync(customerId);

                if (existingCustomer == null)
                {
                    MessageBox.Show("Không tìm thấy khách hàng để cập nhật.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    await LoadCustomers(); // Tải lại danh sách để đồng bộ
                    return;
                }

                // Cập nhật các thuộc tính của đối tượng đã được tải
                existingCustomer.Name = txtName.Text;
                existingCustomer.Email = txtEmail.Text;
                // ĐÃ SỬA: KHÔNG CẬP NHẬT MẬT KHẨU TẠI ĐÂY. Mật khẩu sẽ không bị thay đổi.
                // existingCustomer.Password = txtPassword.Password; // Dòng này đã được loại bỏ

                bool success = await _customerService.UpdateCustomerProfileAsync(existingCustomer); // Truyền đối tượng đã tải và sửa đổi

                if (success)
                {
                    MessageBox.Show("Cập nhật khách hàng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearInputFields();
                    await LoadCustomers();
                }
                else
                {
                    MessageBox.Show("Cập nhật khách hàng thất bại. Vui lòng thử lại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (ArgumentException argEx)
            {
                MessageBox.Show(argEx.Message, "Lỗi Cập Nhật Khách Hàng", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi cập nhật khách hàng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void DeleteCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCustomerId.Text))
            {
                MessageBox.Show("Vui lòng chọn khách hàng để xóa.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show("Bạn có chắc chắn muốn xóa khách hàng này?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (!int.TryParse(txtCustomerId.Text, out int customerId))
                {
                    MessageBox.Show("ID khách hàng không hợp lệ.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                try
                {
                    bool success = await _customerService.DeleteCustomerByIdAsync(customerId);

                    if (success)
                    {
                        MessageBox.Show("Xóa khách hàng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        ClearInputFields();
                        await LoadCustomers();
                    }
                    else
                    {
                        MessageBox.Show("Xóa khách hàng thất bại. Vui lòng thử lại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Đã xảy ra lỗi khi xóa khách hàng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // ĐÃ THÊM: Phương thức tìm kiếm khách hàng theo Email
        private async void SearchByEmail_Click(object sender, RoutedEventArgs e)
        {
            string email = txtSearchEmail.Text.Trim();

            if (string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Vui lòng nhập Email để tìm kiếm.", "Tìm kiếm khách hàng", MessageBoxButton.OK, MessageBoxImage.Information);
                await LoadCustomers(); // Tải lại tất cả nếu trường tìm kiếm trống
                return;
            }

            if (!IsValidEmail(email))
            {
                MessageBox.Show("Email tìm kiếm không hợp lệ.", "Tìm kiếm khách hàng", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                Customer? foundCustomer = await _customerService.GetCustomerByEmailAsync(email);

                if (foundCustomer != null)
                {
                    dgCustomers.ItemsSource = new List<Customer> { foundCustomer }; // Hiển thị chỉ khách hàng tìm thấy
                    ClearInputFieldsExceptSearch(); // Xóa các trường nhập liệu khác
                    txtCustomerId.Text = foundCustomer.CustomerId.ToString();
                    txtName.Text = foundCustomer.Name;
                    txtEmail.Text = foundCustomer.Email;
                    // Mật khẩu vẫn không hiển thị
                }
                else
                {
                    dgCustomers.ItemsSource = null; // Xóa DataGrid
                    MessageBox.Show("Không tìm thấy khách hàng với Email này.", "Tìm kiếm khách hàng", MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearInputFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi tìm kiếm khách hàng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Thêm phương thức ClearInputFieldsExceptSearch để sử dụng khi tìm kiếm
        private void ClearInputFieldsExceptSearch()
        {
            txtCustomerId.Text = string.Empty;
            txtName.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtPassword.Password = string.Empty;
            dgCustomers.SelectedItem = null;
        }


        private bool ValidateInput(bool isNew)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Tên không được để trống.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtName.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Email không được để trống.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtEmail.Focus();
                return false;
            }
            if (!IsValidEmail(txtEmail.Text))
            {
                MessageBox.Show("Email không hợp lệ.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtEmail.Focus();
                return false;
            }
            // Chỉ kiểm tra mật khẩu khi thêm mới
            if (isNew && string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                MessageBox.Show("Mật khẩu không được để trống khi thêm mới.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPassword.Focus();
                return false;
            }
            if (isNew && txtPassword.Password.Length < 6)
            {
                MessageBox.Show("Mật khẩu phải có ít nhất 6 ký tự.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPassword.Focus();
                return false;
            }
            return true;
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
