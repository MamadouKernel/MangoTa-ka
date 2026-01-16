using ClosedXML.Excel;
using MangoTaikaDistrict.Domain.Entities;
using MangoTaikaDistrict.Domain.Enums;
using MangoTaikaDistrict.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MangoTaikaDistrict.Infrastructure.Services;

public class ExcelImportService : IExcelImportService
{
    private readonly AppDbContext _db;

    public ExcelImportService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<(int success, int errors, List<string> errorMessages)> ImportScoutsAsync(Stream excelStream)
    {
        var errors = new List<string>();
        int successCount = 0;
        int errorCount = 0;

        try
        {
            using var workbook = new XLWorkbook(excelStream);
            var worksheet = workbook.Worksheet(1); // Première feuille

            var firstRowUsed = worksheet.FirstRowUsed();
            if (firstRowUsed == null)
            {
                errors.Add("Le fichier Excel est vide.");
                return (0, 1, errors);
            }

            var firstRow = firstRowUsed.RowNumber();
            var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? firstRow;

            // Parcourir les lignes (en supposant que la première ligne contient les en-têtes)
            for (int row = firstRow + 1; row <= lastRow; row++)
            {
                try
                {
                    var nom = worksheet.Cell(row, GetColumnIndex(worksheet, "Nom", firstRow))?.GetValue<string>()?.Trim();
                    var prenoms = worksheet.Cell(row, GetColumnIndex(worksheet, "Prénoms", firstRow))?.GetValue<string>()?.Trim();
                    var matricule = worksheet.Cell(row, GetColumnIndex(worksheet, "Matricule", firstRow))?.GetValue<string>()?.Trim();
                    var sexe = worksheet.Cell(row, GetColumnIndex(worksheet, "Sexe", firstRow))?.GetValue<string>()?.Trim();
                    var dateNaissanceStr = worksheet.Cell(row, GetColumnIndex(worksheet, "Date de naissance", firstRow))?.GetValue<string>()?.Trim();
                    var groupeNom = worksheet.Cell(row, GetColumnIndex(worksheet, "Groupe", firstRow))?.GetValue<string>()?.Trim();
                    var statut = worksheet.Cell(row, GetColumnIndex(worksheet, "Statut", firstRow))?.GetValue<string>()?.Trim() ?? "Actif";

                    if (string.IsNullOrWhiteSpace(nom) || string.IsNullOrWhiteSpace(prenoms))
                    {
                        errors.Add($"Ligne {row}: Nom et Prénoms sont obligatoires.");
                        errorCount++;
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(groupeNom))
                    {
                        errors.Add($"Ligne {row}: Groupe est obligatoire.");
                        errorCount++;
                        continue;
                    }

                    // Chercher le groupe
                    var groupe = await _db.Groupes.FirstOrDefaultAsync(g => g.Nom == groupeNom);
                    if (groupe == null)
                    {
                        errors.Add($"Ligne {row}: Groupe '{groupeNom}' introuvable.");
                        errorCount++;
                        continue;
                    }

                    // Vérifier si le scout existe déjà (par matricule ou nom+prenoms)
                    Scout? existingScout = null;
                    if (!string.IsNullOrWhiteSpace(matricule))
                    {
                        existingScout = await _db.Scouts.FirstOrDefaultAsync(s => s.Matricule == matricule);
                    }

                    if (existingScout == null)
                    {
                        existingScout = await _db.Scouts
                            .FirstOrDefaultAsync(s => s.Nom == nom && s.Prenoms == prenoms && s.GroupeId == groupe.Id);
                    }

                    DateOnly? dateNaissance = null;
                    if (!string.IsNullOrWhiteSpace(dateNaissanceStr))
                    {
                        if (DateTime.TryParse(dateNaissanceStr, out var dt))
                        {
                            dateNaissance = DateOnly.FromDateTime(dt);
                        }
                    }

                    if (existingScout == null)
                    {
                        // Créer nouveau scout
                        var scout = new Scout
                        {
                            Nom = nom,
                            Prenoms = prenoms,
                            Matricule = matricule,
                            Sexe = sexe,
                            DateNaissance = dateNaissance,
                            GroupeId = groupe.Id,
                            Statut = statut
                        };

                        _db.Scouts.Add(scout);
                        successCount++;
                    }
                    else
                    {
                        // Mettre à jour scout existant
                        existingScout.Nom = nom;
                        existingScout.Prenoms = prenoms;
                        if (!string.IsNullOrWhiteSpace(matricule))
                            existingScout.Matricule = matricule;
                        existingScout.Sexe = sexe;
                        existingScout.DateNaissance = dateNaissance;
                        existingScout.GroupeId = groupe.Id;
                        existingScout.Statut = statut;
                        existingScout.UpdatedAt = DateTime.UtcNow;
                        successCount++;
                    }
                }
                catch (Exception ex)
                {
                    errors.Add($"Ligne {row}: Erreur - {ex.Message}");
                    errorCount++;
                }
            }

            await _db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            errors.Add($"Erreur lors de la lecture du fichier Excel: {ex.Message}");
            errorCount++;
        }

        return (successCount, errorCount, errors);
    }

