using MangoTaikaDistrict.Application.Interfaces;
using MangoTaikaDistrict.Application.Services;
using MangoTaikaDistrict.Infrastructure.Data;
using MangoTaikaDistrict.Infrastructure.Data.Seed;
using MangoTaikaDistrict.Infrastructure.Repositories;
using MangoTaikaDistrict.Infrastructure.Security;
using MangoTaikaDistrict.Infrastructure.Storage;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

QuestPDF.Settings.License = LicenseType.Community;

builder.Services.AddControllersWithViews();

// Sessions pour MFA
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// Auth Cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.Name = "MangoTaikaDistrict.Auth";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

// DI - Repositories
builder.Services.AddScoped<IUtilisateurRepository, UtilisateurRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IGroupeRepository, GroupeRepository>();
builder.Services.AddScoped<IScoutRepository, ScoutRepository>();
builder.Services.AddScoped<ICotisationRepository, CotisationRepository>();
builder.Services.AddScoped<INominationRepository, NominationRepository>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<IContentRepository, ContentRepository>();
builder.Services.AddScoped<IWorkflowRepository, WorkflowRepository>();
builder.Services.AddScoped<IMotCommissaireRepository, MotCommissaireRepository>();
builder.Services.AddScoped<ICompetenceRepository, CompetenceRepository>();
builder.Services.AddScoped<IDemandeRgpdRepository, DemandeRgpdRepository>();
builder.Services.AddScoped<IParentRepository, ParentRepository>();
builder.Services.AddScoped<IAscciStatusRepository, AscciStatusRepository>();
builder.Services.AddScoped<IFormationRepository, FormationRepository>();
builder.Services.AddScoped<IInscriptionFormationRepository, InscriptionFormationRepository>();



// DI - Security + Services
builder.Services.AddSingleton<IPasswordService, PasswordService>();
builder.Services.AddSingleton<IMfaService, MfaService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<TicketService>();
builder.Services.AddScoped<WorkflowService>();
builder.Services.AddScoped<DocumentService>();
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddScoped<IFileStorageService, FileStorageService>();
builder.Services.AddScoped<MangoTaikaDistrict.Infrastructure.Services.IExcelImportService, MangoTaikaDistrict.Infrastructure.Services.ExcelImportService>();

var app = builder.Build();

// Seed (roles/admin/circuit)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var password = scope.ServiceProvider.GetRequiredService<IPasswordService>();
    await DbSeeder.SeedAsync(db, password); // <- signature modifi�e en Livraison 3
}

app.UseStaticFiles();
app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// Routes pour les areas (doit �tre avant la route par d�faut)
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

// Route par d�faut
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
