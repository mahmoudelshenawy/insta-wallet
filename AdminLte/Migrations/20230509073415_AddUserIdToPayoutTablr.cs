using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminLte.Migrations
{
    public partial class AddUserIdToPayoutTablr : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "PayoutSettings",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_PayoutSettings_UserId",
                table: "PayoutSettings",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PayoutSettings_AspNetUsers_UserId",
                table: "PayoutSettings",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PayoutSettings_AspNetUsers_UserId",
                table: "PayoutSettings");

            migrationBuilder.DropIndex(
                name: "IX_PayoutSettings_UserId",
                table: "PayoutSettings");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "PayoutSettings");
        }
    }
}
