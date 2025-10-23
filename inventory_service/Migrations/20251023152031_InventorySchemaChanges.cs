using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace inventory_service.Migrations
{
    /// <inheritdoc />
    public partial class InventorySchemaChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "product_id",
                table: "inventories",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "product_id",
                table: "inventories");
        }
    }
}
