using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SunBattery_Api.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "34aa2623-6a9f-4f24-b3dc-afee4c6a0f6e", "1", "Admin", "Admin" },
                    { "c8d67b89-0c48-4a5f-86a9-d038aaf553d6", "2", "User", "User" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "34aa2623-6a9f-4f24-b3dc-afee4c6a0f6e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c8d67b89-0c48-4a5f-86a9-d038aaf553d6");
        }
    }
}
