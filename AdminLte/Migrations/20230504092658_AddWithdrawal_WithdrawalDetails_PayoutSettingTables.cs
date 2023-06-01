using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminLte.Migrations
{
    public partial class AddWithdrawal_WithdrawalDetails_PayoutSettingTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PayoutSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentMethodId = table.Column<int>(type: "int", nullable: false),
                    BankSetting = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CashSetting = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WalletSetting = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaypalSetting = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PayoneerSetting = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayoutSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayoutSettings_PaymentMethods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalTable: "PaymentMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Withdrawals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PayoutSettingId = table.Column<int>(type: "int", nullable: true),
                    CurrencyId = table.Column<int>(type: "int", nullable: false),
                    PaymentMethodId = table.Column<int>(type: "int", nullable: false),
                    Uuid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FixedFeeAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0.00m),
                    PercentFeeAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalFees = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Withdrawals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Withdrawals_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Withdrawals_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Withdrawals_PaymentMethods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalTable: "PaymentMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Withdrawals_PayoutSettings_PayoutSettingId",
                        column: x => x.PayoutSettingId,
                        principalTable: "PayoutSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "WithdrawalDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WithdrawalId = table.Column<int>(type: "int", nullable: false),
                    BankSetting = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CashSetting = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WalletSetting = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaypalSetting = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PayoneerSetting = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WithdrawalDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WithdrawalDetails_Withdrawals_WithdrawalId",
                        column: x => x.WithdrawalId,
                        principalTable: "Withdrawals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PayoutSettings_PaymentMethodId",
                table: "PayoutSettings",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_WithdrawalDetails_WithdrawalId",
                table: "WithdrawalDetails",
                column: "WithdrawalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Withdrawals_CurrencyId",
                table: "Withdrawals",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Withdrawals_PaymentMethodId",
                table: "Withdrawals",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_Withdrawals_PayoutSettingId",
                table: "Withdrawals",
                column: "PayoutSettingId");

            migrationBuilder.CreateIndex(
                name: "IX_Withdrawals_UserId",
                table: "Withdrawals",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WithdrawalDetails");

            migrationBuilder.DropTable(
                name: "Withdrawals");

            migrationBuilder.DropTable(
                name: "PayoutSettings");
        }
    }
}
