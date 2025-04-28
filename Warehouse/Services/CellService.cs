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

        public IEnumerable<Cell> GetCellsByProduct(int productId)
        {
            return _cellRepository.GetAllCells()
                .Where(c => c.ProductId == productId)
                .OrderBy(c => c.Quantity)
                .ToList();
        }

        // Списание из ячеек по логике: сначала из тех, где меньше всего
        public void DeductFromCells(int productId, int quantity)
        {
            var toDeduct = quantity;
            var cells = GetCellsByProduct(productId).ToList();

            foreach (var cell in cells)
            {
                if (toDeduct <= 0)
                    break;

                var take = System.Math.Min(cell.Quantity, toDeduct);
                cell.Quantity -= take;
                toDeduct -= take;
                if (cell.Quantity == 0)
                {
                    cell.Product = null;
                    cell.ProductId = null;
                }
                _cellRepository.UpdateCell(cell);
            }

            if (toDeduct > 0)
                throw new InvalidOperationException($"Не удалось списать {quantity} шт.: осталось {toDeduct} шт. в ячейках.");
        }
    }
}
