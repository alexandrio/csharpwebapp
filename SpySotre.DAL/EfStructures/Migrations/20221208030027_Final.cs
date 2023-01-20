using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpyStore.DAL.EfStructures.Migrations
{
    /// <inheritdoc />
    public partial class Final : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "OrderTotal",
                schema: "Store",
                table: "Orders",
                type: "money",
                nullable: false,
                computedColumnSql: "Store.GetOrderTotal([Id])");

            migrationBuilder.AddColumn<decimal>(
                name: "LineItemTotal",
                schema: "Store",
                table: "OrderDetails",
                type: "money",
                nullable: false,
                computedColumnSql: "[Quantity]*[UnitCost]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderTotal",
                schema: "Store",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "LineItemTotal",
                schema: "Store",
                table: "OrderDetails");
        }
    }
}
