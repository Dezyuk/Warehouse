using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Data.Repositories;
using Warehouse.Models;

namespace Warehouse.Services
{
    public class OrderProductService : IOrderProductService
    {
        private readonly IOrderProductRepository _orderProductRepository;

        public OrderProductService(IOrderProductRepository orderProductRepository)
        {
            _orderProductRepository = orderProductRepository;
        }

        public IEnumerable<OrderProduct> GetOrderProductsByOrderId(int orderId)
        {
            return _orderProductRepository.GetByOrderId(orderId);
        }

        public void AddOrderProduct(OrderProduct orderProduct)
        {
            _orderProductRepository.AddOrderProduct(orderProduct);
        }

        public void UpdateOrderProduct(OrderProduct orderProduct)
        {
            _orderProductRepository.UpdateOrderProduct(orderProduct);
        }

        public void DeleteOrderProduct(int orderId, int productId)
        {
            _orderProductRepository.DeleteOrderProduct(orderId, productId);
        }
    }
}
