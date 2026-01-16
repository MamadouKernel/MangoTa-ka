using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MangoTaikaDistrict.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMfaCompetencesAndLivreOrPagess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UtilisateurId",
                table: "Scouts",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ScoutId1",
                table: "ScoutCompetences",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DemandesDroitRgpd",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UtilisateurId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Raison = table.Column<string>(type: "text", nullable: true),
                    Statut = table.Column<int>(type: "integer", nullable: false),
                    TraiteParId = table.Column<Guid>(type: "uuid", nullable: true),
                    TraiteLe = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Reponse = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DemandesDroitRgpd", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DemandesDroitRgpd_Utilisateurs_TraiteParId",
                        column: x => x.TraiteParId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DemandesDroitRgpd_Utilisateurs_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Scouts_UtilisateurId",
                table: "Scouts",
                column: "UtilisateurId");

            migrationBuilder.CreateIndex(
                name: "IX_ScoutCompetences_ScoutId1",
                table: "ScoutCompetences",
                column: "ScoutId1");

            migrationBuilder.CreateIndex(
                name: "IX_DemandesDroitRgpd_TraiteParId",
                table: "DemandesDroitRgpd",
                column: "TraiteParId");

            migrationBuilder.CreateIndex(
                name: "IX_DemandesDroitRgpd_UtilisateurId",
                table: "DemandesDroitRgpd",
                column: "UtilisateurId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScoutCompetences_Scouts_ScoutId1",
                table: "ScoutCompetences",
                column: "ScoutId1",
                principalTable: "Scouts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Scouts_Utilisateurs_UtilisateurId",
                table: "Scouts",
                column: "UtilisateurId",
                principalTable: "Utilisateurs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScoutCompetences_Scouts_ScoutId1",
                table: "ScoutCompetences");

            migrationBuilder.DropForeignKey(
                name: "FK_Scouts_Utilisateurs_UtilisateurId",
                table: "Scouts");

            migrationBuilder.DropTable(
                name: "DemandesDroitRgpd");

            migrationBuilder.DropIndex(
                name: "IX_Scouts_UtilisateurId",
                table: "Scouts");

            migrationBuilder.DropIndex(
                name: "IX_ScoutCompetences_ScoutId1",
                table: "ScoutCompetences");

            migrationBuilder.DropColumn(
                name: "UtilisateurId",
                table: "Scouts");

            migrationBuilder.DropColumn(
                name: "ScoutId1",
                table: "ScoutCompetences");
        }
    }
}
