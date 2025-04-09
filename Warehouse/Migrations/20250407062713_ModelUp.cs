using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Warehouse.Migrations
{
    /// <inheritdoc />
    public partial class ModelUp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cells_WarehouseZones_WarehouseZoneId",
                table: "Cells");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderProducts_Products_ProductsId",
                table: "OrderProducts");

            migrationBuilder.DropTable(
                name: "WarehouseZones");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderProducts",
                table: "OrderProducts");

            migrationBuilder.DropIndex(
                name: "IX_OrderProducts_ProductsId",
                table: "OrderProducts");

            migrationBuilder.DropIndex(
                name: "IX_Cells_WarehouseZoneId",
                table: "Cells");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "MinimumStock",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsOccupied",
                table: "Cells");

            migrationBuilder.RenameColumn(
                name: "ProductsId",
                table: "OrderProducts",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "WarehouseZoneId",
                table: "Cells",
                newName: "WarehouseZoneType");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Products",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "OrderProducts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Discount",
                table: "OrderProducts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceAtOrder",
                table: "OrderProducts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderProducts",
                table: "OrderProducts",
                columns: new[] { "OrderId", "ProductId" });

            migrationBuilder.CreateIndex(
                name: "IX_OrderProducts_ProductId",
                table: "OrderProducts",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderProducts_Products_ProductId",
                table: "OrderProducts",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderProducts_Products_ProductId",
                table: "OrderProducts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderProducts",
                table: "OrderProducts");

            migrationBuilder.DropIndex(
                name: "IX_OrderProducts_ProductId",
                table: "OrderProducts");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "OrderProducts");

            migrationBuilder.DropColumn(
                name: "Discount",
                table: "OrderProducts");

            migrationBuilder.DropColumn(
                name: "PriceAtOrder",
                table: "OrderProducts");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "OrderProducts",
                newName: "ProductsId");

            migrationBuilder.RenameColumn(
                name: "WarehouseZoneType",
                table: "Cells",
                newName: "WarehouseZoneId");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Products",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MinimumStock",
                table: "Products",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsOccupied",
                table: "Cells",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderProducts",
                table: "OrderProducts",
                columns: new[] { "OrderId", "ProductsId" });

            migrationBuilder.CreateTable(
                name: "WarehouseZones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarehouseZones", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderProducts_ProductsId",
                table: "OrderProducts",
                column: "ProductsId");

            migrationBuilder.CreateIndex(
                name: "IX_Cells_WarehouseZoneId",
                table: "Cells",
                column: "WarehouseZoneId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cells_WarehouseZones_WarehouseZoneId",
                table: "Cells",
                column: "WarehouseZoneId",
                principalTable: "WarehouseZones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderProducts_Products_ProductsId",
                table: "OrderProducts",
                column: "ProductsId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
