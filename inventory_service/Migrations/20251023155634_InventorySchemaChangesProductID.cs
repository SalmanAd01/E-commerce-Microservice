using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace inventory_service.Migrations
{
    /// <inheritdoc />
    public partial class InventorySchemaChangesProductID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "product_id",
                table: "inventories",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "product_id",
                table: "inventories",
                type: "character varying",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
