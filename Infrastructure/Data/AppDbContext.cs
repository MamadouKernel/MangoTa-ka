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
    }
}
