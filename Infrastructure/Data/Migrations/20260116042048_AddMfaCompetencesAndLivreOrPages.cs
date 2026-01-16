using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MangoTaikaDistrict.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMfaCompetencesAndLivreOrPages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsValidated",
                table: "Utilisateurs",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "MfaSecret",
                table: "Utilisateurs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidatedAt",
                table: "Utilisateurs",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ValidatedById",
                table: "Utilisateurs",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Adresse",
                table: "Scouts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "GpsLat",
                table: "Scouts",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "GpsLng",
                table: "Scouts",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LieuNaissance",
                table: "Scouts",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Competences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Libelle = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Competences", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LivreOrPages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Titre = table.Column<string>(type: "text", nullable: false),
                    Contenu = table.Column<string>(type: "text", nullable: false),
                    PhotoUrl = table.Column<string>(type: "text", nullable: true),
                    Ordre = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LivreOrPages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MotsCommissaire",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Titre = table.Column<string>(type: "text", nullable: false),
                    Contenu = table.Column<string>(type: "text", nullable: false),
                    PhotoUrl = table.Column<string>(type: "text", nullable: true),
                    Annee = table.Column<int>(type: "integer", nullable: false),
                    DateDebut = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateFin = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MotsCommissaire", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MotsCommissaire_Utilisateurs_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ScoutCompetences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ScoutId = table.Column<Guid>(type: "uuid", nullable: false),
                    CompetenceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Niveau = table.Column<string>(type: "text", nullable: true),
                    DateAcquisition = table.Column<DateOnly>(type: "date", nullable: true),
                    CertificatUrl = table.Column<string>(type: "text", nullable: true),
                    Observations = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScoutCompetences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScoutCompetences_Competences_CompetenceId",
                        column: x => x.CompetenceId,
                        principalTable: "Competences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ScoutCompetences_Scouts_ScoutId",
                        column: x => x.ScoutId,
                        principalTable: "Scouts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Utilisateurs_ValidatedById",
                table: "Utilisateurs",
                column: "ValidatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MotsCommissaire_CreatedById",
                table: "MotsCommissaire",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ScoutCompetences_CompetenceId",
                table: "ScoutCompetences",
                column: "CompetenceId");

            migrationBuilder.CreateIndex(
                name: "IX_ScoutCompetences_ScoutId",
                table: "ScoutCompetences",
                column: "ScoutId");

            migrationBuilder.AddForeignKey(
                name: "FK_Utilisateurs_Utilisateurs_ValidatedById",
                table: "Utilisateurs",
                column: "ValidatedById",
                principalTable: "Utilisateurs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Utilisateurs_Utilisateurs_ValidatedById",
                table: "Utilisateurs");

            migrationBuilder.DropTable(
                name: "LivreOrPages");

            migrationBuilder.DropTable(
                name: "MotsCommissaire");

            migrationBuilder.DropTable(
                name: "ScoutCompetences");

            migrationBuilder.DropTable(
                name: "Competences");

            migrationBuilder.DropIndex(
                name: "IX_Utilisateurs_ValidatedById",
                table: "Utilisateurs");

            migrationBuilder.DropColumn(
                name: "IsValidated",
                table: "Utilisateurs");

            migrationBuilder.DropColumn(
                name: "MfaSecret",
                table: "Utilisateurs");

            migrationBuilder.DropColumn(
                name: "ValidatedAt",
                table: "Utilisateurs");

            migrationBuilder.DropColumn(
                name: "ValidatedById",
                table: "Utilisateurs");

            migrationBuilder.DropColumn(
                name: "Adresse",
                table: "Scouts");

            migrationBuilder.DropColumn(
                name: "GpsLat",
                table: "Scouts");

            migrationBuilder.DropColumn(
                name: "GpsLng",
                table: "Scouts");

            migrationBuilder.DropColumn(
                name: "LieuNaissance",
                table: "Scouts");
        }
    }
}
