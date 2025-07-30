using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BusinessLogicLayer.Services;
using BusinessLogicLayer.DTOs; // Thêm để sử dụng CustomerDTO

namespace GASMWPF.Admin
{
    public partial class CustomerManagementWindow : Window
    {
        private readonly ICustomerService _customerService;

        // Constructor nhận ICustomerService thông qua Dependency Injection
        public CustomerManagementWindow(ICustomerService customerService)
        {
            InitializeComponent();
            _customerService = customerService; // Gán service được inject

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
                // Gọi Service Layer để lấy CustomerDTOs
                IEnumerable<CustomerDTO> customers = await _customerService.GetAllCustomersAsync();
                dgCustomers.ItemsSource = customers.ToList(); // DataGrid bind với CustomerDTOs
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
            txtPassword.Password = string.Empty;
            txtSearchEmail.Text = string.Empty;
            dgCustomers.SelectedItem = null;
        }

        private void DgCustomers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Kiểm tra kiểu là CustomerDTO
            if (dgCustomers.SelectedItem is CustomerDTO selectedCustomer)
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

            // Tạo CustomerDTO từ input UI
            CustomerDTO newCustomerDto = new CustomerDTO
            {
                // CustomerId sẽ được tạo bởi DB, không cần gán ở đây khi thêm mới
                Name = txtName.Text,
                Email = txtEmail.Text,
                Password = txtPassword.Password,
                IsAdmin = false // Mặc định khi thêm mới là người dùng thường
            };

            try
            {
                // Gọi Service Layer với CustomerDTO, phương thức RegisterAsync trả về bool
                bool success = await _customerService.RegisterAsync(newCustomerDto);

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

            // Không cho phép thay đổi mật khẩu từ đây (như logic hiện có)
            if (!string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                MessageBox.Show("Không thể thay đổi mật khẩu từ màn hình quản lý. Vui lòng sử dụng chức năng đổi mật khẩu riêng.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                txtPassword.Password = string.Empty; // Xóa mật khẩu đã nhập
                return;
            }

            try
            {
                // Lấy CustomerDTO hiện có từ Service để đảm bảo cập nhật đúng
                CustomerDTO? existingCustomerDto = await _customerService.GetCustomerByIdAsync(customerId);

                if (existingCustomerDto == null)
                {
                    MessageBox.Show("Không tìm thấy khách hàng để cập nhật.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    await LoadCustomers(); // Tải lại danh sách để đồng bộ
                    return;
                }

                // Cập nhật các thuộc tính của DTO từ UI
                existingCustomerDto.Name = txtName.Text;
                existingCustomerDto.Email = txtEmail.Text;
                // Mật khẩu và IsAdmin không được sửa đổi từ UI này, giữ nguyên giá trị đã tải

                // Gọi Service Layer với CustomerDTO đã cập nhật, sử dụng UpdateCustomerProfileAsync
                bool success = await _customerService.UpdateCustomerProfileAsync(existingCustomerDto);

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
                    // Gọi Service Layer để xóa theo ID, sử dụng DeleteCustomerByIdAsync
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

        private async void SearchByEmail_Click(object sender, RoutedEventArgs e)
        {
            string searchTerm = txtSearchEmail.Text.Trim();

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                MessageBox.Show("Vui lòng nhập Email hoặc một phần Email để tìm kiếm.", "Tìm kiếm khách hàng", MessageBoxButton.OK, MessageBoxImage.Information);
                await LoadCustomers(); // Tải lại tất cả nếu trường tìm kiếm trống
                return;
            }

            try
            {
                // Gọi Service Layer để lấy tất cả CustomerDTOs và lọc ở đây
                IEnumerable<CustomerDTO> allCustomers = await _customerService.GetAllCustomersAsync();
                List<CustomerDTO> filteredCustomers = allCustomers.Where(c => c.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();

                if (filteredCustomers.Any())
                {
                    dgCustomers.ItemsSource = filteredCustomers;
                    ClearInputFieldsExceptSearch();
                    if (filteredCustomers.Count == 1)
                    {
                        txtCustomerId.Text = filteredCustomers[0].CustomerId.ToString();
                        txtName.Text = filteredCustomers[0].Name;
                        txtEmail.Text = filteredCustomers[0].Email;
                        txtPassword.Password = string.Empty;
                    }
                }
                else
                {
                    dgCustomers.ItemsSource = null;
                    MessageBox.Show("Không tìm thấy khách hàng nào khớp với Email này.", "Tìm kiếm khách hàng", MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearInputFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi tìm kiếm khách hàng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

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