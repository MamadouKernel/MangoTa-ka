using MangoTaikaDistrict.Domain.Entities;
using MangoTaikaDistrict.Domain.Enums;
using MangoTaikaDistrict.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;

namespace MangoTaikaDistrict.Infrastructure.Data.Seed;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db, IPasswordService password)
    {
        await db.Database.MigrateAsync();

        if (!await db.Roles.AnyAsync())
        {
            db.Roles.AddRange(new[]
            {
                new Role{ Code = RoleCode.ADMIN, Libelle="Administrateur" },
                new Role{ Code = RoleCode.GESTIONNAIRE, Libelle="Gestionnaire" },
                new Role{ Code = RoleCode.SUPERVISEUR, Libelle="Superviseur" },
                new Role{ Code = RoleCode.RESPONSABLE_GROUPE, Libelle="Responsable de Groupe" },
                new Role{ Code = RoleCode.SCOUT, Libelle="Scout" },
                new Role{ Code = RoleCode.PARENT, Libelle="Parent" },
                new Role{ Code = RoleCode.CONSULTANT, Libelle="Consultant" }
            });
            await db.SaveChangesAsync();
        }

        // Admin user: tel 0100000000 / mdp Admin@2026
        var admin = await db.Utilisateurs.FirstOrDefaultAsync(u => u.Telephone == "0100000000");
        if (admin is null)
        {
            admin = new Utilisateur
            {
                Telephone = "0100000000",
                Email = "admin@mangotaika.local",
                Nom = "ADMIN",
                Prenoms = "MangoTaika",
                PasswordHash = password.Hash("Admin@2026"),
                IsActive = true,
                MfaEnabled = false
            };
            db.Utilisateurs.Add(admin);
            await db.SaveChangesAsync();

            var adminRoleId = await db.Roles
                .Where(r => r.Code == RoleCode.ADMIN)
                .Select(r => r.Id)
                .FirstAsync();

            db.UtilisateurRoles.Add(new UtilisateurRole
            {
                UtilisateurId = admin.Id,
                RoleId = adminRoleId
            });

            await db.SaveChangesAsync();
        }
        else
        {
            // Corriger le hash si c'est un placeholder
            if (admin.PasswordHash == null || 
                admin.PasswordHash.Contains("CHANGE_ME") || 
                admin.PasswordHash.Contains("PLACEHOLDER") ||
                !admin.PasswordHash.StartsWith("PBKDF2$"))
            {
                admin.PasswordHash = password.Hash("Admin@2026");
                admin.IsActive = true;
                await db.SaveChangesAsync();
            }

            // S'assurer que l'admin a le rôle ADMIN
            var hasAdminRole = await db.UtilisateurRoles
                .Include(ur => ur.Role)
                .AnyAsync(ur => ur.UtilisateurId == admin.Id && ur.Role.Code == RoleCode.ADMIN);

            if (!hasAdminRole)
            {
                var adminRoleId = await db.Roles
                    .Where(r => r.Code == RoleCode.ADMIN)
                    .Select(r => r.Id)
                    .FirstAsync();

                db.UtilisateurRoles.Add(new UtilisateurRole
                {
                    UtilisateurId = admin.Id,
                    RoleId = adminRoleId
                });
                await db.SaveChangesAsync();
            }
        }

        // Circuit default
        if (!await db.CircuitsValidation.AnyAsync(c => c.Code == "AUTORISATION_ACTIVITE"))
        {
            var circuit = new CircuitValidation
            {
                Code = "AUTORISATION_ACTIVITE",
                Libelle = "Circuit d'autorisation d'activité",
                IsActive = true,
                Etapes = new List<CircuitEtape>
                {
                    new CircuitEtape{ StepOrder = 1, RoleRequis = "GESTIONNAIRE", Libelle="Validation gestionnaire" },
                    new CircuitEtape{ StepOrder = 2, RoleRequis = "SUPERVISEUR", Libelle="Validation superviseur" },
                    new CircuitEtape{ StepOrder = 3, RoleRequis = "ADMIN", Libelle="Validation finale admin" }
                }
            };
            db.CircuitsValidation.Add(circuit);
            await db.SaveChangesAsync();
        }
    }
}
