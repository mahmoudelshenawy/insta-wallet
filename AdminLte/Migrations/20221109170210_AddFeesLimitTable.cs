using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminLte.Migrations
{
    public partial class AddFeesLimitTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        { 

            migrationBuilder.CreateTable(
                name: "FeesLimits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CurrencyId = table.Column<int>(type: "int", nullable: false),
                    TransactionTypeId = table.Column<int>(type: "int", nullable: true),
                    PaymentMethodId = table.Column<int>(type: "int", nullable: true),
                    FixedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    PercentAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    MinLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 1m),
                    MaxLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProcessingTime = table.Column<int>(type: "int", nullable: false),
                    HasTransaction = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeesLimits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeesLimits_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FeesLimits_PaymentMethods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalTable: "PaymentMethods",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FeesLimits_TransactionTypes_TransactionTypeId",
                        column: x => x.TransactionTypeId,
                        principalTable: "TransactionTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FeesLimits_CurrencyId",
                table: "FeesLimits",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_FeesLimits_PaymentMethodId",
                table: "FeesLimits",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_FeesLimits_TransactionTypeId",
                table: "FeesLimits",
                column: "TransactionTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FeesLimits");

            migrationBuilder.DropColumn(
                name: "Feeable",
                table: "TransactionTypes");
        }
    }
}
