using DataAccessLayer.Entities;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IOrderRepository
    {
        IQueryable<Order> GetAll();
        Task<Order?> GetByIdAsync(int id);
        Task AddAsync(Order entity);
        Task UpdateAsync(Order entity);
        Task DeleteAsync(int id);
        Task<IQueryable<Order>> FindByConditionAsync(Expression<Func<Order, bool>> expression);

        // Các phương thức cụ thể cho Order
        Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(int customerId);
        Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus);
    }
}