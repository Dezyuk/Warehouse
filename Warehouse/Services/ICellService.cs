using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Models;

namespace Warehouse.Services
{
    interface ICellService
    {
        IEnumerable<Cell> GetAllCells();
        Cell GetCellById(int id);
        void AddCell(Cell cell);
        void UpdateCell(Cell cell);
        void DeleteCell(int id);
    }
}
