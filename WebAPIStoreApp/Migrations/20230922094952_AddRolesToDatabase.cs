using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIStoreApp.Migrations
{
    public partial class AddRolesToDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "2a80e0b0-16ab-4599-a542-035abfa326f0", "c7f47911-88d4-4f2f-9281-c79406714a3d", "User", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "e5c63b04-3d15-4891-8055-7d0897598b16", "1d3836f2-a695-47b7-8966-078b88045e9a", "Admin", "ADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "e915d6a0-7b67-4ae2-8c48-bbfde951118a", "31b821f2-8330-454c-854c-8add53926d82", "Editor", "EDITOR" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2a80e0b0-16ab-4599-a542-035abfa326f0");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e5c63b04-3d15-4891-8055-7d0897598b16");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e915d6a0-7b67-4ae2-8c48-bbfde951118a");
        }
    }
}
