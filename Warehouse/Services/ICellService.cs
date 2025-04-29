using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Models;

namespace Warehouse.Services
{
    public interface ICellService
    {
        IEnumerable<Cell> GetAllCells();
        Cell? GetCellById(int id);
        Cell? GetCellByProduct(int id);
        void AddCell(Cell cell);
        void UpdateCell(Cell cell);
        void DeleteCell(int id);
        IEnumerable<Cell> GetCellsByProduct(int productId);
        void DeductFromCells(int productId, int quantity);
        void AssignColorsToProductCells(IEnumerable<Cell> cells);
    }
}
