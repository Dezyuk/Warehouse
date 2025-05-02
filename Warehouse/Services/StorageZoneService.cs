using Warehouse.Models;

namespace Warehouse.Services
{
    public class StorageZoneService
    {
        private readonly int[,] _directions = new int[,]
        {
            { 0, 1 }, // вправо
            { 1, 0 }, // вниз
            { 0, -1 }, // влево
            { -1, 0 }  // вверх
        };

        public Dictionary<int, List<Cell>> ClusterStorageZones(List<Cell> allCells)
        {
            var storageCells = allCells
                .Where(c => c.ZoneType == ZoneType.Storage)
                .ToDictionary(c => (c.X, c.Y), c => c);

            var visited = new HashSet<(int x, int y)>();
            var zones = new Dictionary<int, List<Cell>>();
            int zoneId = 0;

            foreach (var (coord, cell) in storageCells)
            {
                if (visited.Contains(coord))
                    continue;

                var zone = new List<Cell>();
                var queue = new Queue<(int x, int y)>();
                queue.Enqueue(coord);
                visited.Add(coord);

                while (queue.Count > 0)
                {
                    var (x, y) = queue.Dequeue();
                    var current = storageCells[(x, y)];
                    zone.Add(current);

                    for (int d = 0; d < 4; d++)
                    {
                        int nx = x + _directions[d, 0];
                        int ny = y + _directions[d, 1];

                        var nextCoord = (nx, ny);
                        if (storageCells.ContainsKey(nextCoord) && !visited.Contains(nextCoord))
                        {
                            visited.Add(nextCoord);
                            queue.Enqueue(nextCoord);
                        }
                    }
                }

                zones[zoneId++] = zone;
            }

            return zones;
        }
    }
}
