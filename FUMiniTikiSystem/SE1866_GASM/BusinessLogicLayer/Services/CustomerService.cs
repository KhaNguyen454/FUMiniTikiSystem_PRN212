using DataAccessLayer.Entities;
using DataAccessLayer;
using DataAccessLayer.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace BusinessLogicLayer.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService()
        {
            _customerRepository = new CustomerRepository(CreateDbContext());
        }

        private FUMiniTikiSystemDBContext CreateDbContext()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
            string connectionString = configuration.GetConnectionString("FUMiniTikiDB");

            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = "Server=(local);Database=FUMiniTikiSystem;Trusted_Connection=True;TrustServerCertificate=True;";
                Console.WriteLine("Cảnh báo: Không tìm thấy ConnectionString 'FUMiniTikiDB' trong appsettings.json. Đang sử dụng chuỗi mặc định.");
            }

            var optionsBuilder = new DbContextOptionsBuilder<FUMiniTikiSystemDBContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new FUMiniTikiSystemDBContext(optionsBuilder.Options);
        }

        public async Task<Customer?> LoginAsync(string email, string password)
        {
            if (email == "admin@FUMiniTikiSystem.com" && password == "@@abc123@@")
            {
                return new Customer { CustomerId = -1, Name = "Admin", Email = email, Password = password };
            }

            try
            {
                return await _customerRepository.GetCustomerByEmailAndPasswordAsync(email, password);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi đăng nhập: {ex.Message}");
                throw new Exception("Đã xảy ra lỗi trong quá trình đăng nhập. Vui lòng thử lại sau.", ex);
            }
        }

        public async Task<bool> RegisterAsync(Customer customer)
        {
            if (string.IsNullOrWhiteSpace(customer.Name) || string.IsNullOrWhiteSpace(customer.Email) || string.IsNullOrWhiteSpace(customer.Password))
            {
                throw new ArgumentException("Tên, Email và Mật khẩu không được để trống.");
            }
            if (customer.Password.Length < 6)
            {
                throw new ArgumentException("Mật khẩu phải có ít nhất 6 ký tự.");
            }

            try
            {
                if (await _customerRepository.IsEmailExistsAsync(customer.Email))
                {
                    throw new ArgumentException("Email này đã được đăng ký. Vui lòng sử dụng Email khác.");
                }

                await _customerRepository.AddCustomerAsync(customer);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi đăng ký: {ex.Message}");
                throw new Exception("Đã xảy ra lỗi trong quá trình đăng ký. Vui lòng thử lại sau.", ex);
            }
        }

        public async Task<bool> ChangePasswordAsync(string email, string oldPassword, string newPassword)
        {
            try
            {
                var customer = await _customerRepository.GetCustomerByEmailAndPasswordAsync(email, oldPassword);
                if (customer == null)
                {
                    throw new ArgumentException("Email hoặc mật khẩu cũ không đúng.");
                }
                if (newPassword.Length < 6)
                {
                    throw new ArgumentException("Mật khẩu mới phải có ít nhất 6 ký tự.");
                }

                customer.Password = newPassword;
                await _customerRepository.UpdateCustomerAsync(customer);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi đổi mật khẩu: {ex.Message}");
                throw new Exception("Đã xảy ra lỗi khi đổi mật khẩu. Vui lòng thử lại sau.", ex);
            }
        }

        public async Task<Customer?> GetCustomerByEmailAsync(string email)
        {
            try
            {
                return await _customerRepository.GetAllCustomers().FirstOrDefaultAsync(c => c.Email == email);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy thông tin khách hàng: {ex.Message}");
                throw new Exception("Đã xảy ra lỗi khi lấy thông tin khách hàng. Vui lòng thử lại sau.", ex);
            }
        }

        public async Task<bool> UpdateCustomerProfileAsync(Customer customer)
        {
            if (string.IsNullOrWhiteSpace(customer.Name) || string.IsNullOrWhiteSpace(customer.Email))
            {
                throw new ArgumentException("Tên và Email không được để trống.");
            }

            try
            {
                // 1. Tải khách hàng hiện có từ database để nó được theo dõi bởi DbContext
                var existingCustomer = await _customerRepository.GetCustomerByIdAsync(customer.CustomerId);

                if (existingCustomer == null)
                {
                    throw new ArgumentException("Không tìm thấy khách hàng để cập nhật.");
                }

                // 2. Kiểm tra xem email mới có bị trùng với email của khách hàng khác không
                var customerWithSameEmail = await _customerRepository.GetAllCustomers()
                                                .FirstOrDefaultAsync(c => c.Email == customer.Email && c.CustomerId != customer.CustomerId);

                if (customerWithSameEmail != null)
                {
                    throw new ArgumentException("Email này đã được sử dụng bởi tài khoản khác.");
                }

                // 3. Cập nhật các thuộc tính của entity đã được theo dõi
                existingCustomer.Name = customer.Name;
                existingCustomer.Email = customer.Email;
                // ĐÃ SỬA: KHÔNG CẬP NHẬT MẬT KHẨU TẠI ĐÂY. Mật khẩu được xử lý bằng ChangePasswordAsync.
                // existingCustomer.Password = customer.Password; // Dòng này đã được loại bỏ

                // 4. Lưu thay đổi
                await _customerRepository.UpdateCustomerAsync(existingCustomer); // Truyền entity đã được theo dõi

                return true;
            }
            catch (ArgumentException argEx)
            {
                Console.WriteLine($"Lỗi Argument khi cập nhật hồ sơ: {argEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                string errorMessage = $"Đã xảy ra lỗi khi cập nhật hồ sơ khách hàng: {ex.Message}";
                if (ex.InnerException != null)
                {
                    errorMessage += $" Chi tiết: {ex.InnerException.Message}";
                }
                Console.WriteLine($"Lỗi chi tiết khi cập nhật hồ sơ khách hàng: {errorMessage}");
                throw new Exception(errorMessage, ex);
            }
        }

        public async Task<bool> IsEmailExistsInDbAsync(string email)
        {
            try
            {
                return await _customerRepository.IsEmailExistsAsync(email);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi kiểm tra email tồn tại trong DB: {ex.Message}");
                throw new Exception("Đã xảy ra lỗi khi kiểm tra email. Vui lòng thử lại sau.", ex);
            }
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            try
            {
                return await _customerRepository.GetAllCustomers().ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy tất cả khách hàng: {ex.Message}");
                throw new Exception("Đã xảy ra lỗi khi tải danh sách khách hàng. Vui lòng thử lại sau.", ex);
            }
        }

        public async Task<Customer?> GetCustomerByIdAsync(int id)
        {
            try
            {
                return await _customerRepository.GetCustomerByIdAsync(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy khách hàng theo ID: {ex.Message}");
                throw new Exception($"Đã xảy ra lỗi khi lấy thông tin khách hàng với ID {id}. Vui lòng thử lại sau.", ex);
            }
        }

        public async Task<bool> DeleteCustomerByIdAsync(int customerId)
        {
            try
            {
                await _customerRepository.DeleteCustomerAsync(customerId);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi xóa khách hàng: {ex.Message}");
                throw new Exception($"Đã xảy ra lỗi khi xóa khách hàng với ID {customerId}. Vui lòng thử lại sau.", ex);
            }
        }
    }
}