    public async Task<(int success, int errors, List<string> errorMessages)> ImportCotisationsAsync(Stream excelStream)
    {
        var errors = new List<string>();
        int successCount = 0;
        int errorCount = 0;

        try
        {
            using var workbook = new XLWorkbook(excelStream);
            var worksheet = workbook.Worksheet(1);

            var firstRowUsed = worksheet.FirstRowUsed();
            if (firstRowUsed == null)
            {
                errors.Add("Le fichier Excel est vide.");
                return (0, 1, errors);
            }

            var firstRow = firstRowUsed.RowNumber();
            var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? firstRow;

            for (int row = firstRow + 1; row <= lastRow; row++)
            {
                try
                {
                    var scoutNom = worksheet.Cell(row, GetColumnIndex(worksheet, "Scout", firstRow))?.GetValue<string>()?.Trim();
                    var groupeNom = worksheet.Cell(row, GetColumnIndex(worksheet, "Groupe", firstRow))?.GetValue<string>()?.Trim();
                    var periode = worksheet.Cell(row, GetColumnIndex(worksheet, "Période", firstRow))?.GetValue<string>()?.Trim();
                    var montantAttenduStr = worksheet.Cell(row, GetColumnIndex(worksheet, "Montant Attendu", firstRow))?.GetValue<string>()?.Trim();
                    var montantPayeStr = worksheet.Cell(row, GetColumnIndex(worksheet, "Montant Payé", firstRow))?.GetValue<string>()?.Trim() ?? "0";

                    if (string.IsNullOrWhiteSpace(scoutNom) || string.IsNullOrWhiteSpace(groupeNom) || string.IsNullOrWhiteSpace(periode))
                    {
                        errors.Add($"Ligne {row}: Scout, Groupe et Période sont obligatoires.");
                        errorCount++;
                        continue;
                    }

                    // Chercher le scout (par nom+prenoms)
                    var nomParts = scoutNom.Split(' ', 2);
                    var nom = nomParts[0];
                    var prenoms = nomParts.Length > 1 ? nomParts[1] : "";

                    var scout = await _db.Scouts
                        .Include(s => s.Groupe)
                        .FirstOrDefaultAsync(s => s.Nom == nom && s.Prenoms == prenoms);

                    if (scout == null)
                    {
                        errors.Add($"Ligne {row}: Scout '{scoutNom}' introuvable.");
                        errorCount++;
                        continue;
                    }

                    var groupe = await _db.Groupes.FirstOrDefaultAsync(g => g.Nom == groupeNom);
                    if (groupe == null)
                    {
                        errors.Add($"Ligne {row}: Groupe '{groupeNom}' introuvable.");
                        errorCount++;
                        continue;
                    }

                    if (!decimal.TryParse(montantAttenduStr, out var montantAttendu))
                    {
                        errors.Add($"Ligne {row}: Montant Attendu invalide.");
                        errorCount++;
                        continue;
                    }

                    decimal.TryParse(montantPayeStr, out var montantPaye);

                    // Vérifier si la cotisation existe déjà
                    var existing = await _db.Cotisations
                        .FirstOrDefaultAsync(c => c.ScoutId == scout.Id && c.GroupeId == groupe.Id && c.Periode == periode);

                    var statut = montantPaye >= montantAttendu ? StatutCotisation.PAYE :
                                 montantPaye > 0 ? StatutCotisation.PARTIEL :
                                 StatutCotisation.IMPAYE;

                    if (existing == null)
                    {
                        var cotisation = new Cotisation
                        {
                            ScoutId = scout.Id,
                            GroupeId = groupe.Id,
                            Periode = periode,
                            MontantAttendu = montantAttendu,
                            MontantPaye = montantPaye,
                            Statut = statut,
                            DatePaiement = statut == StatutCotisation.PAYE ? DateTime.UtcNow : null
                        };

                        _db.Cotisations.Add(cotisation);
                        successCount++;
                    }
                    else
                    {
                        existing.MontantAttendu = montantAttendu;
                        existing.MontantPaye = montantPaye;
                        existing.Statut = statut;
                        existing.DatePaiement = statut == StatutCotisation.PAYE ? DateTime.UtcNow : null;
                        existing.UpdatedAt = DateTime.UtcNow;
                        successCount++;
                    }
                }
                catch (Exception ex)
                {
                    errors.Add($"Ligne {row}: Erreur - {ex.Message}");
                    errorCount++;
                }
            }

            await _db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            errors.Add($"Erreur lors de la lecture du fichier Excel: {ex.Message}");
            errorCount++;
        }

        return (successCount, errorCount, errors);
    }

