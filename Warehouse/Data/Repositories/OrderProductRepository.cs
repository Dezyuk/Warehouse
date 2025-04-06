using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Warehouse.Data;
using Warehouse.Models;

namespace Warehouse.Data.Repositories
{
    
    public class OrderProductRepository : IOrderProductRepository
    {
        private readonly WarehouseContext _context;

        public OrderProductRepository(WarehouseContext context)
        {
            _context = context;
        }
        public IEnumerable<OrderProduct> GetByOrderId(int orderId)
        {
            return _context.OrderProducts
                           .Include(op => op.Product)
                           .Where(op => op.OrderId == orderId)
                           .ToList();
        }
        public void AddOrderProduct(OrderProduct orderProduct)
        {
            _context.OrderProducts.Add(orderProduct);
            _context.SaveChanges();
        }

        public void UpdateOrderProduct(OrderProduct orderProduct)
        {
            _context.OrderProducts.Update(orderProduct);
            _context.SaveChanges();
        }

        public void DeleteOrderProduct(int orderId, int productId)
        {
            var op = _context.OrderProducts
                             .FirstOrDefault(op => op.OrderId == orderId && op.ProductId == productId);
            if (op != null)
            {
                _context.OrderProducts.Remove(op);
                _context.SaveChanges();
            }
        }
    }
}
