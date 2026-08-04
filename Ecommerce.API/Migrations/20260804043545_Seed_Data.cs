using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Ecommerce.API.Migrations
{
    /// <inheritdoc />
    public partial class Seed_Data : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "CreatedAt", "DeletedAt", "Description", "IsDeleted", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Phones, laptops, and gadgets", false, "Electronics", null },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Appliances and kitchenware", false, "Home & Kitchen", null },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Fiction and non-fiction books", false, "Books", null }
                });

            migrationBuilder.InsertData(
                table: "Sales",
                columns: new[] { "SaleId", "CreatedAt", "CustomerName", "DeletedAt", "IsDeleted", "ProductId", "Quantity", "SaleDate", "UnitPrice", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Jane Doe", null, false, 1, 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 999.99m, null },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "John Smith", null, false, 2, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 29.99m, null },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Jane Doe", null, false, 4, 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 39.99m, null }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "CategoryId", "CreatedAt", "DeletedAt", "Description", "IsDeleted", "Name", "Price", "StockQuantity", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "15-inch laptop, 16GB RAM", false, "Laptop", 999.99m, 10, null },
                    { 2, 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Bluetooth mouse, ergonomic design", false, "Wireless Mouse", 29.99m, 50, null },
                    { 3, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "700W countertop blender", false, "Blender", 49.99m, 20, null },
                    { 4, 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Beginner to advanced C# book", false, "C# Programming Guide", 39.99m, 15, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Sales",
                keyColumn: "SaleId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Sales",
                keyColumn: "SaleId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Sales",
                keyColumn: "SaleId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 3);
        }
    }
}
