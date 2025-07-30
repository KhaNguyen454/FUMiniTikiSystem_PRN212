using DataAccessLayer.Entities;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICustomerRepository _customerRepository; 

        public OrderService(IOrderRepository orderRepository, ICustomerRepository customerRepository)
        {
            _orderRepository = orderRepository;
            _customerRepository = customerRepository;
        }

        public async Task<bool> PlaceOrderAsync(Order newOrder)
        {
            // Validation
            if (newOrder == null)
            {
                throw new ArgumentNullException(nameof(newOrder), "Thông tin đơn hàng không được rỗng.");
            }
            if (newOrder.CustomerId <= 0)
            {
                throw new ArgumentException("ID Khách hàng không hợp lệ.", nameof(newOrder.CustomerId));
            }
            if (newOrder.OrderAmount < 0)
            {
                throw new ArgumentException("Số tiền đơn hàng không thể là số âm.", nameof(newOrder.OrderAmount));
            }
            if (newOrder.OrderDate == default(DateTime))
            {
                newOrder.OrderDate = DateTime.Now;
            }

            var customerExists = await _customerRepository.GetCustomerByIdAsync(newOrder.CustomerId);
            if (customerExists == null)
            {
                throw new ArgumentException($"Khách hàng với ID '{newOrder.CustomerId}' không tồn tại.", nameof(newOrder.CustomerId));
            }

            newOrder.Status = "Pending";

            try
            {
                await _orderRepository.AddAsync(newOrder);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi đặt hàng: {ex.Message}");
                return false;
            }
        }

        public async Task<IEnumerable<Order>> GetCustomerOrdersAsync(int customerId)
        {
            if (customerId <= 0)
            {
                throw new ArgumentException("ID Khách hàng không hợp lệ.", nameof(customerId));
            }
            var customerExists = await _customerRepository.GetCustomerByIdAsync(customerId);
            if (customerExists == null)
            {
                throw new ArgumentException($"Khách hàng với ID '{customerId}' không tồn tại.", nameof(customerId));
            }

            return await _orderRepository.GetOrdersByCustomerIdAsync(customerId);
        }

        public async Task<Order> GetOrderDetailsAsync(int orderId)
        {
            if (orderId <= 0)
            {
                throw new ArgumentException("ID Đơn hàng không hợp lệ.", nameof(orderId));
            }
            return await _orderRepository.GetByIdAsync(orderId);
        }

        public async Task<bool> ChangeOrderStatusAsync(int orderId, string newStatus)
        {
            if (orderId <= 0)
            {
                throw new ArgumentException("ID Đơn hàng không hợp lệ.", nameof(orderId));
            }
            if (string.IsNullOrWhiteSpace(newStatus))
            {
                throw new ArgumentException("Trạng thái mới không được rỗng.", nameof(newStatus));
            }

            return await _orderRepository.UpdateOrderStatusAsync(orderId, newStatus);
        }
    }
}