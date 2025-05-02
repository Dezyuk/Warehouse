using System;
using System.Collections.Generic;
using System.Linq;
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

        public void DeductFromCells(int productId, int quantity)
        {
            var toDeduct = quantity;
            var cells = GetCellsByProduct(productId).ToList();

            foreach (var cell in cells)
            {
                if (toDeduct <= 0)
                    break;

                var take = Math.Min(cell.Quantity, toDeduct);
                cell.Quantity -= take;
                toDeduct -= take;
                if (cell.Quantity == 0)
                {
                    cell.Product = null;
                    cell.ProductId = null;
                    cell.FillColor = null;
                }
                _cellRepository.UpdateCell(cell);
            }

            if (toDeduct > 0)
                throw new InvalidOperationException($"Не удалось списать {quantity} шт.: осталось {toDeduct} шт. в ячейках.");
        }

        private Dictionary<int, string> _productColors = new();
        private Random _random = new Random();

        private string GetRandomColor()
        {
            return $"#{_random.Next(0x1000000):X6}";
        }

        public void AssignColorsToProductCells(IEnumerable<Cell> cells)
        {
            foreach (var cell in cells)
            {
                if (cell.ProductId == null)
                {
                    cell.FillColor = null;
                    continue;
                }

                if (cell.FillColor != null)
                {
                    _productColors[cell.ProductId.Value] = cell.FillColor;
                }

                if (!_productColors.ContainsKey(cell.ProductId.Value))
                {
                    _productColors[cell.ProductId.Value] = GetRandomColor();
                }

                cell.FillColor = _productColors[cell.ProductId.Value];
            }
        }

        // 🔷 Новый метод: разделение на изолированные зоны хранения
        public List<List<Cell>> GetSeparatedStorageZones()
        {
            var allCells = _cellRepository.GetAllCells().ToList();
            var storageCells = allCells
                .Where(c => c.ZoneType == ZoneType.Storage)
                .ToList();

            var visited = new HashSet<(int, int)>();
            var zones = new List<List<Cell>>();

            foreach (var cell in storageCells)
            {
                if (visited.Contains((cell.X, cell.Y)))
                    continue;

                var zone = new List<Cell>();
                var queue = new Queue<Cell>();
                queue.Enqueue(cell);
                visited.Add((cell.X, cell.Y));

                while (queue.Count > 0)
                {
                    var current = queue.Dequeue();
                    zone.Add(current);

                    var neighbors = storageCells.Where(c =>
                        !visited.Contains((c.X, c.Y)) &&
                        ((Math.Abs(c.X - current.X) == 1 && c.Y == current.Y) ||
                         (Math.Abs(c.Y - current.Y) == 1 && c.X == current.X)));

                    foreach (var neighbor in neighbors)
                    {
                        visited.Add((neighbor.X, neighbor.Y));
                        queue.Enqueue(neighbor);
                    }
                }

                zones.Add(zone);
            }

            return zones;
        }
    }
}
