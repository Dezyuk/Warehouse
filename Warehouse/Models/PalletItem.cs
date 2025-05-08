using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse.Models
{
    public class PalletItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public int Quantity { get; set; }
        public Cell FromCell { get; set; }
        public List<(int X, int Y)>? Path { get; set; }

        public PalletItem(int productId, string productName, int quantity, Cell fromCellId)
        {
            ProductId = productId;
            ProductName = productName;
            Quantity = quantity;
            FromCell = fromCellId;
        }
    }
}
