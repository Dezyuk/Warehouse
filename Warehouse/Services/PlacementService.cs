using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Warehouse.Models;

namespace Warehouse.Services
{
    public class PlacementService
    {
        private const int MaxPerCell = 1000;

        public void PlaceAllProducts(ObservableCollection<Cell> cells, IEnumerable<Product> products)
        {
            var cellMap = cells.ToDictionary(c => (c.X, c.Y));
            var storageCells = cells.Where(c => c.ZoneType == ZoneType.Storage).ToList();
            var passageCells = cells.Where(c => c.ZoneType == ZoneType.Passage).ToList();
            var passageDistances = ComputeDistancesFromSources(passageCells, cellMap);

            var sortedProducts = products
                .OrderByDescending(p => p.Quantity)
                .ToList();

            foreach (var product in sortedProducts)
            {
                // Сколько уже размещено
                int placed = storageCells
                    .Where(c => c.ProductId == product.Id)
                    .Sum(c => c.Quantity);

                int remaining = product.Quantity - placed;
                if (remaining <= 0)
                    continue;

                var existing = storageCells
                    .Where(c => c.ProductId == product.Id)
                    .ToList();

                // 1. Если товар уже размещён – продолжить рядом
                if (existing.Any())
                {
                    while (remaining > 0)
                    {
                        // Найти соседнюю линию
                        var adjLine = FindAdjacentColumn(existing, storageCells, passageDistances);
                        if (adjLine != null)
                        {
                            FillLine(adjLine, product, ref remaining);
                            existing.AddRange(adjLine);
                            continue;
                        }

                        // Иначе – одиночная соседняя ячейка
                        var neighbor = SelectBestNeighbor(existing, storageCells, cellMap, passageDistances);
                        if (neighbor != null)
                        {
                            int chunk = Math.Min(remaining, MaxPerCell);
                            neighbor.ProductId = product.Id;
                            neighbor.Product = product;
                            neighbor.Quantity = chunk;
                            remaining -= chunk;
                            existing.Add(neighbor);
                            continue;
                        }

                        break; // если негде разместить рядом
                    }
                }

                // 2. Если всё ещё есть остаток – новые полные линии
                if (remaining > 0)
                {
                    var lines = FindFullLines(storageCells, passageCells, passageDistances);
                    foreach (var line in lines)
                    {
                        if (remaining <= 0) break;
                        FillLine(line, product, ref remaining);
                    }
                }

                // 3. В крайний случай – любые свободные ячейки
                while (remaining > 0)
                {
                    int chunk = Math.Min(remaining, MaxPerCell);
                    var target = storageCells
                        .Where(c => c.ProductId == null && passageDistances.ContainsKey((c.X, c.Y)))
                        .OrderByDescending(c => passageDistances[(c.X, c.Y)])
                        .FirstOrDefault()
                        ?? storageCells.FirstOrDefault(c => c.ProductId == null);

                    if (target == null)
                        throw new InvalidOperationException("Недостаточно ячеек.");

                    target.ProductId = product.Id;
                    target.Product = product;
                    target.Quantity = chunk;
                    remaining -= chunk;
                }
            }
        }

        // 1. Формируем все полные линии от дальней точки до проезда
        private List<List<Cell>> FindFullLines(
    List<Cell> storageCells,
    List<Cell> passageCells,
    Dictionary<(int, int), int> passageDistances)
        {
            var lines = new List<List<Cell>>();
            foreach (var passage in passageCells)
            {
                foreach (var dir in new[] { (1, 0), (-1, 0), (0, 1), (0, -1) })
                {
                    var line = new List<Cell>();
                    int step = 1;
                    while (true)
                    {
                        var coord = (passage.X + dir.Item1 * step, passage.Y + dir.Item2 * step);
                        var cell = storageCells.FirstOrDefault(c => (c.X, c.Y) == coord && c.ProductId == null);
                        if (cell == null || cell.ZoneType == ZoneType.Passage) break; // Исключаем проезды

                        var tempLine = line.Append(cell).ToList();
                        if (!CheckNoBlockage(tempLine, storageCells, passageCells)) break;

                        line.Add(cell);
                        step++;
                    }
                    if (line.Any())
                        lines.Add(line);
                }
            }

            // Сортируем линии, чтобы дальше от проезда были первыми
            return lines
                .Select(l => new { Line = l, MinDist = l.Min(c => passageDistances.GetValueOrDefault((c.X, c.Y), 0)) })
                .OrderByDescending(x => x.MinDist)
                .Select(x => x.Line)
                .ToList();
        }

