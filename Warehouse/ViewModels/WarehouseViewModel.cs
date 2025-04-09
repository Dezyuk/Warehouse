//using System.Collections.ObjectModel;
//using System.Linq;
//using Warehouse.Models;

//namespace Warehouse.ViewModels
//{
//    public class WarehouseViewModel
//    {
//        public ObservableCollection<Cell> Cells { get; } = new ObservableCollection<Cell>();

//        public void AddCell(int x, int y)
//        {
//            if (!Cells.Any(c => c.X == x && c.Y == y))
//            {
//                Cells.Add(new Cell { X = x, Y = y, IsOccupied = false });
//            }
//        }

//        public void RemoveCell(int x, int y)
//        {
//            var cell = Cells.FirstOrDefault(c => c.X == x && c.Y == y);
//            if (cell != null)
//            {
//                Cells.Remove(cell);
//            }
//        }

//        public bool HasNeighbor(int x, int y)
//        {
//            return Cells.Any(c =>
//                (c.X == x - 1 && c.Y == y) ||
//                (c.X == x + 1 && c.Y == y) ||
//                (c.X == x && c.Y == y - 1) ||
//                (c.X == x && c.Y == y + 1));
//        }
//    }
//}
