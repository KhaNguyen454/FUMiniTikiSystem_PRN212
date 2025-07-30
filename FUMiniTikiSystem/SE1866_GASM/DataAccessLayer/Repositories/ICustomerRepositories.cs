using DataAccessLayer.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public interface ICustomerRepository
    {
        IQueryable<Customer> GetAllCustomers();
        Task<Customer?> GetCustomerByIdAsync(int id);
        Task AddCustomerAsync(Customer customer);
        Task UpdateCustomerAsync(Customer customer);
        Task DeleteCustomerAsync(int id);
        Task<bool> IsEmailExistsAsync(string email);
        Task<bool> IsEmailExistsForOtherCustomerAsync(string email, int currentCustomerId);
        Task<Customer?> GetCustomerByEmailAndPasswordAsync(string email, string password);
    }
}