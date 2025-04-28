using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Changelongtexttovarchar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Wallets",
                keyColumn: "Id",
                keyValue: new Guid("8f0cebfb-ea05-4eb0-9b8a-a3b80cb4a7cd"));

            migrationBuilder.DeleteData(
                table: "Wallets",
                keyColumn: "Id",
                keyValue: new Guid("a1f0501a-f18b-4318-96de-833a01630168"));

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "TourBookings",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "9acdd1fa-a44d-4079-b1a2-75972f6001a1", new DateTime(2025, 4, 28, 16, 58, 44, 867, DateTimeKind.Utc).AddTicks(1714), "AQAAAAIAAYagAAAAEJNFCYA6jnPjSDVabXohq08UIAwClszcWOJjvuuAwhYQ9ohOXF9aGKce6hL02sFNpQ==", "0721a60b-f144-4716-aa08-64c897ad1d22" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "9e224968-33e4-4652-b7b7-8574d048cdb9",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "07db7f11-6676-4709-86f0-10a4bfa46992", new DateTime(2025, 4, 28, 16, 58, 44, 926, DateTimeKind.Utc).AddTicks(162), "AQAAAAIAAYagAAAAEGtb199xTuDmmfa7mEKy0pSMWmnF44N+fV3N0uRDQID0J5/4TRaCveNE56t4NDm3+Q==", "fda16b2a-97c1-4e45-bd22-11a672abeaaa" });

            migrationBuilder.InsertData(
                table: "Wallets",
                columns: new[] { "Id", "Balance", "CreatedAt", "CreatedBy", "IsDeleted", "LastModified", "LastModifiedBy", "UserId" },
                values: new object[,]
                {
                    { new Guid("293c0015-6b8a-4a61-b5cb-85dae14ccbb8"), 1000m, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, null, "8e445865-a24d-4543-a6c6-9443d048cdb9" },
                    { new Guid("6d9da11b-d8c5-418d-a99d-6d4844f11390"), 500m, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, null, "9e224968-33e4-4652-b7b7-8574d048cdb9" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_TourBookings_UserId",
                table: "TourBookings",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TourBookings_AspNetUsers_UserId",
                table: "TourBookings",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TourBookings_AspNetUsers_UserId",
                table: "TourBookings");

            migrationBuilder.DropIndex(
                name: "IX_TourBookings_UserId",
                table: "TourBookings");

            migrationBuilder.DeleteData(
                table: "Wallets",
                keyColumn: "Id",
                keyValue: new Guid("293c0015-6b8a-4a61-b5cb-85dae14ccbb8"));

            migrationBuilder.DeleteData(
                table: "Wallets",
                keyColumn: "Id",
                keyValue: new Guid("6d9da11b-d8c5-418d-a99d-6d4844f11390"));

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "TourBookings",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "bee3246f-3466-4af1-9085-fe6384c7abf8", new DateTime(2025, 4, 22, 14, 10, 22, 450, DateTimeKind.Utc).AddTicks(8892), "AQAAAAIAAYagAAAAEGtvLrxlIQeZrH1cReK4E4JCZzuQZhr5bC/Em0bOmTq1fi3dLjV2bPRTZnP26r135A==", "63007789-0888-4368-bef9-1244fdf6db57" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "9e224968-33e4-4652-b7b7-8574d048cdb9",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "dfcbe297-a62d-4e96-a42d-d9abb3a5aa8a", new DateTime(2025, 4, 22, 14, 10, 22, 499, DateTimeKind.Utc).AddTicks(6355), "AQAAAAIAAYagAAAAEPLWIRcDSIKyAiVcqi5Qdp8Oqnyf3fC/zSHBWx8P+39cgEktNYHp9Hk4Ehdg273gJw==", "0a50ec29-f5af-452f-ad60-177ef411a6ff" });

            migrationBuilder.InsertData(
                table: "Wallets",
                columns: new[] { "Id", "Balance", "CreatedAt", "CreatedBy", "IsDeleted", "LastModified", "LastModifiedBy", "UserId" },
                values: new object[,]
                {
                    { new Guid("8f0cebfb-ea05-4eb0-9b8a-a3b80cb4a7cd"), 500m, new DateTime(2025, 4, 22, 21, 10, 22, 507, DateTimeKind.Local).AddTicks(9130), null, false, null, null, "9e224968-33e4-4652-b7b7-8574d048cdb9" },
                    { new Guid("a1f0501a-f18b-4318-96de-833a01630168"), 1000m, new DateTime(2025, 4, 22, 21, 10, 22, 507, DateTimeKind.Local).AddTicks(9079), null, false, null, null, "8e445865-a24d-4543-a6c6-9443d048cdb9" }
                });
        }
    }
}
