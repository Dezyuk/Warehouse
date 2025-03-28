using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse.Models
{
    public class WarehouseZone
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<Cell> Cells { get; set; } = new List<Cell>();
    }
}
