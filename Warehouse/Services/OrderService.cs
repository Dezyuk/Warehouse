using System.Collections.ObjectModel;
using System.Linq;
using Warehouse.Data.Repositories;
using Warehouse.Models;

namespace Warehouse.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public ObservableCollection<Order> Orders { get; }

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
            Orders = new ObservableCollection<Order>(
                _orderRepository.GetAllOrders());
        }

        public IEnumerable<Order> GetAllOrders() => Orders;

        public Order? GetOrderById(int id)
        {
            return Orders.FirstOrDefault(o => o.Id == id)
                   ?? _orderRepository.GetOrderById(id);
        }

        public void AddOrder(Order order)
        {
            _orderRepository.AddOrder(order);
            Orders.Add(order);
        }

        public void UpdateOrder(Order order)
        {
            _orderRepository.UpdateOrder(order);

            var existing = Orders.FirstOrDefault(o => o.Id == order.Id);
            if (existing == null) return;

            existing.CustomerName = order.CustomerName;
            existing.OrderDate = order.OrderDate;
            existing.OrderProducts.Clear();
            foreach (var op in order.OrderProducts)
                existing.OrderProducts.Add(op);
        }

        public void DeleteOrder(int id)
        {
            _orderRepository.DeleteOrder(id);

            var toRemove = Orders.FirstOrDefault(o => o.Id == id);
            if (toRemove != null)
                Orders.Remove(toRemove);
        }

    }
}
