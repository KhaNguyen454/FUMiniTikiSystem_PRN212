using DataAccessLayer.Entities; // Use Entities from DataAccessLayer
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public interface ICustomerService
    {
        // Login logic
        Task<Customer?> LoginAsync(string email, string password);
        // Register new account logic
        Task<bool> RegisterAsync(Customer customer);
        // Change password logic
        Task<bool> ChangePasswordAsync(string email, string oldPassword, string newPassword);
        // Get customer information logic (e.g., for profile)
        Task<Customer?> GetCustomerByEmailAsync(string email);
        // Update customer profile logic
        Task<bool> UpdateCustomerProfileAsync(Customer customer);
        // Check if email exists
        Task<bool> IsEmailExistsInDbAsync(string email);
    }
}
