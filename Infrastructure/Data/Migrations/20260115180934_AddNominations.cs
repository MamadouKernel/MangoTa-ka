using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MangoTaikaDistrict.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNominations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Nominations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ScoutId = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Poste = table.Column<string>(type: "text", nullable: false),
                    Fonction = table.Column<string>(type: "text", nullable: true),
                    DateDebut = table.Column<DateOnly>(type: "date", nullable: false),
                    DateFin = table.Column<DateOnly>(type: "date", nullable: true),
                    Statut = table.Column<int>(type: "integer", nullable: false),
                    CurrentStep = table.Column<int>(type: "integer", nullable: false),
                    AutoriteValidationId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreeParId = table.Column<Guid>(type: "uuid", nullable: true),
                    Observations = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nominations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Nominations_Groupes_GroupeId",
                        column: x => x.GroupeId,
                        principalTable: "Groupes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Nominations_Scouts_ScoutId",
                        column: x => x.ScoutId,
                        principalTable: "Scouts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Nominations_Utilisateurs_AutoriteValidationId",
                        column: x => x.AutoriteValidationId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Nominations_Utilisateurs_CreeParId",
                        column: x => x.CreeParId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ValidationsNomination",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NominationId = table.Column<Guid>(type: "uuid", nullable: false),
                    StepOrder = table.Column<int>(type: "integer", nullable: false),
                    ValideurId = table.Column<Guid>(type: "uuid", nullable: false),
                    Decision = table.Column<int>(type: "integer", nullable: false),
                    Commentaire = table.Column<string>(type: "text", nullable: true),
                    DecidedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValidationsNomination", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ValidationsNomination_Nominations_NominationId",
                        column: x => x.NominationId,
                        principalTable: "Nominations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ValidationsNomination_Utilisateurs_ValideurId",
                        column: x => x.ValideurId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Nominations_AutoriteValidationId",
                table: "Nominations",
                column: "AutoriteValidationId");

            migrationBuilder.CreateIndex(
                name: "IX_Nominations_CreeParId",
                table: "Nominations",
                column: "CreeParId");

            migrationBuilder.CreateIndex(
                name: "IX_Nominations_GroupeId",
                table: "Nominations",
                column: "GroupeId");

            migrationBuilder.CreateIndex(
                name: "IX_Nominations_ScoutId",
                table: "Nominations",
                column: "ScoutId");

            migrationBuilder.CreateIndex(
                name: "IX_ValidationsNomination_NominationId",
                table: "ValidationsNomination",
                column: "NominationId");

            migrationBuilder.CreateIndex(
                name: "IX_ValidationsNomination_ValideurId",
                table: "ValidationsNomination",
                column: "ValideurId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ValidationsNomination");

            migrationBuilder.DropTable(
                name: "Nominations");
        }
    }
}
