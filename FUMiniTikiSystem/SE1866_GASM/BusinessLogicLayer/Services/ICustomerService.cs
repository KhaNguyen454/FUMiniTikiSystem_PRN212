using DataAccessLayer.Entities; // Use Entities from DataAccessLayer
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public interface ICustomerService
    {
        
        Task<Customer?> LoginAsync(string email, string password);
        Task<bool> RegisterAsync(Customer customer);
        Task<bool> ChangePasswordAsync(string email, string oldPassword, string newPassword);
        Task<Customer?> GetCustomerByEmailAsync(string email);
        Task<bool> UpdateCustomerProfileAsync(Customer customer);
        Task<bool> IsEmailExistsInDbAsync(string email);

        Task<IEnumerable<Customer>> GetAllCustomersAsync(); // Lấy tất cả khách hàng (Async)
        Task<Customer?> GetCustomerByIdAsync(int id); // Lấy khách hàng theo ID (Async)
        Task<bool> DeleteCustomerByIdAsync(int customerId); // Xóa khách hàng theo ID (Async)
    }
}
