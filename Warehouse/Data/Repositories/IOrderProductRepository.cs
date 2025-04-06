using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Models;

namespace Warehouse.Data.Repositories
{
    public interface IOrderProductRepository
    {
        IEnumerable<OrderProduct> GetByOrderId(int orderId); 
        void AddOrderProduct(OrderProduct orderProduct); 
        void UpdateOrderProduct(OrderProduct orderProduct); 
        void DeleteOrderProduct(int orderId, int productId);
    }
}
