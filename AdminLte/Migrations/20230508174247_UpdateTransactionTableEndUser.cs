using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminLte.Migrations
{
    public partial class UpdateTransactionTableEndUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Withdrawals_PayoutSettings_PayoutSettingId",
                table: "Withdrawals");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_EndUserId",
                table: "Transactions");

            migrationBuilder.AlterColumn<int>(
                name: "PayoutSettingId",
                table: "Withdrawals",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "EndUserId",
                table: "Transactions",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_EndUserId",
                table: "Transactions",
                column: "EndUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Withdrawals_PayoutSettings_PayoutSettingId",
                table: "Withdrawals",
                column: "PayoutSettingId",
                principalTable: "PayoutSettings",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Withdrawals_PayoutSettings_PayoutSettingId",
                table: "Withdrawals");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_EndUserId",
                table: "Transactions");

            migrationBuilder.AlterColumn<int>(
                name: "PayoutSettingId",
                table: "Withdrawals",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EndUserId",
                table: "Transactions",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_EndUserId",
                table: "Transactions",
                column: "EndUserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Withdrawals_PayoutSettings_PayoutSettingId",
                table: "Withdrawals",
                column: "PayoutSettingId",
                principalTable: "PayoutSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
