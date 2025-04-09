//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Warehouse.Models;

//namespace Warehouse.Data.Repositories
//{
//    //Юзлес
//    class WarehouseZoneRepository : IWarehouseZoneRepository
//    {
//        private readonly WarehouseContext _context;

//        public WarehouseZoneRepository(WarehouseContext context)
//        {
//            _context = context;
//        }

//        public IEnumerable<WarehouseZone> GetAllWarehouseZones()
//        {
//            return _context.WarehouseZones.Include(w => w.Cells).ToList();
//        }

//        public WarehouseZone GetWarehouseZoneById(int id)
//        {
//            return _context.WarehouseZones.Include(z => z.Cells).FirstOrDefault(z => z.Id == id);
//        }

//        public void AddWarehouseZone(WarehouseZone warehouseZone)
//        {
//            _context.WarehouseZones.Add(warehouseZone);
//            _context.SaveChanges();
//        }

//        public void UpdateWarehouseZone(WarehouseZone warehouseZone)
//        {
//            _context.WarehouseZones.Update(warehouseZone);
//            _context.SaveChanges();
//        }

//        public void DeleteWarehouseZone(int id)
//        {
//            var warehouseZone = _context.WarehouseZones.Find(id);
//            if (warehouseZone != null)
//            {
//                _context.WarehouseZones.Remove(warehouseZone);
//                _context.SaveChanges();
//            }
//        }
//    }
//}
