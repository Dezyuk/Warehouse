using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using Warehouse.Models;

namespace Warehouse.Services
{
    public interface IPlacementService
    {
        void PlaceAllProducts();
    }

    public class PlacementService : IPlacementService
    {
        private const int MaxPerCell = 1000;

        private readonly ICellService _cellService;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly AbcXyzService _abcXyzService;

        public PlacementService(
            ICellService cellService,
            IProductService productService,
            IOrderService orderService,
            AbcXyzService abcXyzService)
        {
            _cellService = cellService;
            _productService = productService;
            _orderService = orderService;
            _abcXyzService = abcXyzService;
        }

        public void PlaceAllProducts()
        {
            // 1. Остатки - работает заебись
            var toPlace = ComputeToPlace();

            // 2. Зоны хранения - работает заебись
            var allCells = _cellService.GetAllCells().ToList();
            var storageCells = allCells.Where(c => c.ZoneType == ZoneType.Storage).ToList();
            var zones = ComputeZones(storageCells);

            // 3. Кластеры в каждой зоне - работает заебись
            var clustersPerZone = zones
                .Select(zone => ComputeClusters(zone, allCells))
                .ToList();

            // 4. Заполнение существующих кластеров
            foreach (var stat in _abcXyzService.ClassifyByAbcXyz(_orderService.GetAllOrders()))
            {
                if (!toPlace.TryGetValue(stat.ProductId, out var remaining) || remaining <= 0)
                    continue;

                // 4.1 Существующие кластеры
                remaining = FillExistingClusters(stat.ProductId, clustersPerZone, remaining);

                // 5. Новые кластеры
                if (remaining > 0)
                    remaining = FillNewClusters(stat.ProductId, clustersPerZone, remaining, allCells);

                // 6. Фолбэк одиночные
                if (remaining > 0)
                    remaining = FillSingleCells(stat.ProductId, storageCells, remaining, allCells);

                if (remaining > 0)
                    continue;
            }

        }

        /// <summary>
        /// Вычисляет, сколько единиц каждого товара нужно разместить,
        /// учитывая текущие заполняемые Storage ячейки и общий запас.
        /// </summary>
        private Dictionary<int, int> ComputeToPlace()
        {
            // Получаем все Storage ячейки
            var allCells = _cellService.GetAllCells()
                .Where(c => c.ZoneType == ZoneType.Storage)
                .ToList();

            // Считаем, сколько каждой единицы уже размещено
            var placed = allCells
                .Where(c => c.ProductId.HasValue)
                .GroupBy(c => c.ProductId!.Value)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(c => c.Quantity)
                );

            // Берём текущие товары с общим количеством на складе
            var products = _productService.GetAllProducts().ToList();

            var toPlace = new Dictionary<int, int>();
            foreach (var product in products)
            {
                var totalQty = product.Quantity;
                var placedQty = placed.GetValueOrDefault(product.Id, 0);
                var need = totalQty - placedQty;
                if (need > 0)
                    toPlace[product.Id] = need;
            }

            return toPlace;
        }
        private List<List<Cell>> ComputeZones(List<Cell> storageCells)
        {
            var visited = new HashSet<(int x, int y)>();
            var zones = new List<List<Cell>>();

            foreach (var cell in storageCells)
            {
                var key = (cell.X, cell.Y);
                if (visited.Contains(key))
                    continue;

                var zone = new List<Cell>();
                var stack = new Stack<Cell>();
                stack.Push(cell);
                visited.Add(key);

                while (stack.Count > 0)
                {
                    var current = stack.Pop();
                    zone.Add(current);

                    var neighbors = storageCells.Where(c =>
                        !visited.Contains((c.X, c.Y)) &&
                        ((Math.Abs(c.X - current.X) == 1 && c.Y == current.Y) ||
                         (Math.Abs(c.Y - current.Y) == 1 && c.X == current.X)));

                    foreach (var neighbor in neighbors)
                    {
                        visited.Add((neighbor.X, neighbor.Y));
                        stack.Push(neighbor);
                    }
                }

                zones.Add(zone);
            }

            return zones;
        }

        /// <summary>
        /// Разбивает зону на линейные кластеры,
        /// обходя от каждой Passage-ячейки вдоль прямой.
        /// Остальные ячейки группирует как отдельные "мертвые зоны" по связности.
        /// </summary>
        private List<List<Cell>> ComputeClusters(List<Cell> zone, List<Cell> allCells)
        {
            var storageSet = new HashSet<(int, int)>(zone.Select(c => (c.X, c.Y)));
            var assigned = new HashSet<(int, int)>();
            var clusters = new List<List<Cell>>();

            // Шаг: для каждой Passage-ячейки
            foreach (var p in allCells.Where(c => c.ZoneType == ZoneType.Passage))
            {
                // для каждого из 4 направлений
                var dirs = new[] { (1, 0), (-1, 0), (0, 1), (0, -1) };
                foreach (var (dx, dy) in dirs)
                {
                    var line = new List<Cell>();
                    var x = p.X + dx;
                    var y = p.Y + dy;
                    // если сосед — Storage
                    while (storageSet.Contains((x, y)) && !assigned.Contains((x, y)))
                    {
                        var cell = zone.First(c => c.X == x && c.Y == y);
                        line.Add(cell);
                        assigned.Add((x, y));
                        x += dx;
                        y += dy;
                    }
                    if (line.Any()) clusters.Add(line);
                }
            }

            // Группируем оставшиеся ячейки в "мертвые зоны" по связности
            var deadCells = zone
                .Where(c => !assigned.Contains((c.X, c.Y)))
                .ToList();
            var deadZones = ComputeZones(deadCells);
            foreach (var dz in deadZones)
                clusters.Add(dz);

            return clusters;
        }
        private int FillExistingClusters(int productId, List<List<List<Cell>>> clustersPerZone, int remaining)
        {
            foreach (var zoneClusters in clustersPerZone)
            {
                foreach (var cluster in zoneClusters)
                {
                    if (cluster.Any(c => c.ProductId == productId))
                    {
                        remaining = FillCluster(cluster, productId, remaining);
                        if (remaining == 0) return 0;
                    }
                }
            }
            return remaining;
        }

