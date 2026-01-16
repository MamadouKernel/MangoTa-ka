# Architecture du Projet Mango Taika District

## Structure des Dossiers

```
MangoTaikaDistrict/
│
├── Application/                    # Couche Application
│   ├── Interfaces/                # Interfaces des services applicatifs
│   │   ├── IAuthService.cs
│   │   └── IEmailService.cs
│   └── Services/                  # Services applicatifs
│       ├── AuthService.cs
│       ├── DocumentService.cs
│       ├── EmailService.cs
│       ├── TicketService.cs
│       └── WorkflowService.cs
│
├── Controllers/                    # Contrôleurs MVC
│   ├── Account/                   # Authentification et compte utilisateur
│   ├── Admin/                     # Administration (Area)
│   ├── Lms/                       # Learning Management System
│   ├── Public/                    # Pages publiques
│   └── BaseController.cs          # Contrôleur de base
│
├── Domain/                        # Couche Domaine
│   ├── Entities/                  # Entités du domaine métier
│   │   ├── Utilisateur.cs
│   │   ├── Scout.cs
│   │   ├── Groupe.cs
│   │   ├── Cotisation.cs
│   │   ├── Nomination.cs
│   │   ├── Activite.cs
│   │   └── ... (38 entités)
│   └── Enums/                     # Énumérations
│       ├── RoleCode.cs
│       ├── StatutCotisation.cs
│       ├── StatutDemande.cs
│       └── ... (15 enums)
│
├── Infrastructure/                 # Couche Infrastructure
│   ├── Data/                      # Accès aux données
│   │   ├── AppDbContext.cs       # Contexte EF Core
│   │   ├── Migrations/           # Migrations EF Core
│   │   └── Seed/                  # Données initiales
│   │       └── DbSeeder.cs
│   ├── Pdf/                       # Génération de PDF (QuestPDF)
│   │   ├── ScoutsOfficialPdfDocument.cs
│   │   ├── CotisationsOfficialPdfDocument.cs
│   │   └── ...
│   ├── Repositories/              # Implémentations des repositories
│   │   ├── IUtilisateurRepository.cs
│   │   ├── UtilisateurRepository.cs
│   │   └── ... (32 repositories)
│   ├── Security/                  # Services de sécurité
│   │   ├── IPasswordService.cs
│   │   ├── PasswordService.cs
│   │   ├── IMfaService.cs
│   │   └── MfaService.cs
│   ├── Services/                  # Services infrastructure
│   │   ├── IExcelImportService.cs
│   │   └── ExcelImportService.cs
│   └── Storage/                    # Stockage de fichiers
│       ├── IFileStorageService.cs
│       └── FileStorageService.cs
│
├── Models/                         # ViewModels et DTOs
│   ├── Auth/                      # Modèles d'authentification
│   │   ├── LoginVm.cs
│   │   ├── RegisterVm.cs
│   │   └── MfaVerifyVm.cs
│   └── ErrorViewModel.cs
│
├── Views/                          # Vues Razor
│   ├── Account/                    # Vues d'authentification
│   ├── Admin/                     # Vues d'administration
│   ├── Lms/                       # Vues LMS
│   ├── Public/                    # Vues publiques
│   └── Shared/                    # Layouts et composants partagés
│
├── wwwroot/                        # Fichiers statiques
│   ├── assets/                    # Assets (logo, etc.)
│   ├── css/                       # Feuilles de style
│   ├── js/                        # Scripts JavaScript
│   ├── lib/                       # Bibliothèques tierces
│   └── uploads/                   # Fichiers uploadés
│
├── docs/                           # Documentation
│   ├── README.md                  # Index de la documentation
│   ├── ACCES.md
│   ├── CAHIER_CHARGES_COMPLIANCE.md
│   ├── CHARTE_GRAPHIQUE_APPLIQUEE.md
│   └── ...
│
├── Program.cs                      # Point d'entrée de l'application
├── appsettings.json               # Configuration
└── MangoTaikaDistrict.csproj      # Fichier projet
```

## Principes d'Architecture

### Architecture en Couches (Layered Architecture)

1. **Domain** : Entités métier et enums (pas de dépendances)
2. **Application** : Services applicatifs et interfaces (dépend de Domain)
3. **Infrastructure** : Accès données, repositories, sécurité (dépend de Domain et Application)
4. **Controllers** : Contrôleurs MVC (dépend de Application et Infrastructure)
5. **Views** : Vues Razor (dépend de Controllers)
6. **Models** : ViewModels (dépend de Domain)

### Patterns Utilisés

- **Repository Pattern** : Abstraction de l'accès aux données
- **Service Pattern** : Logique métier dans les services
- **Dependency Injection** : Injection de dépendances via Program.cs
- **Cookie Authentication** : Authentification par cookies (pas ASP.NET Identity)
- **Claims-based Authorization** : Gestion des rôles via Claims

### Technologies

- **ASP.NET Core MVC 8.0** : Framework web
- **Entity Framework Core 8.0** : ORM
- **PostgreSQL** : Base de données
- **QuestPDF** : Génération de PDF
- **ClosedXML** : Import/Export Excel
- **Otp.NET** : Authentification à deux facteurs (MFA)

## Conventions de Nommage

- **Dossiers** : PascalCase (ex: `Application`, `Infrastructure`)
- **Fichiers** : PascalCase.cs (ex: `AuthService.cs`)
- **Interfaces** : Préfixe `I` (ex: `IAuthService`)
- **Namespaces** : Correspondent aux dossiers (ex: `MangoTaikaDistrict.Application.Services`)

## Organisation des Fichiers

- ✅ Dossiers vides supprimés
- ✅ Documentation centralisée dans `docs/`
- ✅ Structure cohérente et maintenable
- ✅ Respect des conventions C# et .NET
