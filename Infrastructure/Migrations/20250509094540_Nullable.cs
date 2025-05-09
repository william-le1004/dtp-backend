using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Nullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("07da38e8-23f6-47bd-8068-0d5d979c9410"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("a43bdf43-8b5c-4e5a-a5f2-b4ca8ae9367e"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("bb6f624a-1c1e-4bc7-9ab1-b840c9adbf70"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("dbb44662-9e09-41fe-8e67-eefade9004d4"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("f9ac5bc5-7b0d-4252-b7c6-40b8da0d9ca4"));

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: new Guid("05c05958-13b0-4f94-be71-cf5ae1c56540"));

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: new Guid("5a476a66-36c4-4c5d-af85-2177d1638e49"));

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: new Guid("db155d3b-db8f-4c20-bdc9-afaccf4df307"));

            migrationBuilder.DeleteData(
                table: "Destinations",
                keyColumn: "Id",
                keyValue: new Guid("3f71194b-0653-433f-b571-211ff8d1c3eb"));

            migrationBuilder.DeleteData(
                table: "Destinations",
                keyColumn: "Id",
                keyValue: new Guid("5518f659-a253-4a78-afc0-becdf72f227a"));

            migrationBuilder.DeleteData(
                table: "Destinations",
                keyColumn: "Id",
                keyValue: new Guid("652760bc-6567-4175-9260-961654cfff88"));

            migrationBuilder.DeleteData(
                table: "Destinations",
                keyColumn: "Id",
                keyValue: new Guid("77d58d4c-e75d-4fac-8711-532761586db1"));

            migrationBuilder.DeleteData(
                table: "Destinations",
                keyColumn: "Id",
                keyValue: new Guid("ae2652fe-39eb-4b7f-818d-4f7a4ecdfe2e"));

            migrationBuilder.DeleteData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("16cb777b-9e88-4416-946e-c49ef6753047"));

            migrationBuilder.DeleteData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("553a8605-e1b9-4f0d-857f-942e08e279f3"));

            migrationBuilder.DeleteData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("e3cbd5b5-22e8-4e83-a79d-88b91b18d9e2"));

            migrationBuilder.DeleteData(
                table: "Wallets",
                keyColumn: "Id",
                keyValue: new Guid("14358248-735f-4505-b913-cf3aad11c468"));

            migrationBuilder.DeleteData(
                table: "Wallets",
                keyColumn: "Id",
                keyValue: new Guid("b5fd5b31-ff02-4258-bcd8-a953fa6d8670"));

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "StartTime",
                table: "DestinationActivities",
                type: "time(6)",
                nullable: true,
                oldClrType: typeof(TimeSpan),
                oldType: "time(6)");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "EndTime",
                table: "DestinationActivities",
                type: "time(6)",
                nullable: true,
                oldClrType: typeof(TimeSpan),
                oldType: "time(6)");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8e8c85ee-be02-45f5-82ef-1f9e4857b863", new DateTime(2025, 5, 9, 16, 45, 38, 866, DateTimeKind.Local).AddTicks(9312), "AQAAAAIAAYagAAAAEEfhljpVt9kWAjCCeLHF0wuvg34ZAtSezSwTE6P92FZZAlfVF0F7U2gIlYMlVV3DGA==", "ee3992f5-8b44-4465-9c3e-8502f17bb937" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "9e224968-33e4-4652-b7b7-8574d048cdb9",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "be5764e7-dfd7-4921-a723-0beb71d2979c", new DateTime(2025, 5, 9, 16, 45, 38, 941, DateTimeKind.Local).AddTicks(7274), "AQAAAAIAAYagAAAAENgLN/UUsyV3drsIPHdJnfWjqXnPaMeJhN/9Anl3ifGmwe7u6Xh3EDytZE8YKtD4Zw==", "a0dbc3f8-a492-49f9-83a0-37e9ca258be1" });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "IsDeleted", "LastModified", "LastModifiedBy", "Name" },
                values: new object[,]
                {
                    { new Guid("1e5ce424-8d8c-42b4-90a8-3905036bc988"), new DateTime(2025, 5, 9, 16, 45, 38, 766, DateTimeKind.Local).AddTicks(6315), "admin", false, new DateTime(2025, 5, 9, 16, 45, 38, 766, DateTimeKind.Local).AddTicks(6315), "admin", "Tour nửa ngày" },
                    { new Guid("382f523a-e5b0-4526-920c-30f1d94e3b3c"), new DateTime(2025, 5, 9, 16, 45, 38, 766, DateTimeKind.Local).AddTicks(6315), "admin", false, new DateTime(2025, 5, 9, 16, 45, 38, 766, DateTimeKind.Local).AddTicks(6315), "admin", "Tour 7 ngày" },
                    { new Guid("3ad630d7-bb05-41a3-8557-594c7e815e7c"), new DateTime(2025, 5, 9, 16, 45, 38, 766, DateTimeKind.Local).AddTicks(6315), "admin", false, new DateTime(2025, 5, 9, 16, 45, 38, 766, DateTimeKind.Local).AddTicks(6315), "admin", "Tour 3 ngày" },
                    { new Guid("c2898d8a-8d9e-405e-a9f2-95194d94f5e3"), new DateTime(2025, 5, 9, 16, 45, 38, 766, DateTimeKind.Local).AddTicks(6315), "admin", false, new DateTime(2025, 5, 9, 16, 45, 38, 766, DateTimeKind.Local).AddTicks(6315), "admin", "Tour trong ngày" },
                    { new Guid("ec35877b-d176-4103-b2b1-6f3e6c765f8a"), new DateTime(2025, 5, 9, 16, 45, 38, 766, DateTimeKind.Local).AddTicks(6315), "admin", false, new DateTime(2025, 5, 9, 16, 45, 38, 766, DateTimeKind.Local).AddTicks(6315), "admin", "Tour 1 ngày" }
                });

            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Id", "Address", "CommissionRate", "CreatedAt", "CreatedBy", "Email", "IsDeleted", "LastModified", "LastModifiedBy", "Licensed", "Name", "Phone", "TaxCode" },
                values: new object[,]
                {
                    { new Guid("22c70bf6-a2f5-4f9d-8334-f6c7a3f50e2b"), "TP. Hồ Chí Minh", 15.0, new DateTime(2025, 5, 9, 16, 45, 38, 768, DateTimeKind.Local).AddTicks(6138), "System", "sgtravel@example.com", false, new DateTime(2025, 5, 9, 16, 45, 38, 768, DateTimeKind.Local).AddTicks(6138), "System", false, "Du lịch Sài Gòn Travel", "0909222333", "987654321" },
                    { new Guid("6fcf2dc8-01a0-4c92-a5cd-61456923acdb"), "Hà Nội", 12.0, new DateTime(2025, 5, 9, 16, 45, 38, 768, DateTimeKind.Local).AddTicks(6138), "System", "xyz@example.com", false, new DateTime(2025, 5, 9, 16, 45, 38, 768, DateTimeKind.Local).AddTicks(6138), "System", false, "Công ty Du lịch XYZ", "0988999111", "123456789" },
                    { new Guid("8f4ec603-ac7e-453a-ba98-20a91c060ff8"), "Đà Nẵng", 10.0, new DateTime(2025, 5, 9, 16, 45, 38, 768, DateTimeKind.Local).AddTicks(6138), "System", "info@khamphavn.vn", false, new DateTime(2025, 5, 9, 16, 45, 38, 768, DateTimeKind.Local).AddTicks(6138), "System", false, "Khám phá Việt Nam", "0912345678", "1122334455" }
                });

            migrationBuilder.InsertData(
                table: "Destinations",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "IsDeleted", "LastModified", "LastModifiedBy", "Latitude", "Longitude", "Name" },
                values: new object[,]
                {
                    { new Guid("1403e340-1e15-4ff7-91c3-fbfdc2491041"), new DateTime(2025, 5, 9, 16, 45, 38, 769, DateTimeKind.Local).AddTicks(5799), "admin", false, new DateTime(2025, 5, 9, 16, 45, 38, 769, DateTimeKind.Local).AddTicks(5799), "admin", "13.3580", "109.2065", "Bãi Xép" },
                    { new Guid("3da5f873-9c1a-4bf5-b70d-2f2cb828935b"), new DateTime(2025, 5, 9, 16, 45, 38, 769, DateTimeKind.Local).AddTicks(5799), "admin", false, new DateTime(2025, 5, 9, 16, 45, 38, 769, DateTimeKind.Local).AddTicks(5799), "admin", "13.3457", "109.1457", "Eo Gió" },
                    { new Guid("58c73d7a-be83-44f6-a6bc-169ff5de0c48"), new DateTime(2025, 5, 9, 16, 45, 38, 769, DateTimeKind.Local).AddTicks(5799), "admin", false, new DateTime(2025, 5, 9, 16, 45, 38, 769, DateTimeKind.Local).AddTicks(5799), "admin", "13.4644", "109.1999", "Cù Lao Xanh" },
                    { new Guid("8fc72f8b-957f-4a05-af89-f9e56f1e3a2b"), new DateTime(2025, 5, 9, 16, 45, 38, 769, DateTimeKind.Local).AddTicks(5799), "admin", false, new DateTime(2025, 5, 9, 16, 45, 38, 769, DateTimeKind.Local).AddTicks(5799), "admin", "13.3490", "109.1888", "Tháp Đôi" },
                    { new Guid("f640ef66-8171-40cf-85e7-6c456d747df1"), new DateTime(2025, 5, 9, 16, 45, 38, 769, DateTimeKind.Local).AddTicks(5799), "admin", false, new DateTime(2025, 5, 9, 16, 45, 38, 769, DateTimeKind.Local).AddTicks(5799), "admin", "13.3456", "109.1456", "Kỳ Co" }
                });

            migrationBuilder.InsertData(
                table: "Voucher",
                columns: new[] { "Id", "Code", "CreatedAt", "CreatedBy", "Description", "ExpiryDate", "IsDeleted", "LastModified", "LastModifiedBy", "MaxDiscountAmount", "Percent", "Quantity" },
                values: new object[,]
                {
                    { new Guid("7b499f2b-0264-4125-9a4c-bab4f3c9217e"), "8OMPKJGXE3", new DateTime(2025, 5, 9, 16, 45, 38, 944, DateTimeKind.Local).AddTicks(4720), "System", "Giảm 50% tối đa 200K", new DateTime(2025, 6, 30, 23, 59, 59, 0, DateTimeKind.Unspecified), false, null, null, 200000m, 0.5, 20 },
                    { new Guid("e79e7b8d-86a4-4de8-a42e-360ae896ce32"), "8OMPKJGX2F", new DateTime(2025, 5, 9, 16, 45, 38, 944, DateTimeKind.Local).AddTicks(4700), "System", "Giảm 20% tối đa 150K", new DateTime(2025, 12, 31, 23, 59, 59, 0, DateTimeKind.Unspecified), false, null, null, 150000m, 0.20000000000000001, 50 },
                    { new Guid("eaa6e0b4-bfd4-4d95-ac98-6d4d3dd226b2"), "8OMPKJGX2P", new DateTime(2025, 5, 9, 16, 45, 38, 944, DateTimeKind.Local).AddTicks(4672), "System", "Giảm 10% tối đa 100K", new DateTime(2025, 12, 31, 23, 59, 59, 0, DateTimeKind.Unspecified), false, null, null, 100000m, 0.10000000000000001, 100 }
                });

            migrationBuilder.InsertData(
                table: "Wallets",
                columns: new[] { "Id", "Balance", "CreatedAt", "CreatedBy", "IsDeleted", "LastModified", "LastModifiedBy", "UserId" },
                values: new object[,]
                {
                    { new Guid("7eea2a9e-dfff-44ff-b8b9-592f1c796de8"), 500m, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, null, "9e224968-33e4-4652-b7b7-8574d048cdb9" },
                    { new Guid("8e64b3c9-1d31-4c0a-abc3-9ec1cb550ba5"), 1000m, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, null, "8e445865-a24d-4543-a6c6-9443d048cdb9" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("1e5ce424-8d8c-42b4-90a8-3905036bc988"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("382f523a-e5b0-4526-920c-30f1d94e3b3c"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("3ad630d7-bb05-41a3-8557-594c7e815e7c"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("c2898d8a-8d9e-405e-a9f2-95194d94f5e3"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("ec35877b-d176-4103-b2b1-6f3e6c765f8a"));

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: new Guid("22c70bf6-a2f5-4f9d-8334-f6c7a3f50e2b"));

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: new Guid("6fcf2dc8-01a0-4c92-a5cd-61456923acdb"));

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: new Guid("8f4ec603-ac7e-453a-ba98-20a91c060ff8"));

            migrationBuilder.DeleteData(
                table: "Destinations",
                keyColumn: "Id",
                keyValue: new Guid("1403e340-1e15-4ff7-91c3-fbfdc2491041"));

            migrationBuilder.DeleteData(
                table: "Destinations",
                keyColumn: "Id",
                keyValue: new Guid("3da5f873-9c1a-4bf5-b70d-2f2cb828935b"));

            migrationBuilder.DeleteData(
                table: "Destinations",
                keyColumn: "Id",
                keyValue: new Guid("58c73d7a-be83-44f6-a6bc-169ff5de0c48"));

            migrationBuilder.DeleteData(
                table: "Destinations",
                keyColumn: "Id",
                keyValue: new Guid("8fc72f8b-957f-4a05-af89-f9e56f1e3a2b"));

            migrationBuilder.DeleteData(
                table: "Destinations",
                keyColumn: "Id",
                keyValue: new Guid("f640ef66-8171-40cf-85e7-6c456d747df1"));

            migrationBuilder.DeleteData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("7b499f2b-0264-4125-9a4c-bab4f3c9217e"));

            migrationBuilder.DeleteData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("e79e7b8d-86a4-4de8-a42e-360ae896ce32"));

            migrationBuilder.DeleteData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("eaa6e0b4-bfd4-4d95-ac98-6d4d3dd226b2"));

            migrationBuilder.DeleteData(
                table: "Wallets",
                keyColumn: "Id",
                keyValue: new Guid("7eea2a9e-dfff-44ff-b8b9-592f1c796de8"));

            migrationBuilder.DeleteData(
                table: "Wallets",
                keyColumn: "Id",
                keyValue: new Guid("8e64b3c9-1d31-4c0a-abc3-9ec1cb550ba5"));

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "StartTime",
                table: "DestinationActivities",
                type: "time(6)",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0),
                oldClrType: typeof(TimeSpan),
                oldType: "time(6)",
                oldNullable: true);

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "EndTime",
                table: "DestinationActivities",
                type: "time(6)",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0),
                oldClrType: typeof(TimeSpan),
                oldType: "time(6)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1fa01e39-6dbf-4eba-88aa-0a176b2fbda4", new DateTime(2025, 5, 8, 21, 27, 52, 348, DateTimeKind.Local).AddTicks(5835), "AQAAAAIAAYagAAAAEJe9w5HTSuux0JOZ8r9DbdgQKUFwe32qi+scw+517kJiBfYbSaaBvHytIRgo/ctKZw==", "c313a20b-d692-412b-a76e-42cab9428ca0" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "9e224968-33e4-4652-b7b7-8574d048cdb9",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a0015870-a6d5-4997-bdf4-2ff3001f93b1", new DateTime(2025, 5, 8, 21, 27, 52, 406, DateTimeKind.Local).AddTicks(6511), "AQAAAAIAAYagAAAAEHQZa0aHGyX23HRbmecFwDYZ2XZe3tKKir7wWD2uBjY4F0S3MF1QJQoVxmdPF7D6oA==", "41b69344-1c17-4a77-bcfc-b70dd76d8cc8" });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "IsDeleted", "LastModified", "LastModifiedBy", "Name" },
                values: new object[,]
                {
                    { new Guid("07da38e8-23f6-47bd-8068-0d5d979c9410"), new DateTime(2025, 5, 8, 21, 27, 52, 299, DateTimeKind.Local).AddTicks(6401), "admin", false, new DateTime(2025, 5, 8, 21, 27, 52, 299, DateTimeKind.Local).AddTicks(6401), "admin", "Tour 1 ngày" },
                    { new Guid("a43bdf43-8b5c-4e5a-a5f2-b4ca8ae9367e"), new DateTime(2025, 5, 8, 21, 27, 52, 299, DateTimeKind.Local).AddTicks(6401), "admin", false, new DateTime(2025, 5, 8, 21, 27, 52, 299, DateTimeKind.Local).AddTicks(6401), "admin", "Tour trong ngày" },
                    { new Guid("bb6f624a-1c1e-4bc7-9ab1-b840c9adbf70"), new DateTime(2025, 5, 8, 21, 27, 52, 299, DateTimeKind.Local).AddTicks(6401), "admin", false, new DateTime(2025, 5, 8, 21, 27, 52, 299, DateTimeKind.Local).AddTicks(6401), "admin", "Tour 7 ngày" },
                    { new Guid("dbb44662-9e09-41fe-8e67-eefade9004d4"), new DateTime(2025, 5, 8, 21, 27, 52, 299, DateTimeKind.Local).AddTicks(6401), "admin", false, new DateTime(2025, 5, 8, 21, 27, 52, 299, DateTimeKind.Local).AddTicks(6401), "admin", "Tour nửa ngày" },
                    { new Guid("f9ac5bc5-7b0d-4252-b7c6-40b8da0d9ca4"), new DateTime(2025, 5, 8, 21, 27, 52, 299, DateTimeKind.Local).AddTicks(6401), "admin", false, new DateTime(2025, 5, 8, 21, 27, 52, 299, DateTimeKind.Local).AddTicks(6401), "admin", "Tour 3 ngày" }
                });

            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Id", "Address", "CommissionRate", "CreatedAt", "CreatedBy", "Email", "IsDeleted", "LastModified", "LastModifiedBy", "Licensed", "Name", "Phone", "TaxCode" },
                values: new object[,]
                {
                    { new Guid("05c05958-13b0-4f94-be71-cf5ae1c56540"), "TP. Hồ Chí Minh", 15.0, new DateTime(2025, 5, 8, 21, 27, 52, 299, DateTimeKind.Local).AddTicks(7404), "System", "sgtravel@example.com", false, new DateTime(2025, 5, 8, 21, 27, 52, 299, DateTimeKind.Local).AddTicks(7404), "System", false, "Du lịch Sài Gòn Travel", "0909222333", "987654321" },
                    { new Guid("5a476a66-36c4-4c5d-af85-2177d1638e49"), "Đà Nẵng", 10.0, new DateTime(2025, 5, 8, 21, 27, 52, 299, DateTimeKind.Local).AddTicks(7404), "System", "info@khamphavn.vn", false, new DateTime(2025, 5, 8, 21, 27, 52, 299, DateTimeKind.Local).AddTicks(7404), "System", false, "Khám phá Việt Nam", "0912345678", "1122334455" },
                    { new Guid("db155d3b-db8f-4c20-bdc9-afaccf4df307"), "Hà Nội", 12.0, new DateTime(2025, 5, 8, 21, 27, 52, 299, DateTimeKind.Local).AddTicks(7404), "System", "xyz@example.com", false, new DateTime(2025, 5, 8, 21, 27, 52, 299, DateTimeKind.Local).AddTicks(7404), "System", false, "Công ty Du lịch XYZ", "0988999111", "123456789" }
                });

            migrationBuilder.InsertData(
                table: "Destinations",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "IsDeleted", "LastModified", "LastModifiedBy", "Latitude", "Longitude", "Name" },
                values: new object[,]
                {
                    { new Guid("3f71194b-0653-433f-b571-211ff8d1c3eb"), new DateTime(2025, 5, 8, 21, 27, 52, 299, DateTimeKind.Local).AddTicks(8138), "admin", false, new DateTime(2025, 5, 8, 21, 27, 52, 299, DateTimeKind.Local).AddTicks(8138), "admin", "13.3580", "109.2065", "Bãi Xép" },
                    { new Guid("5518f659-a253-4a78-afc0-becdf72f227a"), new DateTime(2025, 5, 8, 21, 27, 52, 299, DateTimeKind.Local).AddTicks(8138), "admin", false, new DateTime(2025, 5, 8, 21, 27, 52, 299, DateTimeKind.Local).AddTicks(8138), "admin", "13.3490", "109.1888", "Tháp Đôi" },
                    { new Guid("652760bc-6567-4175-9260-961654cfff88"), new DateTime(2025, 5, 8, 21, 27, 52, 299, DateTimeKind.Local).AddTicks(8138), "admin", false, new DateTime(2025, 5, 8, 21, 27, 52, 299, DateTimeKind.Local).AddTicks(8138), "admin", "13.3457", "109.1457", "Eo Gió" },
                    { new Guid("77d58d4c-e75d-4fac-8711-532761586db1"), new DateTime(2025, 5, 8, 21, 27, 52, 299, DateTimeKind.Local).AddTicks(8138), "admin", false, new DateTime(2025, 5, 8, 21, 27, 52, 299, DateTimeKind.Local).AddTicks(8138), "admin", "13.4644", "109.1999", "Cù Lao Xanh" },
                    { new Guid("ae2652fe-39eb-4b7f-818d-4f7a4ecdfe2e"), new DateTime(2025, 5, 8, 21, 27, 52, 299, DateTimeKind.Local).AddTicks(8138), "admin", false, new DateTime(2025, 5, 8, 21, 27, 52, 299, DateTimeKind.Local).AddTicks(8138), "admin", "13.3456", "109.1456", "Kỳ Co" }
                });

            migrationBuilder.InsertData(
                table: "Voucher",
                columns: new[] { "Id", "Code", "CreatedAt", "CreatedBy", "Description", "ExpiryDate", "IsDeleted", "LastModified", "LastModifiedBy", "MaxDiscountAmount", "Percent", "Quantity" },
                values: new object[,]
                {
                    { new Guid("16cb777b-9e88-4416-946e-c49ef6753047"), "8OMPKJGX2P", new DateTime(2025, 5, 8, 21, 27, 52, 407, DateTimeKind.Local).AddTicks(4398), "System", "Giảm 10% tối đa 100K", new DateTime(2025, 12, 31, 23, 59, 59, 0, DateTimeKind.Unspecified), false, null, null, 100000m, 0.10000000000000001, 100 },
                    { new Guid("553a8605-e1b9-4f0d-857f-942e08e279f3"), "8OMPKJGXE3", new DateTime(2025, 5, 8, 21, 27, 52, 407, DateTimeKind.Local).AddTicks(4424), "System", "Giảm 50% tối đa 200K", new DateTime(2025, 6, 30, 23, 59, 59, 0, DateTimeKind.Unspecified), false, null, null, 200000m, 0.5, 20 },
                    { new Guid("e3cbd5b5-22e8-4e83-a79d-88b91b18d9e2"), "8OMPKJGX2F", new DateTime(2025, 5, 8, 21, 27, 52, 407, DateTimeKind.Local).AddTicks(4416), "System", "Giảm 20% tối đa 150K", new DateTime(2025, 12, 31, 23, 59, 59, 0, DateTimeKind.Unspecified), false, null, null, 150000m, 0.20000000000000001, 50 }
                });

            migrationBuilder.InsertData(
                table: "Wallets",
                columns: new[] { "Id", "Balance", "CreatedAt", "CreatedBy", "IsDeleted", "LastModified", "LastModifiedBy", "UserId" },
                values: new object[,]
                {
                    { new Guid("14358248-735f-4505-b913-cf3aad11c468"), 1000m, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, null, "8e445865-a24d-4543-a6c6-9443d048cdb9" },
                    { new Guid("b5fd5b31-ff02-4258-bcd8-a953fa6d8670"), 500m, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, null, "9e224968-33e4-4652-b7b7-8574d048cdb9" }
                });
        }
    }
}
