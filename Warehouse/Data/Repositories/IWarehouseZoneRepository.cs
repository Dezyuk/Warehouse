using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Models;

namespace Warehouse.Data.Repositories
{
    interface IWarehouseZoneRepository
    {
        IEnumerable<WarehouseZone> GetAllWarehouseZones();
        WarehouseZone GetWarehouseZoneById(int id);
        void AddWarehouseZone(WarehouseZone warehouseZone);
        void UpdateWarehouseZone(WarehouseZone warehouseZone);
        void DeleteWarehouseZone(int id);
    }
}
