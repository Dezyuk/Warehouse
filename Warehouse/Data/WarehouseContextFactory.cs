using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Warehouse.Data
{
    public class WarehouseContextFactory : IDesignTimeDbContextFactory<WarehouseContext>
    {
        public WarehouseContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<WarehouseContext>();
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=Warehouse_DB;Username=postgres;Password=123456");
            return new WarehouseContext(optionsBuilder.Options);
        }
    }
}
