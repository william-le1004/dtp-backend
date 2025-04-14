using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Wallets",
                keyColumn: "Id",
                keyValue: new Guid("7ab5c9b3-3b5f-4a5a-b609-55f965cd76b0"));

            migrationBuilder.DeleteData(
                table: "Wallets",
                keyColumn: "Id",
                keyValue: new Guid("b55a8915-0fef-4df5-999d-3cafb9785108"));

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Companies",
                type: "longtext",
                nullable: false);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1286c068-fcf8-4d1a-a41c-f4344befba0c", new DateTime(2025, 4, 13, 15, 44, 33, 343, DateTimeKind.Utc).AddTicks(4003), "AQAAAAIAAYagAAAAEFQP/VtgYwZ17IaSdlsFpQnhzPsjlTQb/cFvN4qGiadvg4cdeVpNsRM/VbwRiS2FQA==", "3e10743f-a753-49d8-a57d-cad62b54116b" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "9e224968-33e4-4652-b7b7-8574d048cdb9",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1d061681-2d87-4688-9e15-d6b2d1ac09c6", new DateTime(2025, 4, 13, 15, 44, 33, 401, DateTimeKind.Utc).AddTicks(7941), "AQAAAAIAAYagAAAAEGp7dqr94445esT/++ir4cfme+KpjYTIo7Td0flhTeb+L59BMvA2YdmW0ijBSwQQrg==", "0ed63b42-366e-47bc-a634-5a491bc22f45" });

            migrationBuilder.InsertData(
                table: "Wallets",
                columns: new[] { "Id", "Balance", "CreatedAt", "CreatedBy", "IsDeleted", "LastModified", "LastModifiedBy", "UserId" },
                values: new object[,]
                {
                    { new Guid("17b632b7-082a-4b47-8a17-0b87c5f91291"), 500m, new DateTime(2025, 4, 13, 22, 44, 33, 404, DateTimeKind.Local).AddTicks(2019), null, false, null, null, "9e224968-33e4-4652-b7b7-8574d048cdb9" },
                    { new Guid("8368db05-5fb6-4be4-b7e1-acb210eda9fc"), 1000m, new DateTime(2025, 4, 13, 22, 44, 33, 404, DateTimeKind.Local).AddTicks(1957), null, false, null, null, "8e445865-a24d-4543-a6c6-9443d048cdb9" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Wallets",
                keyColumn: "Id",
                keyValue: new Guid("17b632b7-082a-4b47-8a17-0b87c5f91291"));

            migrationBuilder.DeleteData(
                table: "Wallets",
                keyColumn: "Id",
                keyValue: new Guid("8368db05-5fb6-4be4-b7e1-acb210eda9fc"));

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Companies");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "454b2235-a233-4a47-8dac-3cc9d0dcee78", new DateTime(2025, 4, 11, 14, 3, 12, 429, DateTimeKind.Utc).AddTicks(6621), "AQAAAAIAAYagAAAAEEcg3u8BA2EjnFe4tXjji7EKAXH30ueF8Wca3lC3Eyt4wWs1a+yw6riFgDUEQJY+Bg==", "09dd41bb-ebc9-4be8-8f09-f12ae0644767" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "9e224968-33e4-4652-b7b7-8574d048cdb9",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d6a0cb43-d181-416d-8297-82dca3072acd", new DateTime(2025, 4, 11, 14, 3, 12, 478, DateTimeKind.Utc).AddTicks(3364), "AQAAAAIAAYagAAAAEFei5km+iv65RxGIGBNWLFFJ4AAlefxqcFY11jk0CyjbPg3AQhvl0Cu0Luw0nbE1lQ==", "83c8aed1-6171-4a33-a951-49e4f1842122" });

            migrationBuilder.InsertData(
                table: "Wallets",
                columns: new[] { "Id", "Balance", "CreatedAt", "CreatedBy", "IsDeleted", "LastModified", "LastModifiedBy", "UserId" },
                values: new object[,]
                {
                    { new Guid("7ab5c9b3-3b5f-4a5a-b609-55f965cd76b0"), 1000m, new DateTime(2025, 4, 11, 21, 3, 12, 482, DateTimeKind.Local).AddTicks(4513), null, false, null, null, "8e445865-a24d-4543-a6c6-9443d048cdb9" },
                    { new Guid("b55a8915-0fef-4df5-999d-3cafb9785108"), 500m, new DateTime(2025, 4, 11, 21, 3, 12, 482, DateTimeKind.Local).AddTicks(4579), null, false, null, null, "9e224968-33e4-4652-b7b7-8574d048cdb9" }
                });
        }
    }
}
