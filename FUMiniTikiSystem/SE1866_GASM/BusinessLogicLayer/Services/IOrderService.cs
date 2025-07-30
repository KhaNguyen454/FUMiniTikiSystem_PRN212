// BusinessLogicLayer/Interfaces/IOrderService.cs (hoặc BusinessLogicLayer/Services/IOrderService.cs)
using DataAccessLayer.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services 

{
    public interface IOrderService
    {
        Task<bool> PlaceOrderAsync(Order newOrder);
        Task<IEnumerable<Order>> GetCustomerOrdersAsync(int customerId);
        Task<Order> GetOrderDetailsAsync(int orderId);
        Task<bool> ChangeOrderStatusAsync(int orderId, string newStatus);
    }
}