    public async Task<(int success, int errors, List<string> errorMessages)> ImportNominationsAsync(Stream excelStream)
    {
        var errors = new List<string>();
        int successCount = 0;
        int errorCount = 0;

        try
        {
            using var workbook = new XLWorkbook(excelStream);
            var worksheet = workbook.Worksheet(1);

            var firstRowUsed = worksheet.FirstRowUsed();
            if (firstRowUsed == null)
            {
                errors.Add("Le fichier Excel est vide.");
                return (0, 1, errors);
            }

            var firstRow = firstRowUsed.RowNumber();
            var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? firstRow;

            for (int row = firstRow + 1; row <= lastRow; row++)
            {
                try
                {
                    var scoutNom = worksheet.Cell(row, GetColumnIndex(worksheet, "Scout", firstRow))?.GetValue<string>()?.Trim();
                    var groupeNom = worksheet.Cell(row, GetColumnIndex(worksheet, "Groupe", firstRow))?.GetValue<string>()?.Trim();
                    var poste = worksheet.Cell(row, GetColumnIndex(worksheet, "Poste", firstRow))?.GetValue<string>()?.Trim();
                    var fonction = worksheet.Cell(row, GetColumnIndex(worksheet, "Fonction", firstRow))?.GetValue<string>()?.Trim();
                    var dateDebutStr = worksheet.Cell(row, GetColumnIndex(worksheet, "Date Début", firstRow))?.GetValue<string>()?.Trim();
                    var dateFinStr = worksheet.Cell(row, GetColumnIndex(worksheet, "Date Fin", firstRow))?.GetValue<string>()?.Trim();

                    if (string.IsNullOrWhiteSpace(scoutNom) || string.IsNullOrWhiteSpace(groupeNom) || string.IsNullOrWhiteSpace(poste))
                    {
                        errors.Add($"Ligne {row}: Scout, Groupe et Poste sont obligatoires.");
                        errorCount++;
                        continue;
                    }

                    // Chercher le scout
                    var nomParts = scoutNom.Split(' ', 2);
                    var nom = nomParts[0];
                    var prenoms = nomParts.Length > 1 ? nomParts[1] : "";

                    var scout = await _db.Scouts.FirstOrDefaultAsync(s => s.Nom == nom && s.Prenoms == prenoms);
                    if (scout == null)
                    {
                        errors.Add($"Ligne {row}: Scout '{scoutNom}' introuvable.");
                        errorCount++;
                        continue;
                    }

                    var groupe = await _db.Groupes.FirstOrDefaultAsync(g => g.Nom == groupeNom);
                    if (groupe == null)
                    {
                        errors.Add($"Ligne {row}: Groupe '{groupeNom}' introuvable.");
                        errorCount++;
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(dateDebutStr) || !DateOnly.TryParse(dateDebutStr, out var dateDebut))
                    {
                        errors.Add($"Ligne {row}: Date Début invalide.");
                        errorCount++;
                        continue;
                    }

                    DateOnly? dateFin = null;
                    if (!string.IsNullOrWhiteSpace(dateFinStr) && DateOnly.TryParse(dateFinStr, out var df))
                    {
                        dateFin = df;
                    }

                    // Vérifier si la nomination existe déjà
                    var existing = await _db.Nominations
                        .FirstOrDefaultAsync(n => n.ScoutId == scout.Id && n.GroupeId == groupe.Id && n.Poste == poste && n.DateDebut == dateDebut);

                    if (existing == null)
                    {
                        var nomination = new Nomination
                        {
                            ScoutId = scout.Id,
                            GroupeId = groupe.Id,
                            Poste = poste,
                            Fonction = fonction,
                            DateDebut = dateDebut,
                            DateFin = dateFin,
                            Statut = StatutNomination.BROUILLON
                        };

                        _db.Nominations.Add(nomination);
                        successCount++;
                    }
                    else
                    {
                        existing.Fonction = fonction;
                        existing.DateFin = dateFin;
                        existing.UpdatedAt = DateTime.UtcNow;
                        successCount++;
                    }
                }
                catch (Exception ex)
                {
                    errors.Add($"Ligne {row}: Erreur - {ex.Message}");
                    errorCount++;
                }
            }

            await _db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            errors.Add($"Erreur lors de la lecture du fichier Excel: {ex.Message}");
            errorCount++;
        }

        return (successCount, errorCount, errors);
    }

    private int GetColumnIndex(IXLWorksheet worksheet, string columnName, int headerRow)
    {
        var lastCol = worksheet.LastColumnUsed()?.ColumnNumber() ?? 1;
        for (int col = 1; col <= lastCol; col++)
        {
            var cellValue = worksheet.Cell(headerRow, col)?.GetValue<string>()?.Trim();
            if (cellValue != null && cellValue.Contains(columnName, StringComparison.OrdinalIgnoreCase))
            {
                return col;
            }
        }
        return -1;
    }
}
