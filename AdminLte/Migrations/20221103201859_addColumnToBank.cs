using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminLte.Migrations
{
    public partial class addColumnToBank : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "Banks",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Active",
                table: "Banks");
        }
    }
}
