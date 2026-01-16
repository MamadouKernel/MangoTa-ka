using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MangoTaikaDistrict.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCotisations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cotisations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ScoutId = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Periode = table.Column<string>(type: "text", nullable: false),
                    MontantAttendu = table.Column<decimal>(type: "numeric", nullable: false),
                    MontantPaye = table.Column<decimal>(type: "numeric", nullable: false),
                    Statut = table.Column<int>(type: "integer", nullable: false),
                    DateEnregistrement = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DatePaiement = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Observations = table.Column<string>(type: "text", nullable: true),
                    EnregistreParId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cotisations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cotisations_Groupes_GroupeId",
                        column: x => x.GroupeId,
                        principalTable: "Groupes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cotisations_Scouts_ScoutId",
                        column: x => x.ScoutId,
                        principalTable: "Scouts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cotisations_Utilisateurs_EnregistreParId",
                        column: x => x.EnregistreParId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cotisations_EnregistreParId",
                table: "Cotisations",
                column: "EnregistreParId");

            migrationBuilder.CreateIndex(
                name: "IX_Cotisations_GroupeId",
                table: "Cotisations",
                column: "GroupeId");

            migrationBuilder.CreateIndex(
                name: "IX_Cotisations_ScoutId",
                table: "Cotisations",
                column: "ScoutId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cotisations");
        }
    }
}
