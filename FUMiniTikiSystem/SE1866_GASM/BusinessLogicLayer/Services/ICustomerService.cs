using DataAccessLayer.Entities; // Use Entities from DataAccessLayer
using BusinessLogicLayer.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public interface ICustomerService
    {
        Task<CustomerDTO?> LoginAsync(string email, string password);
        Task<bool> RegisterAsync(CustomerDTO customer);
        Task<bool> ChangePasswordAsync(string email, string oldPassword, string newPassword);
        Task<CustomerDTO?> GetCustomerByEmailAsync(string email);
        Task<bool> UpdateCustomerProfileAsync(CustomerDTO customer);
        Task<bool> IsEmailExistsInDbAsync(string email);

        Task<IEnumerable<CustomerDTO>> GetAllCustomersAsync(); // Lấy tất cả khách hàng (Async)
        Task<CustomerDTO?> GetCustomerByIdAsync(int id); // Lấy khách hàng theo ID (Async)
        Task<bool> DeleteCustomerByIdAsync(int customerId); // Xóa khách hàng theo ID (Async)
    }
}
