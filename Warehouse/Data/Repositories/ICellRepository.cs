using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Models;

namespace Warehouse.Data.Repositories
{
    public interface ICellRepository
    {
        IEnumerable<Cell> GetAllCells();
        Cell GetCellById(int id);
        void AddCell(Cell cell);
        void UpdateCell(Cell cell);
        void DeleteCell(int id);
    }
}
