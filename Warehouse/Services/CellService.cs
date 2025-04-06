using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Data.Repositories;
using Warehouse.Models;

namespace Warehouse.Services
{
    public class CellService : ICellService
    {
        private readonly ICellRepository _cellRepository;

        public CellService(ICellRepository cellRepository)
        {
            _cellRepository = cellRepository;
        }

        public IEnumerable<Cell> GetAllCells()
        {
            return _cellRepository.GetAllCells();
        }
        public Cell? GetCellById(int id)
        {
            return _cellRepository.GetCellById(id);
        }
        public Cell? GetCellByProduct(int id)
        {
            return _cellRepository.GetCellByProduct(id);
        }

        public void AddCell(Cell cell)
        {
            _cellRepository.AddCell(cell);
        }

        public void UpdateCell(Cell cell)
        {
            _cellRepository.UpdateCell(cell);
        }

        public void DeleteCell(int id)
        {
            _cellRepository.DeleteCell(id);
        }
    }
}
