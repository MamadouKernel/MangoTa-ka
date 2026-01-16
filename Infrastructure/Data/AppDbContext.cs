using Microsoft.EntityFrameworkCore;
using MangoTaikaDistrict.Domain.Entities;

namespace MangoTaikaDistrict.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Utilisateur> Utilisateurs => Set<Utilisateur>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UtilisateurRole> UtilisateurRoles => Set<UtilisateurRole>();
    public DbSet<Consentement> Consentements => Set<Consentement>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    public DbSet<Groupe> Groupes => Set<Groupe>();
    public DbSet<Unite> Unites => Set<Unite>();
    public DbSet<Scout> Scouts => Set<Scout>();
    public DbSet<Parent> Parents => Set<Parent>();
    public DbSet<ParentScout> ParentScouts => Set<ParentScout>();

    public DbSet<CarteScout> CartesScout => Set<CarteScout>();
    public DbSet<Assurance> Assurances => Set<Assurance>();
    public DbSet<Document> Documents => Set<Document>();

    public DbSet<Actualite> Actualites => Set<Actualite>();
    public DbSet<Album> Albums => Set<Album>();
    public DbSet<Media> Medias => Set<Media>();
    public DbSet<LivreOrMessage> LivreOr => Set<LivreOrMessage>();

    public DbSet<Activite> Activites => Set<Activite>();
    public DbSet<DemandeAutorisation> DemandesAutorisation => Set<DemandeAutorisation>();
    public DbSet<CircuitValidation> CircuitsValidation => Set<CircuitValidation>();
    public DbSet<CircuitEtape> CircuitEtapes => Set<CircuitEtape>();
    public DbSet<Validation> Validations => Set<Validation>();

    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<TicketMessage> TicketMessages => Set<TicketMessage>();

    public DbSet<Cotisation> Cotisations => Set<Cotisation>();
    public DbSet<Nomination> Nominations => Set<Nomination>();
    public DbSet<ValidationNomination> ValidationsNomination => Set<ValidationNomination>();

    public DbSet<MotCommissaire> MotsCommissaire => Set<MotCommissaire>();

    public DbSet<Competence> Competences => Set<Competence>();
    public DbSet<ScoutCompetence> ScoutCompetences => Set<ScoutCompetence>();
    public DbSet<LivreOrPage> LivreOrPages => Set<LivreOrPage>();
    public DbSet<DemandeDroitRgpd> DemandesDroitRgpd => Set<DemandeDroitRgpd>();
    public DbSet<AscciStatus> AscciStatuses => Set<AscciStatus>();

    // LMS
    public DbSet<Formation> Formations => Set<Formation>();
    public DbSet<ModuleFormation> ModulesFormation => Set<ModuleFormation>();
    public DbSet<InscriptionFormation> InscriptionsFormation => Set<InscriptionFormation>();
    public DbSet<ProgressionModule> ProgressionsModule => Set<ProgressionModule>();
    public DbSet<Certificat> Certificats => Set<Certificat>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("uuid-ossp");

        modelBuilder.Entity<Utilisateur>()
            .HasIndex(x => x.Telephone)
            .IsUnique();

        modelBuilder.Entity<Scout>()
            .HasIndex(x => x.Matricule)
            .IsUnique();

        // Many-to-many Utilisateur <-> Role via UtilisateurRole
        modelBuilder.Entity<UtilisateurRole>()
            .HasKey(x => new { x.UtilisateurId, x.RoleId });

        modelBuilder.Entity<UtilisateurRole>()
            .HasOne(x => x.Utilisateur)
            .WithMany(x => x.UtilisateurRoles)
            .HasForeignKey(x => x.UtilisateurId);

        modelBuilder.Entity<UtilisateurRole>()
            .HasOne(x => x.Role)
            .WithMany()
            .HasForeignKey(x => x.RoleId);

        // Parent <-> Scout
        modelBuilder.Entity<ParentScout>()
            .HasKey(x => new { x.ParentId, x.ScoutId });

        modelBuilder.Entity<ParentScout>()
            .HasOne(x => x.Parent)
            .WithMany(x => x.ParentScouts)
            .HasForeignKey(x => x.ParentId);

        modelBuilder.Entity<ParentScout>()
            .HasOne(x => x.Scout)
            .WithMany()
            .HasForeignKey(x => x.ScoutId);

        // Document.UploadedBy
        modelBuilder.Entity<Document>()
            .HasOne(d => d.UploadedBy)
            .WithMany()
            .HasForeignKey(d => d.UploadedById)
            .OnDelete(DeleteBehavior.Restrict);

        // Ticket relations
        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.CreatedBy)
            .WithMany()
            .HasForeignKey(t => t.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.AssignedTo)
            .WithMany()
            .HasForeignKey(t => t.AssignedToId)
            .OnDelete(DeleteBehavior.Restrict);

        // Activite <-> DemandeAutorisation (1-1)
        modelBuilder.Entity<Activite>()
            .HasOne(a => a.DemandeAutorisation)
            .WithOne(d => d.Activite)
            .HasForeignKey<DemandeAutorisation>(d => d.ActiviteId);

        // Cotisation relations
        modelBuilder.Entity<Cotisation>()
            .HasOne(c => c.Scout)
            .WithMany(s => s.Cotisations)
            .HasForeignKey(c => c.ScoutId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Cotisation>()
            .HasOne(c => c.Groupe)
            .WithMany(g => g.Cotisations)
            .HasForeignKey(c => c.GroupeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Cotisation>()
            .HasOne(c => c.EnregistrePar)
            .WithMany()
            .HasForeignKey(c => c.EnregistreParId)
            .OnDelete(DeleteBehavior.SetNull);

        // Nomination relations
        modelBuilder.Entity<Nomination>()
            .HasOne(n => n.Scout)
            .WithMany(s => s.Nominations)
            .HasForeignKey(n => n.ScoutId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Nomination>()
            .HasOne(n => n.Groupe)
            .WithMany(g => g.Nominations)
            .HasForeignKey(n => n.GroupeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Nomination>()
            .HasOne(n => n.CreePar)
            .WithMany()
            .HasForeignKey(n => n.CreeParId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Nomination>()
            .HasOne(n => n.AutoriteValidation)
            .WithMany()
            .HasForeignKey(n => n.AutoriteValidationId)
            .OnDelete(DeleteBehavior.SetNull);

        // ValidationNomination relations
        modelBuilder.Entity<ValidationNomination>()
            .HasOne(v => v.Nomination)
            .WithMany(n => n.Validations)
            .HasForeignKey(v => v.NominationId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ValidationNomination>()
            .HasOne(v => v.Valideur)
            .WithMany()
            .HasForeignKey(v => v.ValideurId)
            .OnDelete(DeleteBehavior.Restrict);

        // MotCommissaire relations
        modelBuilder.Entity<MotCommissaire>()
            .HasOne(m => m.CreatedBy)
            .WithMany()
            .HasForeignKey(m => m.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        // Utilisateur validation relations
        modelBuilder.Entity<Utilisateur>()
            .HasOne(u => u.ValidatedBy)
            .WithMany()
            .HasForeignKey(u => u.ValidatedById)
            .OnDelete(DeleteBehavior.SetNull);

        // ScoutCompetence relations
        modelBuilder.Entity<ScoutCompetence>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<ScoutCompetence>()
            .HasOne(sc => sc.Scout)
            .WithMany()
            .HasForeignKey(sc => sc.ScoutId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ScoutCompetence>()
            .HasOne(sc => sc.Competence)
            .WithMany(c => c.ScoutCompetences)
            .HasForeignKey(sc => sc.CompetenceId)
            .OnDelete(DeleteBehavior.Restrict);

        // Scout - Utilisateur relation
        modelBuilder.Entity<Scout>()
            .HasOne(s => s.Utilisateur)
            .WithMany(u => u.Scouts)
            .HasForeignKey(s => s.UtilisateurId)
            .OnDelete(DeleteBehavior.SetNull);

        // DemandeDroitRgpd relations
        modelBuilder.Entity<DemandeDroitRgpd>()
            .HasOne(d => d.Utilisateur)
            .WithMany(u => u.DemandesDroitRgpd)
            .HasForeignKey(d => d.UtilisateurId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DemandeDroitRgpd>()
            .HasOne(d => d.TraitePar)
            .WithMany()
            .HasForeignKey(d => d.TraiteParId)
            .OnDelete(DeleteBehavior.SetNull);

        // AscciStatus relations
        modelBuilder.Entity<AscciStatus>()
            .HasOne(a => a.Scout)
            .WithMany()
            .HasForeignKey(a => a.ScoutId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AscciStatus>()
            .HasOne(a => a.VerifiePar)
            .WithMany()
            .HasForeignKey(a => a.VerifieParId)
            .OnDelete(DeleteBehavior.SetNull);

        // LMS Relations
        modelBuilder.Entity<Formation>()
            .HasOne(f => f.CreatedBy)
            .WithMany()
            .HasForeignKey(f => f.CreatedById)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<ModuleFormation>()
            .HasOne(m => m.Formation)
            .WithMany(f => f.Modules)
            .HasForeignKey(m => m.FormationId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<InscriptionFormation>()
            .HasOne(i => i.Formation)
            .WithMany(f => f.Inscriptions)
            .HasForeignKey(i => i.FormationId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<InscriptionFormation>()
            .HasOne(i => i.Scout)
            .WithMany()
            .HasForeignKey(i => i.ScoutId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProgressionModule>()
            .HasOne(p => p.InscriptionFormation)
            .WithMany(i => i.Progressions)
            .HasForeignKey(p => p.InscriptionFormationId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProgressionModule>()
            .HasOne(p => p.ModuleFormation)
            .WithMany(m => m.Progressions)
            .HasForeignKey(p => p.ModuleFormationId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Certificat>()
            .HasOne(c => c.InscriptionFormation)
            .WithMany(i => i.Certificats)
            .HasForeignKey(c => c.InscriptionFormationId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Certificat>()
            .HasOne(c => c.EmisPar)
            .WithMany()
            .HasForeignKey(c => c.EmisParId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