        private int FillNewClusters(
    int productId,
    List<List<List<Cell>>> clustersPerZone,
    int remaining,
    List<Cell> allCells)
        {
            // Выбираем только пустые кластеры (все ячейки свободны)
            var emptyClusters = clustersPerZone
                .SelectMany(zoneClusters => zoneClusters)
                .Where(cluster => cluster.All(c => c.ProductId == null))
                .ToList();

            // Получаем все ячейки зоны отгрузки
            var shippingCells = allCells
                .Where(c => c.ZoneType == ZoneType.ShippingArea)
                .ToList();

            // Функция вычисления Манхэттенского расстояния
            int ManhattanDistance(Cell a, Cell b) =>
                Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);

            // Сортируем кластеры: сначала ближе к отгрузке
            var sortedClusters = emptyClusters
                .Select(cluster => new
                {
                    Cluster = cluster,
                    MinDistToShipping = cluster.Min(cell =>
                        shippingCells.Min(ship => ManhattanDistance(cell, ship)))
                })
                .OrderBy(x => x.MinDistToShipping)
                .Select(x => x.Cluster)
                .ToList();

            // Получаем продукт
            var product = _productService.GetProductById(productId);

            foreach (var cluster in sortedClusters)
            {
                foreach (var cell in cluster.AsEnumerable().Reverse()) // от дальнего конца
                {
                    if (remaining <= 0) break;

                    var toPlace = Math.Min(MaxPerCell, remaining);
                    cell.ProductId = productId;
                    cell.Product = product;
                    cell.Quantity = toPlace;
                    _cellService.UpdateCell(cell);
                    remaining -= toPlace;
                }

                if (remaining <= 0)
                    break;
            }

            return remaining;
        }



        private int FillSingleCells(int productId, List<Cell> storageCells, int remaining, List<Cell> allCells)
        {
            var free = storageCells
                .Where(c => c.ProductId == null)
                .OrderBy(c => Math.Abs(c.X) + Math.Abs(c.Y)) // приближённая сортировка
                .ToList();
            foreach (var cell in free)
            {
                if (remaining == 0) break;

                if (!IsCellReachable(cell, allCells))
                    continue;

                var take = Math.Min(MaxPerCell, remaining);
                cell.ProductId = productId;
                cell.Product = _productService.GetProductById(productId);
                cell.Quantity = take;
                _cellService.UpdateCell(cell);
                remaining -= take;
            }
            return remaining;
        }

        private int FillCluster(List<Cell> cluster, int productId, int remaining)
        {
            foreach (var cell in cluster.OrderByDescending(c => c.X + c.Y))
            {
                if (remaining == 0) break;
                var canTake = Math.Min(MaxPerCell - cell.Quantity, remaining);
                if (canTake <= 0) continue;
                cell.ProductId = productId;
                cell.Product = _productService.GetProductById(productId);
                cell.Quantity += canTake;
                _cellService.UpdateCell(cell);
                remaining -= canTake;
            }
            return remaining;
        }

        private bool IsCellReachable(Cell target, List<Cell> allCells)
        {
            var width = allCells.Max(c => c.X) + 1;
            var height = allCells.Max(c => c.Y) + 1;

            var grid = new Cell[width, height];
            foreach (var cell in allCells)
                grid[cell.X, cell.Y] = cell;

            var visited = new HashSet<(int x, int y)>();
            var queue = new Queue<(int x, int y)>();

            // старт из всех Passage-ячееек
            foreach (var start in allCells.Where(c => c.ZoneType == ZoneType.Passage))
            {
                queue.Enqueue((start.X, start.Y));
                visited.Add((start.X, start.Y));
            }

            while (queue.Count > 0)
            {
                var (x, y) = queue.Dequeue();
                if (x == target.X && y == target.Y)
                    return true;

                var neighbors = new (int dx, int dy)[]
                {
            (-1, 0), (1, 0), (0, -1), (0, 1)
                };

                foreach (var (dx, dy) in neighbors)
                {
                    int nx = x + dx;
                    int ny = y + dy;

                    if (nx < 0 || ny < 0 || nx >= width || ny >= height)
                        continue;

                    if (visited.Contains((nx, ny)))
                        continue;

                    var neighbor = grid[nx, ny];
                    if (neighbor == null)
                        continue;

                    // Доступны только пустые ячейки типа Storage
                    bool isPassable = neighbor.ZoneType == ZoneType.Storage && neighbor.ProductId == null;
                    if (!isPassable && neighbor.ZoneType != ZoneType.Passage)
                        continue;

                    queue.Enqueue((nx, ny));
                    visited.Add((nx, ny));
                }
            }
            return false;
        }

        
    }
}