        // 2. Поиск соседнего столбца или ряда у уже размещённых
        private List<Cell>? FindAdjacentColumn(
    List<Cell> existing,
    List<Cell> allStorageCells,
    Dictionary<(int, int), int> passageDistances)
        {
            var directions = new (int dx, int dy)[]
            {
        (0, -1), (0, 1) // вверх, вниз
            };

            foreach (var cell in existing)
            {
                foreach (var (dx, dy) in directions)
                {
                    var group = GetVerticalLine(cell.X, cell.Y + dy, allStorageCells);
                    if (group.Count > 0 && group.All(c => c.ProductId == null))
                    {
                        // Сортируем по расстоянию до ближайшего товара того же типа
                        var sortedGroup = group
                            .OrderBy(c => GetDistanceToNearestProduct(existing, c, allStorageCells))
                            .ThenBy(c => passageDistances.GetValueOrDefault((c.X, c.Y), int.MaxValue))
                            .ToList();

                        return sortedGroup;
                    }
                }
            }

            return null;
        }

        private int GetDistanceToNearestProduct(List<Cell> existing, Cell target, List<Cell> allStorageCells)
        {
            // Находим все ячейки с тем же товаром, что и в alreadyPlaced
            var productCells = existing.Where(c => c.ProductId == target.ProductId).ToList();

            // Если таких ячеек нет, возвращаем максимальное значение
            if (!productCells.Any()) return int.MaxValue;

            // Ищем минимальное расстояние до ячейки с товаром
            int minDistance = int.MaxValue;
            foreach (var cell in productCells)
            {
                int dist = Math.Abs(cell.X - target.X) + Math.Abs(cell.Y - target.Y);
                minDistance = Math.Min(minDistance, dist);
            }

            return minDistance;
        }


        private List<Cell> GetVerticalLine(int startX, int startY, List<Cell> storageCells)
        {
            var line = new List<Cell>();

            // Ищем все ячейки по вертикали (вверх и вниз от начальной точки)
            var directions = new[] { 1, -1 }; // вниз и вверх

            foreach (var direction in directions)
            {
                int x = startX;
                int y = startY;
                while (true)
                {
                    y += direction;
                    var cell = storageCells.FirstOrDefault(c => c.X == x && c.Y == y);
                    if (cell == null || cell.ProductId != null) break; // Прерываем, если ячейка занята или не существует
                    line.Add(cell);
                }
            }

            return line;
        }
        // Заполнение линии ячеек чанками
        private void FillLine(List<Cell> line, Product product, ref int remaining)
        {
            var continuousEmptyCells = new List<Cell>();

            foreach (var cell in line)
            {
                if (cell.ProductId == null)
                {
                    continuousEmptyCells.Add(cell);
                }
                else
                {
                    if (continuousEmptyCells.Count > 0)
                    {
                        break;
                    }
                }
            }

            // Сортируем ячейки по минимальному расстоянию от уже размещенного товара
            continuousEmptyCells = continuousEmptyCells
                .OrderBy(c => GetDistanceToNearestProduct(line, c, line))
                .ToList();

            foreach (var cell in continuousEmptyCells)
            {
                if (remaining <= 0) break;

                int chunk = Math.Min(remaining, MaxPerCell);
                cell.ProductId = product.Id;
                cell.Product = product;
                cell.Quantity = chunk;
                remaining -= chunk;
            }
        }




