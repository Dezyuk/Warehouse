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
        public bool IsOccupied { get; set; }
        public int WarehouseZoneId { get; set; }
        public int? ProductId { get; set; }
        public WarehouseZone? WarehouseZone { get; set; } 
        public Product? Product { get; set; } 
    }
}
