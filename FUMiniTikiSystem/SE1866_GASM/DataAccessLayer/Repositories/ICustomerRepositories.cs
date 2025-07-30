using DataAccessLayer.Entities; // Sử dụng Entities từ DataAccessLayer
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories // Đã đổi namespace
{
    public interface ICustomerRepository
    {
        IQueryable<Customer> GetAllCustomers();
        Task<Customer?> GetCustomerByIdAsync(int id);
        Task AddCustomerAsync(Customer customer);
        Task UpdateCustomerAsync(Customer customer); 
        Task DeleteCustomerAsync(int id); 
        Task<bool> IsEmailExistsAsync(string email);
        Task<Customer?> GetCustomerByEmailAndPasswordAsync(string email, string password);
    }
}
