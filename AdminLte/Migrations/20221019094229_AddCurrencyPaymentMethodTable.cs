using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminLte.Migrations
{
    public partial class AddCurrencyPaymentMethodTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CurrencyPaymentMethods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CurrencyId = table.Column<int>(type: "int", nullable: false),
                    PaymentMethodId = table.Column<int>(type: "int", nullable: false),
                    ActivatedFor = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "deposit, withdrawal single, both or none"),
                    MethodData = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "input field's title and value like client_id, client_secret etc"),
                    ProcessingTime = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyPaymentMethods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CurrencyPaymentMethods_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CurrencyPaymentMethods_PaymentMethods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalTable: "PaymentMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyPaymentMethods_CurrencyId",
                table: "CurrencyPaymentMethods",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyPaymentMethods_PaymentMethodId",
                table: "CurrencyPaymentMethods",
                column: "PaymentMethodId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CurrencyPaymentMethods");
        }
    }
}
