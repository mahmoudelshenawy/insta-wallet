using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminLte.Migrations
{
    public partial class AddNullableToFileAttr : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deposits_Attachments_AttachmentId",
                table: "Deposits");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_AttachmentId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_ConfirmAttachmentId",
                table: "Transactions");

            migrationBuilder.AlterColumn<int>(
                name: "PaymentTypeId",
                table: "Transactions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ConfirmAttachmentId",
                table: "Transactions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "AttachmentId",
                table: "Transactions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "PaymentTypeId",
                table: "Deposits",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "AttachmentId",
                table: "Deposits",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_AttachmentId",
                table: "Transactions",
                column: "AttachmentId",
                unique: true,
                filter: "[AttachmentId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_ConfirmAttachmentId",
                table: "Transactions",
                column: "ConfirmAttachmentId",
                unique: true,
                filter: "[ConfirmAttachmentId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Deposits_Attachments_AttachmentId",
                table: "Deposits",
                column: "AttachmentId",
                principalTable: "Attachments",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deposits_Attachments_AttachmentId",
                table: "Deposits");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_AttachmentId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_ConfirmAttachmentId",
                table: "Transactions");

            migrationBuilder.AlterColumn<int>(
                name: "PaymentTypeId",
                table: "Transactions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ConfirmAttachmentId",
                table: "Transactions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AttachmentId",
                table: "Transactions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PaymentTypeId",
                table: "Deposits",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AttachmentId",
                table: "Deposits",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_AttachmentId",
                table: "Transactions",
                column: "AttachmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_ConfirmAttachmentId",
                table: "Transactions",
                column: "ConfirmAttachmentId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Deposits_Attachments_AttachmentId",
                table: "Deposits",
                column: "AttachmentId",
                principalTable: "Attachments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
