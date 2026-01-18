using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MangoTaikaDistrict.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLogoUrlToGroupe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "Groupes",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "Groupes");
        }
    }
}
