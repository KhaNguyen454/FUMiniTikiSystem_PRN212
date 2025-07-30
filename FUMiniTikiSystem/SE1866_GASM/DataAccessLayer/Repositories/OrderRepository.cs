using DataAccessLayer.Entities;
using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly FUMiniTikiSystemDBContext _context;
        private readonly GenericRepository<Order> _genericOrderRepository; // Sử dụng GenericRepository

        public OrderRepository(FUMiniTikiSystemDBContext context)
        {
            _context = context;
            _genericOrderRepository = new GenericRepository<Order>(context); // Khởi tạo GenericRepository cho Order
        }

        // Triển khai các phương thức CRUD bằng cách ủy quyền cho _genericOrderRepository
        public IQueryable<Order> GetAll()
        {
            return _genericOrderRepository.GetAll();
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _genericOrderRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(Order entity)
        {
            await _genericOrderRepository.AddAsync(entity);
        }

        public async Task UpdateAsync(Order entity)
        {
            await _genericOrderRepository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await _genericOrderRepository.DeleteAsync(id);
        }

        public Task<IQueryable<Order>> FindByConditionAsync(Expression<Func<Order, bool>> expression)
        {
            return _genericOrderRepository.FindByConditionAsync(expression);
        }

        // Triển khai các phương thức cụ thể cho Order
        public async Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(int customerId)
        {
            // Để đảm bảo sử dụng DbContext của Repository này, ta có thể truy vấn trực tiếp
            // hoặc dùng FindByConditionAsync nếu muốn tận dụng _genericOrderRepository
            return await _context.Orders
                                 .AsNoTracking() // Nên dùng AsNoTracking cho các truy vấn chỉ đọc
                                 .Where(o => o.CustomerId == customerId)
                                 .ToListAsync();
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return false;
            }

            order.Status = newStatus;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}