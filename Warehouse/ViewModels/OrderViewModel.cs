using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Models;
using Warehouse.Services;

namespace Warehouse.ViewModels
{
    public class OrderViewModel 
    {
        private readonly IOrderService _orderService;

        public OrderViewModel(IOrderService orderService)
        {
            _orderService = orderService;
            Orders = _orderService.GetAllOrders();
        }

        public IEnumerable<Order> Orders { get; set; }
    }
}
