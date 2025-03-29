using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.DataModels
{
    /// <inheritdoc />
    public partial class NestCost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Wallets",
                keyColumn: "Id",
                keyValue: new Guid("c9e16434-2373-43ba-bf87-162c60e10995"));

            migrationBuilder.DeleteData(
                table: "Wallets",
                keyColumn: "Id",
                keyValue: new Guid("fecd44ee-dadc-41f5-9d9d-75337b85ed52"));

            migrationBuilder.AddColumn<decimal>(
                name: "NetCost",
                table: "Payments",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "RefExternalTransactionCode",
                table: "Payments",
                type: "longtext",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4d71fc1c-2e6d-4b37-8d5e-a97538ac2f7d", new DateTime(2025, 3, 29, 15, 18, 14, 1, DateTimeKind.Utc).AddTicks(3534), "AQAAAAIAAYagAAAAEFYEEvX/rYRDLK4ydc3VpVdbR++48U+jUkLeeYOdKLh6giI1+bZfwlmPoTpQS8yHsg==", "75b112f8-1f02-4758-9b52-f00a9f554bd2" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "9e224968-33e4-4652-b7b7-8574d048cdb9",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a716b4dd-63f9-44b3-8bb8-01ea27e62004", new DateTime(2025, 3, 29, 15, 18, 14, 95, DateTimeKind.Utc).AddTicks(2023), "AQAAAAIAAYagAAAAEO9S3pGZTv60BmR2saIy00k7UIwPLt6zIJrwxMZS4t8gHCVnn8xACoIDUW1tWHF6MQ==", "b7c1251a-dcb7-4cc1-866b-bcc5e60c5691" });

            migrationBuilder.InsertData(
                table: "Wallets",
                columns: new[] { "Id", "Balance", "CreatedAt", "CreatedBy", "IsDeleted", "LastModified", "LastModifiedBy", "UserId" },
                values: new object[,]
                {
                    { new Guid("2a58d4ca-7bb6-4612-82f8-dd425c4c1441"), 1000m, new DateTime(2025, 3, 29, 22, 18, 14, 97, DateTimeKind.Local).AddTicks(6062), null, false, null, null, "8e445865-a24d-4543-a6c6-9443d048cdb9" },
                    { new Guid("38b7f915-aa30-4adb-9435-9f091546e505"), 500m, new DateTime(2025, 3, 29, 22, 18, 14, 97, DateTimeKind.Local).AddTicks(6126), null, false, null, null, "9e224968-33e4-4652-b7b7-8574d048cdb9" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Wallets",
                keyColumn: "Id",
                keyValue: new Guid("2a58d4ca-7bb6-4612-82f8-dd425c4c1441"));

            migrationBuilder.DeleteData(
                table: "Wallets",
                keyColumn: "Id",
                keyValue: new Guid("38b7f915-aa30-4adb-9435-9f091546e505"));

            migrationBuilder.DropColumn(
                name: "NetCost",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "RefExternalTransactionCode",
                table: "Payments");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "892daabf-10ad-49a5-87b5-2d0a78804239", new DateTime(2025, 3, 27, 6, 38, 4, 165, DateTimeKind.Utc).AddTicks(5324), "AQAAAAIAAYagAAAAEGleKu/BKdcmmRjckss9E3xDlK+o9YmCHg7LRPfzJKnI24AWx28H0D3pNnsZVDKewg==", "11d5c6ef-fd32-4da5-9707-763b4f5de02e" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "9e224968-33e4-4652-b7b7-8574d048cdb9",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f37dda2d-674a-40bb-b149-c7155a41c5cd", new DateTime(2025, 3, 27, 6, 38, 4, 241, DateTimeKind.Utc).AddTicks(4086), "AQAAAAIAAYagAAAAEAZs/LL+uNPBKTFKzSy8ntoLEm1BKHDH9UQ+/bkXQTqtWCy3e3/A2KvWtm+2Aeih0A==", "0608c137-32b8-4c7a-b097-b31ae9785a27" });

            migrationBuilder.InsertData(
                table: "Wallets",
                columns: new[] { "Id", "Balance", "CreatedAt", "CreatedBy", "IsDeleted", "LastModified", "LastModifiedBy", "UserId" },
                values: new object[,]
                {
                    { new Guid("c9e16434-2373-43ba-bf87-162c60e10995"), 1000m, new DateTime(2025, 3, 27, 13, 38, 4, 243, DateTimeKind.Local).AddTicks(2849), null, false, null, null, "8e445865-a24d-4543-a6c6-9443d048cdb9" },
                    { new Guid("fecd44ee-dadc-41f5-9d9d-75337b85ed52"), 500m, new DateTime(2025, 3, 27, 13, 38, 4, 243, DateTimeKind.Local).AddTicks(2953), null, false, null, null, "9e224968-33e4-4652-b7b7-8574d048cdb9" }
                });
        }
    }
}
