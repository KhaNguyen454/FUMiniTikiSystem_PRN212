using DataAccessLayer.Entities; // Sử dụng Entities từ DataAccessLayer
using DataAccessLayer; // Cần để sử dụng GenericRepository
using DataAccessLayer.Repositories; // Sử dụng Repositories từ DataAccessLayer
using Microsoft.EntityFrameworkCore; // Cần cho DbContextOptionsBuilder
using Microsoft.Extensions.Configuration; // Cần để đọc appsettings.json
using System;
using System.IO; // Cần cho AppDomain.CurrentDomain.BaseDirectory
using System.Threading.Tasks;
using System.Linq; // Cần cho FirstOrDefaultAsync

namespace BusinessLogicLayer.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository; // Khai báo Repository

        public CustomerService()
        {
            // Khởi tạo Repository bằng cách truyền DbContext đã được cấu hình
            _customerRepository = new CustomerRepository(CreateDbContext());
        }

        // Phương thức helper để tạo DbContextOptions từ Connection String
        private FUMiniTikiSystemDBContext CreateDbContext()
        {
            // 1. Đọc Connection String từ appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory) // Đảm bảo đọc từ thư mục chạy ứng dụng WPF
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
            string connectionString = configuration.GetConnectionString("FUMiniTikiDB"); // Tên ConnectionString của bạn

            if (string.IsNullOrEmpty(connectionString))
            {
                // Fallback nếu không tìm thấy (chỉ cho dev/test)
                connectionString = "Server=(local);Database=FUMiniTikiSystem;Trusted_Connection=True;TrustServerCertificate=True;";
                Console.WriteLine("Cảnh báo: Không tìm thấy ConnectionString 'FUMiniTikiDB' trong appsettings.json. Đang sử dụng chuỗi mặc định.");
            }

            // 2. Tạo DbContextOptions
            var optionsBuilder = new DbContextOptionsBuilder<FUMiniTikiSystemDBContext>();
            optionsBuilder.UseSqlServer(connectionString);

            // 3. Trả về DbContext đã cấu hình
            return new FUMiniTikiSystemDBContext(optionsBuilder.Options);
        }

        public async Task<Customer?> LoginAsync(string email, string password)
        {
            // Logic kiểm tra tài khoản admin cứng (từ appsettings.json)
            if (email == "admin@FUMiniTikiSystem.com" && password == "@@abc123@@")
            {
                return new Customer { CustomerId = -1, Name = "Admin", Email = email, Password = password };
            }

            // Gọi Repository để xác thực từ database
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
            // Validation cơ bản
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
                // Kiểm tra email đã tồn tại
                if (await _customerRepository.IsEmailExistsAsync(customer.Email))
                {
                    throw new ArgumentException("Email này đã được đăng ký. Vui lòng sử dụng Email khác.");
                }

                // Trong thực tế, bạn sẽ hash mật khẩu ở đây trước khi gửi đến Repository
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
                // Lấy khách hàng từ DB
                var customer = await _customerRepository.GetCustomerByEmailAndPasswordAsync(email, oldPassword);
                if (customer == null)
                {
                    throw new ArgumentException("Email hoặc mật khẩu cũ không đúng.");
                }
                if (newPassword.Length < 6)
                {
                    throw new ArgumentException("Mật khẩu mới phải có ít nhất 6 ký tự.");
                }

                // Cập nhật mật khẩu
                customer.Password = newPassword; // Trong thực tế, hash mật khẩu mới
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
                // Sử dụng FindByConditionAsync để lấy khách hàng theo email
                return await _customerRepository.GetAllCustomersAsync().Result.FirstOrDefaultAsync(c => c.Email == email);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy thông tin khách hàng: {ex.Message}");
                throw new Exception("Đã xảy ra lỗi khi lấy thông tin khách hàng. Vui lòng thử lại sau.", ex);
            }
        }

        public async Task<bool> UpdateCustomerProfileAsync(Customer customer)
        {
            // Validation cơ bản cho cập nhật
            if (string.IsNullOrWhiteSpace(customer.Name) || string.IsNullOrWhiteSpace(customer.Email))
            {
                throw new ArgumentException("Tên và Email không được để trống.");
            }

            try
            {
                // Kiểm tra nếu email thay đổi và đã tồn tại bởi người khác
                var existingCustomer = await _customerRepository.GetAllCustomersAsync().Result.FirstOrDefaultAsync(c => c.Email == customer.Email);
                if (existingCustomer != null && existingCustomer.CustomerId != customer.CustomerId)
                {
                    throw new ArgumentException("Email này đã được sử dụng bởi tài khoản khác.");
                }

                await _customerRepository.UpdateCustomerAsync(customer);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi cập nhật hồ sơ khách hàng: {ex.Message}");
                throw new Exception("Đã xảy ra lỗi khi cập nhật hồ sơ khách hàng. Vui lòng thử lại sau.", ex);
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
    }
}