        // Поиск лучшей соседней ячейки
        private Cell? SelectBestNeighbor(
    List<Cell> existing,
    List<Cell> storageCells,
    Dictionary<(int, int), Cell> cellMap,
    Dictionary<(int, int), int> passageDistances)
        {
            var neighbors = new HashSet<Cell>();
            foreach (var c in existing)
                foreach (var n in GetNeighbors(c, cellMap))
                    if (n.ZoneType == ZoneType.Storage && n.ProductId == null)
                        neighbors.Add(n); // Исключаем проезды

            return neighbors
                .Where(c => passageDistances.ContainsKey((c.X, c.Y)))
                .OrderByDescending(c => passageDistances[(c.X, c.Y)])
                .FirstOrDefault();
        }

        // Проверка, что нет блокировки путей
        private bool CheckNoBlockage(
            List<Cell> line,
            List<Cell> storageCells,
            List<Cell> passageCells)
        {
            var productId = line.FirstOrDefault()?.ProductId;
            var map = storageCells.Concat(passageCells).ToDictionary(c => (c.X, c.Y), c => c);

            return line.All(target =>
            {
                var visited = new HashSet<Cell>();
                var queue = new Queue<Cell>(passageCells);

                while (queue.Count > 0)
                {
                    var cur = queue.Dequeue();
                    foreach (var n in GetNeighbors(cur, map))
                    {
                        if (visited.Contains(n)) continue;
                        if (n == target) return true;

                        bool canPass = n.ZoneType == ZoneType.Storage &&
                                       (n.ProductId == null || n.ProductId == productId || n == target);

                        if (canPass)
                        {
                            visited.Add(n);
                            queue.Enqueue(n);
                        }
                    }
                }
                return false;
            });
        }

        // Достижимость от проезда через пустые хран. ячейки
        private bool IsReachableFromPassage(
            Cell target,
            Dictionary<(int, int), Cell> map)
        {
            var visited = new HashSet<Cell>();
            var queue = new Queue<Cell>(map.Values.Where(c => c.ZoneType == ZoneType.Passage));
            while (queue.Count > 0)
            {
                var cur = queue.Dequeue();
                foreach (var n in GetNeighbors(cur, map))
                {
                    if (visited.Contains(n)) continue;
                    if (n == target) return true;
                    if (n.ZoneType == ZoneType.Storage && (n.ProductId == null || n == target))
                    {
                        visited.Add(n);
                        queue.Enqueue(n);
                    }
                }
            }
            return false;
        }

        // Четыре ортогональных соседа
        private IEnumerable<Cell> GetNeighbors(
            Cell c,
            Dictionary<(int, int), Cell> map)
        {
            var dirs = new (int dx, int dy)[] { (1, 0), (-1, 0), (0, 1), (0, -1) };
            foreach (var d in dirs)
                if (map.TryGetValue((c.X + d.dx, c.Y + d.dy), out var n))
                    yield return n;
        }

        // Расстояния через BFS от проездов
        private Dictionary<(int, int), int> ComputeDistancesFromSources(
            List<Cell> sources,
            Dictionary<(int, int), Cell> cellMap)
        {
            var distances = new Dictionary<(int, int), int>();
            var queue = new Queue<Cell>();
            foreach (var s in sources)
            {
                distances[(s.X, s.Y)] = 0;
                queue.Enqueue(s);
            }
            while (queue.Count > 0)
            {
                var cur = queue.Dequeue();
                var d0 = distances[(cur.X, cur.Y)];
                foreach (var n in GetNeighbors(cur, cellMap))
                {
                    var key = (n.X, n.Y);
                    if (distances.ContainsKey(key)) continue;
                    if (n.ZoneType == ZoneType.Passage || (n.ZoneType == ZoneType.Storage && n.ProductId == null))
                    {
                        distances[key] = d0 + 1;
                        queue.Enqueue(n);
                    }
                }
            }
            return distances;
        }
    }
}