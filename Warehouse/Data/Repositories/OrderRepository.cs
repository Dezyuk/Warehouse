﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Warehouse.Data;
using Warehouse.Models;

namespace Warehouse.Data.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly WarehouseContext _context;

        public OrderRepository(WarehouseContext context)
        {
            _context = context;
        }

        public IEnumerable<Order> GetAllOrders()
        {
            return _context.Orders.Include(o => o.OrderProducts)
                                  .ThenInclude(op => op.Product)
                                  .ToList();
        }

        public Order? GetOrderById(int id)
        {
            return _context.Orders.Include(o => o.OrderProducts)
                                  .ThenInclude(op => op.Product)
                                  .FirstOrDefault(o => o.Id == id);
        }

        public void AddOrder(Order order)
        {
            _context.Orders.Add(order);
            _context.SaveChanges();
        }

        public void UpdateOrder(Order order)
        {
            _context.Orders.Update(order);
            _context.SaveChanges();
        }

        public void DeleteOrder(int id)
        {
            var order = _context.Orders.Find(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                _context.SaveChanges();
            }
        }
    }
}
