using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminLte.Migrations
{
    public partial class AddCountriesAndAttachmentsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Attachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attachments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShortName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Iso3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumberCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDefault = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "no")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "Iso3", "Name", "NumberCode", "PhoneCode", "ShortName" },
                values: new object[,]
                {
                    { 1, "EGY", "Egypt", "818", "20", "EG" },
                    { 2, "SAU", "Saudi Arabia", "682", "966", "SA" },
                    { 3, "JOR", "Jordan", "400", "692", "JO" },
                    { 4, "SYR", "Syrian Arab Republic", "760", "963", "SY" },
                    { 5, "IRQ", "Iraq", "386", "20", "IQ" },
                    { 6, "ARE", "United Arab Emirates", "784", "971", "AE" },
                    { 7, "OMN", "Oman", "512", "968", "OM" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Countries_ShortName",
                table: "Countries",
                column: "ShortName",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Attachments");

            migrationBuilder.DropTable(
                name: "Countries");
        }
    }
}
