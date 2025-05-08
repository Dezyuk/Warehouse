using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse.Models
{
    public class Pallet
    {
        public int Id { get; set; }
        public List<PalletItem> Items { get; set; } = new();
        public int TotalQuantity => Items.Sum(x => x.Quantity);
        public bool IsFull => TotalQuantity == 1000;
        public Pallet(int id)
        {
            Id = id;
        }
    }
}
