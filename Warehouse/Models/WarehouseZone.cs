//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Warehouse.Models
//{
//    public class WarehouseZone
//    {
//        public List<Cell> Cells { get; }

//        public WarehouseZone(IEnumerable<Cell> cells)
//        {
//            Cells = cells.ToList();
//        }

//        public List<Cell> GetCellsByType(TopologyMode type)
//        {
//            return Cells.Where(c => c.WarehouseZoneType == type).ToList();
//        }
//        //public List<Cell> Cells { get; set; } = new List<Cell>();
//    }
//}