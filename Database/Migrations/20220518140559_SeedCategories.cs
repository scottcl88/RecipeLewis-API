using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    public partial class SeedCategories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Alias",
                table: "Categories");

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "CreatedByUserId", "CreatedDateTime", "DeletedByUserId", "DeletedDateTime", "ModifiedByUserId", "ModifiedDateTime", "Name" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2022, 5, 18, 14, 5, 59, 246, DateTimeKind.Utc).AddTicks(9965), null, null, null, null, "Any" },
                    { 2, null, new DateTime(2022, 5, 18, 14, 5, 59, 246, DateTimeKind.Utc).AddTicks(9983), null, null, null, null, "Breakfast" },
                    { 3, null, new DateTime(2022, 5, 18, 14, 5, 59, 246, DateTimeKind.Utc).AddTicks(9989), null, null, null, null, "Lunch" },
                    { 4, null, new DateTime(2022, 5, 18, 14, 5, 59, 246, DateTimeKind.Utc).AddTicks(9994), null, null, null, null, "Dinner" },
                    { 5, null, new DateTime(2022, 5, 18, 14, 5, 59, 247, DateTimeKind.Utc), null, null, null, null, "Snack" },
                    { 6, null, new DateTime(2022, 5, 18, 14, 5, 59, 247, DateTimeKind.Utc).AddTicks(6), null, null, null, null, "Desert" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 6);

            migrationBuilder.AddColumn<string>(
                name: "Alias",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
