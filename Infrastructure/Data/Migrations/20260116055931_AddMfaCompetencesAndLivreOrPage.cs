using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MangoTaikaDistrict.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMfaCompetencesAndLivreOrPage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AscciStatuses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ScoutId = table.Column<Guid>(type: "uuid", nullable: false),
                    NumeroCarte = table.Column<string>(type: "text", nullable: true),
                    Statut = table.Column<string>(type: "text", nullable: false),
                    DateVerification = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DateExpiration = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Observations = table.Column<string>(type: "text", nullable: true),
                    VerifieParId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AscciStatuses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AscciStatuses_Scouts_ScoutId",
                        column: x => x.ScoutId,
                        principalTable: "Scouts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AscciStatuses_Utilisateurs_VerifieParId",
                        column: x => x.VerifieParId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Formations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Titre = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Contenu = table.Column<string>(type: "text", nullable: true),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    DureeEstimee = table.Column<int>(type: "integer", nullable: false),
                    Niveau = table.Column<string>(type: "text", nullable: false),
                    EstActive = table.Column<bool>(type: "boolean", nullable: false),
                    EstPublique = table.Column<bool>(type: "boolean", nullable: false),
                    OrdreAffichage = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Formations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Formations_Utilisateurs_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "InscriptionsFormation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FormationId = table.Column<Guid>(type: "uuid", nullable: false),
                    ScoutId = table.Column<Guid>(type: "uuid", nullable: false),
                    Statut = table.Column<int>(type: "integer", nullable: false),
                    DateInscription = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateDebut = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DateCompletion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NoteFinale = table.Column<decimal>(type: "numeric", nullable: true),
                    Commentaire = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InscriptionsFormation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InscriptionsFormation_Formations_FormationId",
                        column: x => x.FormationId,
                        principalTable: "Formations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InscriptionsFormation_Scouts_ScoutId",
                        column: x => x.ScoutId,
                        principalTable: "Scouts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ModulesFormation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FormationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Titre = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Contenu = table.Column<string>(type: "text", nullable: true),
                    VideoUrl = table.Column<string>(type: "text", nullable: true),
                    DocumentUrl = table.Column<string>(type: "text", nullable: true),
                    DureeEstimee = table.Column<int>(type: "integer", nullable: false),
                    Ordre = table.Column<int>(type: "integer", nullable: false),
                    EstObligatoire = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModulesFormation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModulesFormation_Formations_FormationId",
                        column: x => x.FormationId,
                        principalTable: "Formations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Certificats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InscriptionFormationId = table.Column<Guid>(type: "uuid", nullable: false),
                    NumeroCertificat = table.Column<string>(type: "text", nullable: false),
                    DateEmission = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateExpiration = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UrlCertificat = table.Column<string>(type: "text", nullable: true),
                    EstValide = table.Column<bool>(type: "boolean", nullable: false),
                    EmisParId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Certificats_InscriptionsFormation_InscriptionFormationId",
                        column: x => x.InscriptionFormationId,
                        principalTable: "InscriptionsFormation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Certificats_Utilisateurs_EmisParId",
                        column: x => x.EmisParId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ProgressionsModule",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InscriptionFormationId = table.Column<Guid>(type: "uuid", nullable: false),
                    ModuleFormationId = table.Column<Guid>(type: "uuid", nullable: false),
                    EstComplete = table.Column<bool>(type: "boolean", nullable: false),
                    DateDebut = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DateCompletion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TempsConsacre = table.Column<int>(type: "integer", nullable: false),
                    Note = table.Column<decimal>(type: "numeric", nullable: true),
                    Commentaire = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgressionsModule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProgressionsModule_InscriptionsFormation_InscriptionFormati~",
                        column: x => x.InscriptionFormationId,
                        principalTable: "InscriptionsFormation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProgressionsModule_ModulesFormation_ModuleFormationId",
                        column: x => x.ModuleFormationId,
                        principalTable: "ModulesFormation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AscciStatuses_ScoutId",
                table: "AscciStatuses",
                column: "ScoutId");

            migrationBuilder.CreateIndex(
                name: "IX_AscciStatuses_VerifieParId",
                table: "AscciStatuses",
                column: "VerifieParId");

            migrationBuilder.CreateIndex(
                name: "IX_Certificats_EmisParId",
                table: "Certificats",
                column: "EmisParId");

            migrationBuilder.CreateIndex(
                name: "IX_Certificats_InscriptionFormationId",
                table: "Certificats",
                column: "InscriptionFormationId");

            migrationBuilder.CreateIndex(
                name: "IX_Formations_CreatedById",
                table: "Formations",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InscriptionsFormation_FormationId",
                table: "InscriptionsFormation",
                column: "FormationId");

            migrationBuilder.CreateIndex(
                name: "IX_InscriptionsFormation_ScoutId",
                table: "InscriptionsFormation",
                column: "ScoutId");

            migrationBuilder.CreateIndex(
                name: "IX_ModulesFormation_FormationId",
                table: "ModulesFormation",
                column: "FormationId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgressionsModule_InscriptionFormationId",
                table: "ProgressionsModule",
                column: "InscriptionFormationId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgressionsModule_ModuleFormationId",
                table: "ProgressionsModule",
                column: "ModuleFormationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AscciStatuses");

            migrationBuilder.DropTable(
                name: "Certificats");

            migrationBuilder.DropTable(
                name: "ProgressionsModule");

            migrationBuilder.DropTable(
                name: "InscriptionsFormation");

            migrationBuilder.DropTable(
                name: "ModulesFormation");

            migrationBuilder.DropTable(
                name: "Formations");
        }
    }
}
