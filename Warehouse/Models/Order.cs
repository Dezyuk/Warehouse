using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public ObservableCollection<OrderProduct> OrderProducts { get; set; } = new ObservableCollection<OrderProduct>();
        

        [NotMapped]
        public decimal TotalAmount
            => OrderProducts.Sum(op => op.PriceAtOrder * op.Quantity);
    }
}