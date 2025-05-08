using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false),
                    Name = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: false),
                    Email = table.Column<string>(type: "longtext", nullable: false),
                    Phone = table.Column<string>(type: "longtext", nullable: false),
                    Address = table.Column<string>(type: "longtext", nullable: false),
                    TaxCode = table.Column<string>(type: "longtext", nullable: false),
                    CommissionRate = table.Column<double>(type: "double", nullable: false),
                    Licensed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Destinations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: false),
                    Latitude = table.Column<string>(type: "longtext", nullable: false),
                    Longitude = table.Column<string>(type: "longtext", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Destinations", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ImageUrls",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    RefId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Url = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageUrls", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SystemSetting",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    SettingCode = table.Column<string>(type: "longtext", nullable: false),
                    SettingKey = table.Column<string>(type: "longtext", nullable: false),
                    SettingCharValue = table.Column<string>(type: "longtext", nullable: false),
                    SettingIntegerValue = table.Column<string>(type: "longtext", nullable: false),
                    SettingDoubleValue = table.Column<string>(type: "longtext", nullable: false),
                    SettingDecimalValue = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSetting", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Voucher",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Code = table.Column<string>(type: "longtext", nullable: false),
                    MaxDiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Percent = table.Column<double>(type: "double", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Voucher", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<string>(type: "varchar(255)", nullable: false),
                    ClaimType = table.Column<string>(type: "longtext", nullable: true),
                    ClaimValue = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "longtext", nullable: true),
                    Name = table.Column<string>(type: "longtext", nullable: false),
                    Address = table.Column<string>(type: "longtext", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    OtpKey = table.Column<string>(type: "longtext", nullable: true),
                    CompanyId = table.Column<Guid>(type: "char(36)", nullable: true),
                    SecureToken = table.Column<string>(type: "longtext", nullable: true),
                    FcmToken = table.Column<string>(type: "longtext", nullable: true),
                    UserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PasswordHash = table.Column<string>(type: "longtext", nullable: true),
                    SecurityStamp = table.Column<string>(type: "longtext", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true),
                    PhoneNumber = table.Column<string>(type: "longtext", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Tours",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Title = table.Column<string>(type: "longtext", nullable: false),
                    CompanyId = table.Column<Guid>(type: "char(36)", nullable: true),
                    CategoryId = table.Column<Guid>(type: "char(36)", nullable: true),
                    Description = table.Column<string>(type: "longtext", nullable: true),
                    About = table.Column<string>(type: "longtext", nullable: true),
                    Include = table.Column<string>(type: "longtext", nullable: true),
                    Pickinfor = table.Column<string>(type: "longtext", nullable: true),
                    Code = table.Column<string>(type: "longtext", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tours", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tours_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tours_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false),
                    ClaimType = table.Column<string>(type: "longtext", nullable: true),
                    ClaimValue = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "varchar(255)", nullable: false),
                    ProviderKey = table.Column<string>(type: "varchar(255)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "longtext", nullable: true),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false),
                    RoleId = table.Column<string>(type: "varchar(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false),
                    LoginProvider = table.Column<string>(type: "varchar(255)", nullable: false),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false),
                    Value = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Baskets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Baskets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Baskets_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ExternalTransaction",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false),
                    ExternalTransactionCode = table.Column<string>(type: "longtext", nullable: false),
                    BankAccountNumber = table.Column<string>(type: "longtext", nullable: false),
                    BankAccount = table.Column<string>(type: "longtext", nullable: false),
                    BankName = table.Column<string>(type: "longtext", nullable: false),
                    TransactionCode = table.Column<string>(type: "longtext", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalTransaction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExternalTransaction_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Wallets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wallets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wallets_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TicketTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    DefaultNetCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MinimumPurchaseQuantity = table.Column<int>(type: "int", nullable: false),
                    TicketKind = table.Column<int>(type: "int", nullable: false),
                    TourId = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketTypes_Tours_TourId",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TourDestinations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    TourId = table.Column<Guid>(type: "char(36)", nullable: false),
                    DestinationId = table.Column<Guid>(type: "char(36)", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: true),
                    SortOrderByDate = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourDestinations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TourDestinations_Destinations_DestinationId",
                        column: x => x.DestinationId,
                        principalTable: "Destinations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TourDestinations_Tours_TourId",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TourSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    TourId = table.Column<Guid>(type: "char(36)", nullable: false),
                    OpenDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CloseDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    PriceChangeRate = table.Column<double>(type: "double", nullable: false),
                    Remark = table.Column<string>(type: "longtext", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TourSchedules_Tours_TourId",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    WalletId = table.Column<Guid>(type: "char(36)", nullable: false),
                    TransactionCode = table.Column<string>(type: "longtext", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: true),
                    RefTransactionCode = table.Column<string>(type: "longtext", nullable: true),
                    AfterTransactionBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_Wallets_WalletId",
                        column: x => x.WalletId,
                        principalTable: "Wallets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DestinationActivities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: false),
                    TourDestinationId = table.Column<Guid>(type: "char(36)", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DestinationActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DestinationActivities_TourDestinations_TourDestinationId",
                        column: x => x.TourDestinationId,
                        principalTable: "TourDestinations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Feedbacks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    TourScheduleId = table.Column<Guid>(type: "char(36)", nullable: false),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedbacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Feedbacks_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Feedbacks_TourSchedules_TourScheduleId",
                        column: x => x.TourScheduleId,
                        principalTable: "TourSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TourBasketItems",
                columns: table => new
                {
                    BasketId = table.Column<Guid>(type: "char(36)", nullable: false),
                    TourScheduleId = table.Column<Guid>(type: "char(36)", nullable: false),
                    TicketTypeId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourBasketItems", x => new { x.TourScheduleId, x.BasketId, x.TicketTypeId });
                    table.ForeignKey(
                        name: "FK_TourBasketItems_Baskets_BasketId",
                        column: x => x.BasketId,
                        principalTable: "Baskets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TourBasketItems_TourSchedules_TourScheduleId",
                        column: x => x.TourScheduleId,
                        principalTable: "TourSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TourBookings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false),
                    Code = table.Column<string>(type: "longtext", nullable: false),
                    RefCode = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: false),
                    PhoneNumber = table.Column<string>(type: "longtext", nullable: false),
                    Email = table.Column<string>(type: "longtext", nullable: false),
                    TourScheduleId = table.Column<Guid>(type: "char(36)", nullable: false),
                    VoucherCode = table.Column<string>(type: "longtext", nullable: true),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Remark = table.Column<string>(type: "longtext", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourBookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TourBookings_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TourBookings_TourSchedules_TourScheduleId",
                        column: x => x.TourScheduleId,
                        principalTable: "TourSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TourScheduleTicket",
                columns: table => new
                {
                    TicketTypeId = table.Column<Guid>(type: "char(36)", nullable: false),
                    TourScheduleId = table.Column<Guid>(type: "char(36)", nullable: false),
                    NetCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourScheduleTicket", x => new { x.TourScheduleId, x.TicketTypeId });
                    table.ForeignKey(
                        name: "FK_TourScheduleTicket_TicketTypes_TicketTypeId",
                        column: x => x.TicketTypeId,
                        principalTable: "TicketTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TourScheduleTicket_TourSchedules_TourScheduleId",
                        column: x => x.TourScheduleId,
                        principalTable: "TourSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    BookingId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Method = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RefExternalTransactionCode = table.Column<string>(type: "longtext", nullable: true),
                    NetCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RefTransactionCode = table.Column<string>(type: "longtext", nullable: true),
                    PaymentLinkId = table.Column<string>(type: "longtext", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_TourBookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "TourBookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Ratings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    TourId = table.Column<Guid>(type: "char(36)", nullable: false),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false),
                    Star = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "longtext", nullable: false),
                    TourBookingId = table.Column<Guid>(type: "char(36)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ratings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ratings_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ratings_TourBookings_TourBookingId",
                        column: x => x.TourBookingId,
                        principalTable: "TourBookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ratings_Tours_TourId",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Code = table.Column<string>(type: "longtext", nullable: false),
                    TicketTypeId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    GrossCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TourBookingId = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tickets_TicketTypes_TicketTypeId",
                        column: x => x.TicketTypeId,
                        principalTable: "TicketTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tickets_TourBookings_TourBookingId",
                        column: x => x.TourBookingId,
                        principalTable: "TourBookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "000b8fc6-be6c-44ad-9d48-9e7dad7c1221", null, "Admin", "ADMIN" },
                    { "096c07cd-26b0-4ee2-8083-efb870889599", null, "Tourist", "TOURIST" },
                    { "d856e718-a5a2-4dea-80e5-10e4dfc995e2", null, "Manager", "MANAGER" },
                    { "f8efc705-4d2f-40b8-9959-013505b24f86", null, "Operator", "OPERATOR" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Address", "CompanyId", "ConcurrencyStamp", "CreatedAt", "CreatedBy", "Email", "EmailConfirmed", "FcmToken", "IsActive", "LastModified", "LastModifiedBy", "LockoutEnabled", "LockoutEnd", "Name", "NormalizedEmail", "NormalizedUserName", "OtpKey", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecureToken", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "8e445865-a24d-4543-a6c6-9443d048cdb9", 0, "123 Admin St", null, "1fa01e39-6dbf-4eba-88aa-0a176b2fbda4", new DateTime(2025, 5, 8, 21, 27, 52, 348, DateTimeKind.Local).AddTicks(5835), "System", "dtpAdmin@gmail.com", true, null, true, null, null, false, null, "Admin User", "DTPADMIN@GMAIL.COM", "DTPADMIN", null, "AQAAAAIAAYagAAAAEJe9w5HTSuux0JOZ8r9DbdgQKUFwe32qi+scw+517kJiBfYbSaaBvHytIRgo/ctKZw==", "1234567890", false, null, "c313a20b-d692-412b-a76e-42cab9428ca0", false, "dtpAdmin" },
                    { "9e224968-33e4-4652-b7b7-8574d048cdb9", 0, "456 Operator Rd", null, "a0015870-a6d5-4997-bdf4-2ff3001f93b1", new DateTime(2025, 5, 8, 21, 27, 52, 406, DateTimeKind.Local).AddTicks(6511), "System", "operator@gmail.com", true, null, true, null, null, false, null, "Operator User", "OPERATOR@GMAIL.COM", "OPERATOR", null, "AQAAAAIAAYagAAAAEHQZa0aHGyX23HRbmecFwDYZ2XZe3tKKir7wWD2uBjY4F0S3MF1QJQoVxmdPF7D6oA==", "0987654321", false, null, "41b69344-1c17-4a77-bcfc-b70dd76d8cc8", false, "operator" }
                });

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
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "000b8fc6-be6c-44ad-9d48-9e7dad7c1221", "8e445865-a24d-4543-a6c6-9443d048cdb9" },
                    { "f8efc705-4d2f-40b8-9959-013505b24f86", "9e224968-33e4-4652-b7b7-8574d048cdb9" }
                });

            migrationBuilder.InsertData(
                table: "Wallets",
                columns: new[] { "Id", "Balance", "CreatedAt", "CreatedBy", "IsDeleted", "LastModified", "LastModifiedBy", "UserId" },
                values: new object[,]
                {
                    { new Guid("14358248-735f-4505-b913-cf3aad11c468"), 1000m, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, null, "8e445865-a24d-4543-a6c6-9443d048cdb9" },
                    { new Guid("b5fd5b31-ff02-4258-bcd8-a953fa6d8670"), 500m, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, null, "9e224968-33e4-4652-b7b7-8574d048cdb9" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CompanyId",
                table: "AspNetUsers",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Baskets_UserId",
                table: "Baskets",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DestinationActivities_TourDestinationId",
                table: "DestinationActivities",
                column: "TourDestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalTransaction_UserId",
                table: "ExternalTransaction",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_TourScheduleId",
                table: "Feedbacks",
                column: "TourScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_UserId",
                table: "Feedbacks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_BookingId",
                table: "Payments",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_TourBookingId",
                table: "Ratings",
                column: "TourBookingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_TourId",
                table: "Ratings",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_UserId",
                table: "Ratings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_TicketTypeId",
                table: "Tickets",
                column: "TicketTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_TourBookingId",
                table: "Tickets",
                column: "TourBookingId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketTypes_TourId",
                table: "TicketTypes",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_TourBasketItems_BasketId",
                table: "TourBasketItems",
                column: "BasketId");

            migrationBuilder.CreateIndex(
                name: "IX_TourBookings_TourScheduleId",
                table: "TourBookings",
                column: "TourScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_TourBookings_UserId",
                table: "TourBookings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TourDestinations_DestinationId",
                table: "TourDestinations",
                column: "DestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_TourDestinations_TourId",
                table: "TourDestinations",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_Tours_CategoryId",
                table: "Tours",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Tours_CompanyId",
                table: "Tours",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_TourSchedules_TourId",
                table: "TourSchedules",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_TourScheduleTicket_TicketTypeId",
                table: "TourScheduleTicket",
                column: "TicketTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_WalletId",
                table: "Transactions",
                column: "WalletId");

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_UserId",
                table: "Wallets",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "DestinationActivities");

            migrationBuilder.DropTable(
                name: "ExternalTransaction");

            migrationBuilder.DropTable(
                name: "Feedbacks");

            migrationBuilder.DropTable(
                name: "ImageUrls");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Ratings");

            migrationBuilder.DropTable(
                name: "SystemSetting");

            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropTable(
                name: "TourBasketItems");

            migrationBuilder.DropTable(
                name: "TourScheduleTicket");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Voucher");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "TourDestinations");

            migrationBuilder.DropTable(
                name: "TourBookings");

            migrationBuilder.DropTable(
                name: "Baskets");

            migrationBuilder.DropTable(
                name: "TicketTypes");

            migrationBuilder.DropTable(
                name: "Wallets");

            migrationBuilder.DropTable(
                name: "Destinations");

            migrationBuilder.DropTable(
                name: "TourSchedules");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Tours");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Companies");
        }
    }
}
