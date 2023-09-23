using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIStoreApp.Migrations
{
    public partial class addRefreshTokenFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpireTime",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpireTime",
                table: "AspNetUsers");

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
    }
}
