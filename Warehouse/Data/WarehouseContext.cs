using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Warehouse.Models;

namespace Warehouse.Data
{
    public class WarehouseContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<WarehouseZone> WarehouseZones { get; set; }
        public DbSet<Cell> Cells { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Укажите строку подключения к PostgreSQL
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=Warehouse_DB;Username=postgres;Password=123456");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cell>()
                .HasOne(c => c.WarehouseZone)
                .WithMany(z => z.Cells)
                .HasForeignKey(c => c.WarehouseZoneId);

            modelBuilder.Entity<Cell>()
                .HasOne(c => c.Product)
                .WithMany()
                .HasForeignKey(c => c.ProductId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Order>()
                .HasMany(o => o.Products)
                .WithMany()
                .UsingEntity(j => j.ToTable("OrderProducts")); // Связь Many-to-Many
        }
    }
}
