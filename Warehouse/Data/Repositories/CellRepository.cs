using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Models;
using Microsoft.EntityFrameworkCore;

namespace Warehouse.Data.Repositories
{
    public class CellRepository : ICellRepository
    {
        private readonly WarehouseContext _context;
        public CellRepository(WarehouseContext context)
        {
            _context = context;
        }
        public void AddCell(Cell cell)
        {
            _context.Cells.Add(cell);
            _context.SaveChanges();
        }

        public void DeleteCell(int id)
        {
            var cell = _context.Cells.Find(id);
            if(cell is not null)
            {
                _context.Cells.Remove(cell);
                _context.SaveChanges();
            }
        }

        public IEnumerable<Cell> GetAllCells()
        {
            return _context.Cells.Include(c => c.WarehouseZone).Include(c => c.Product).ToList();
        }

        public Cell GetCellById(int id)
        {
            return _context.Cells.Include(c => c.WarehouseZone).Include(c => c.Product).FirstOrDefault(c => c.Id == id);
        }

        public void UpdateCell(Cell cell)
        {
            _context.Cells.Update(cell);
            _context.SaveChanges();
        }
    }
}
