using DataAccessLayer.Entities; // Sử dụng Entities từ DataAccessLayer
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories // Đã đổi namespace
{
    public class CustomerRepository : ICustomerRepository // Đã đổi tên lớp và interface triển khai
    {
        private readonly FUMiniTikiSystemDBContext _context;
        private readonly GenericRepository<Customer> _customerGenericRepository; // Đổi tên biến để tránh nhầm lẫn

        public CustomerRepository(FUMiniTikiSystemDBContext context)
        {
            _context = context;
            _customerGenericRepository = new GenericRepository<Customer>(_context);
        }

        public async Task<IQueryable<Customer>> GetAllCustomersAsync()
        {
            return (await _customerGenericRepository.GetAllAsync()).AsQueryable();
        }

        public async Task<Customer?> GetCustomerByIdAsync(int id)
        {
            return await _customerGenericRepository.GetByIdAsync(id);
        }

        public async Task AddCustomerAsync(Customer customer)
        {
            await _customerGenericRepository.AddAsync(customer);
        }

        public async Task UpdateCustomerAsync(Customer customer)
        {
            await _customerGenericRepository.UpdateAsync(customer);
        }

        public async Task DeleteCustomerAsync(int id)
        {
            await _customerGenericRepository.DeleteAsync(id);
        }

        public async Task<bool> IsEmailExistsAsync(string email)
        {
            return await _customerGenericRepository.FindByConditionAsync(c => c.Email == email).Result.AnyAsync();
        }

        public async Task<Customer?> GetCustomerByEmailAndPasswordAsync(string email, string password)
        {
            return await _customerGenericRepository.FindByConditionAsync(c => c.Email == email && c.Password == password)
                                       .Result.FirstOrDefaultAsync();
        }
    }
}
