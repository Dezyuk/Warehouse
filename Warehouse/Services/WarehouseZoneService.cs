using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Data.Repositories;
using Warehouse.Models;

namespace Warehouse.Services
{
    class WarehouseZoneService : IWarehouseZoneService
    {
        private readonly IWarehouseZoneRepository _warehouseZoneRepository;

        public WarehouseZoneService(IWarehouseZoneRepository warehouseZoneRepository)
        {
            _warehouseZoneRepository = warehouseZoneRepository;
        }

        public IEnumerable<WarehouseZone> GetAllWarehouseZones()
        {
            return _warehouseZoneRepository.GetAllWarehouseZones();
        }

        public WarehouseZone GetWarehouseZoneById(int id)
        {
            return _warehouseZoneRepository.GetWarehouseZoneById(id);
        }

        public void AddWarehouseZone(WarehouseZone warehouseZone)
        {
            _warehouseZoneRepository.AddWarehouseZone(warehouseZone);
        }

        public void UpdateWarehouseZone(WarehouseZone warehouseZone)
        {
            _warehouseZoneRepository.UpdateWarehouseZone(warehouseZone);
        }

        public void DeleteWarehouseZone(int id)
        {
            _warehouseZoneRepository.DeleteWarehouseZone(id);
        }
    }
}
