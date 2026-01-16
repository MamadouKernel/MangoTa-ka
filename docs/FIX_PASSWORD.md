# 🔧 Correction du Mot de Passe Admin

## Problème
Le `PasswordHash` dans la base de données contient un placeholder (`CHANGE_ME_WITH_HASH_IN_LIVRAISO...`) au lieu d'un vrai hash.

## ✅ Solution Automatique (Recommandée)

Le `DbSeeder` a été modifié pour **corriger automatiquement** les placeholders au démarrage de l'application.

**Il suffit de redémarrer l'application** et le hash sera automatiquement corrigé !

## 🔄 Solution Manuelle (Alternative)

Si vous préférez corriger manuellement dans pgAdmin :

### Option 1: Supprimer et recréer l'utilisateur

```sql
-- 1. Supprimer les rôles associés
DELETE FROM public."UtilisateurRoles" 
WHERE "UtilisateurId" IN (
    SELECT "Id" FROM public."Utilisateurs" 
    WHERE "Telephone" = '0100000000'
);

-- 2. Supprimer l'utilisateur
DELETE FROM public."Utilisateurs" 
WHERE "Telephone" = '0100000000';
```

Ensuite, **redémarrez l'application** et le `DbSeeder` créera automatiquement l'utilisateur avec le bon hash.

### Option 2: Générer un hash et le mettre à jour

1. Créez un petit script C# pour générer le hash :

```csharp
using MangoTaikaDistrict.Infrastructure.Security;

var passwordService = new PasswordService();
var hash = passwordService.Hash("Admin@2026");
Console.WriteLine(hash);
```

2. Copiez le hash généré et exécutez dans pgAdmin :

```sql
UPDATE public."Utilisateurs" 
SET "PasswordHash" = 'VOTRE_HASH_GENERE_ICI'
WHERE "Telephone" = '0100000000';
```

## 🚀 Solution la Plus Simple

**Redémarrez simplement l'application !** Le `DbSeeder` corrigera automatiquement le hash au démarrage.

---

**Identifiants après correction :**
- **Téléphone :** `0100000000`
- **Mot de passe :** `Admin@2026`
