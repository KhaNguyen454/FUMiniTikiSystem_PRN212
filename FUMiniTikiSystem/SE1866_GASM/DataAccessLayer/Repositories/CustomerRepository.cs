using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq; // Quan trọng: Đảm bảo có using System.Linq
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly FUMiniTikiSystemDBContext _context;
        private readonly GenericRepository<Customer> _customerGenericRepository;

        public CustomerRepository(FUMiniTikiSystemDBContext context)
        {
            _context = context;
            _customerGenericRepository = new GenericRepository<Customer>(_context);
        }

        // ĐẢM BẢO TRIỂN KHAI NÀY: Gọi GetAll() không có Async từ GenericRepository
        public IQueryable<Customer> GetAllCustomers()
        {
            return _customerGenericRepository.GetAll();
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
            // Các phương thức bất đồng bộ (AnyAsync, FirstOrDefaultAsync) được gọi trên IQueryable
            return await GetAllCustomers().AnyAsync(c => c.Email == email);
        }

        public async Task<Customer?> GetCustomerByEmailAndPasswordAsync(string email, string password)
        {
            // Các phương thức bất đồng bộ (AnyAsync, FirstOrDefaultAsync) được gọi trên IQueryable
            return await GetAllCustomers().FirstOrDefaultAsync(c => c.Email == email && c.Password == password);
        }
    }
}
