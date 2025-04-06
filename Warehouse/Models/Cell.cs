using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse.Models
{
    public class Cell
    {
        public int Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public ZoneType WarehouseZoneType { get; set; }
        public Product? Product { get; set; }
        public int? ProductId { get; set; }
    }
}