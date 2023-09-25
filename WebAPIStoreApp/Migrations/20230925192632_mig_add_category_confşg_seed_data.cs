using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIStoreApp.Migrations
{
    public partial class mig_add_category_confşg_seed_data : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "567e6cb5-3c69-433e-9a56-0ffe54cd1124");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "61bf2521-905c-48ff-bd26-d4aa1219aa1e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a5c38703-de0f-4fdb-b1b2-a0b31818f6c2");

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryId);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "c35ad2c7-394b-4c17-b0e7-a2de07a55ecc", "e4509535-d23c-449b-bafd-017b870b0169", "Admin", "ADMIN" },
                    { "ce0996f8-27fe-47dc-919e-776719c27dd0", "11564ea4-f128-44bc-91c2-b5f968aa77a2", "Editor", "EDITOR" },
                    { "eb671d9d-2b92-4ad8-98a8-ceef952aba7a", "0c88a034-ee8d-4f8d-b79a-baf0dd309a20", "User", "USER" }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "CategoryName" },
                values: new object[,]
                {
                    { 1, "Computer Science" },
                    { 2, "Network" },
                    { 3, "Database Management Science" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c35ad2c7-394b-4c17-b0e7-a2de07a55ecc");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ce0996f8-27fe-47dc-919e-776719c27dd0");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "eb671d9d-2b92-4ad8-98a8-ceef952aba7a");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "567e6cb5-3c69-433e-9a56-0ffe54cd1124", "2bd81b5e-4bca-4eb1-a5f8-6852c7b14b22", "Admin", "ADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "61bf2521-905c-48ff-bd26-d4aa1219aa1e", "b0b61ce1-f2ec-4b93-9680-c5a23c48e908", "User", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "a5c38703-de0f-4fdb-b1b2-a0b31818f6c2", "1a0d15e5-7fd3-44c3-afac-6bf615ef85cb", "Editor", "EDITOR" });
        }
    }
}
