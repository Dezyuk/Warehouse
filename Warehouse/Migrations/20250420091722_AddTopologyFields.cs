using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Warehouse.Migrations
{
    /// <inheritdoc />
    public partial class AddTopologyFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WarehouseZoneType",
                table: "Cells",
                newName: "ZoneType");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Cells",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Cells");

            migrationBuilder.RenameColumn(
                name: "ZoneType",
                table: "Cells",
                newName: "WarehouseZoneType");
        }
    }
}
