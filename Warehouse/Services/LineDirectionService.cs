using System.Collections.Generic;
using System.Linq;
using Warehouse.Models;

namespace Warehouse.Services
{
    public interface ILineDirectionService
    {

        bool CalculateOptimalDirection(IEnumerable<Cell> cells);
    }

    public class LineDirectionService : ILineDirectionService
    {
        public bool CalculateOptimalDirection(IEnumerable<Cell> cells)
        {
            var storageCells = cells.Where(c => c.ZoneType == ZoneType.Storage).ToList();

            var rowStats = storageCells
                .GroupBy(c => c.Y)
                .Select(g => new { Line = g.Key, Count = g.Count() });

            var colStats = storageCells
                .GroupBy(c => c.X)
                .Select(g => new { Line = g.Key, Count = g.Count() });

            return rowStats.Average(x => x.Count) >= colStats.Average(x => x.Count);
        }
    }
}
