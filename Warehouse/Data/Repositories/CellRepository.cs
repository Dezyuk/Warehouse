using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Warehouse.Data;
using Warehouse.Models;

namespace Warehouse.Data.Repositories
{
    public class CellRepository : ICellRepository
    {
        private readonly WarehouseContext _context;

        public CellRepository(WarehouseContext context)
        {
            _context = context;
        }

        public IEnumerable<Cell> GetAllCells()
        {
            return _context.Cells.ToList();
        }

       public Cell? GetCellById(int id)
        {
            return _context.Cells.Find(id);
        }

        public Cell? GetCellByProduct(int productId)
        {
            return _context.Cells.FirstOrDefault(c => c.ProductId == productId);
        }

        public void AddCell(Cell cell)
        {
            _context.Cells.Add(cell);
            _context.SaveChanges();
        }

        public void UpdateCell(Cell cell)
        {
            // ищем в контексте ту же самую сущность
            var existing = _context.Cells.FirstOrDefault(c => c.Id == cell.Id);
            if (existing == null)
                throw new InvalidOperationException($"Cell {cell.Id} not found");

            // переносим все изменившиеся поля
            existing.X = cell.X;
            existing.Y = cell.Y;
            existing.ZoneType = cell.ZoneType;
            existing.ProductId = cell.ProductId;
            existing.Quantity = cell.Quantity;

            // сохраняем изменения
            _context.SaveChanges();
        }

        public void DeleteCell(int id)
        {
            var cell = _context.Cells.Find(id);
            if (cell != null)
            {
                _context.Cells.Remove(cell);
                _context.SaveChanges();
            }
        }
    }
}
