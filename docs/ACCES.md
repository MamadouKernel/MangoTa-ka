# 🔐 Identifiants de Connexion - MangoTaikaDistrict

## 👤 Compte Administrateur

**Téléphone :** `0100000000`  
**Mot de passe :** `Admin@2026`  
**Rôle :** ADMIN  
**Email :** admin@mangotaika.local

---

## 📋 Rôles Disponibles

Le système dispose des rôles suivants :

- **ADMIN** - Administrateur (accès complet)
- **GESTIONNAIRE** - Gestionnaire
- **SUPERVISEUR** - Superviseur
- **SCOUT** - Scout
- **PARENT** - Parent
- **CONSULTANT** - Consultant

---

## 🚀 Accès à l'Application

1. **URL de connexion :** `/Account/Login`
2. **Après connexion :** Redirection automatique vers `/Admin/Dashboard`

---

## ⚙️ Configuration

- **Authentification :** Cookie-based
- **Durée de session :** 8 heures
- **Sliding expiration :** Activée

---

## 📝 Notes

- Le compte admin est créé automatiquement lors du premier démarrage de l'application (via `DbSeeder`)
- Si le compte existe déjà, il ne sera pas recréé
- Le mot de passe est hashé avec un service de hachage sécurisé

---

**⚠️ Important :** Changez le mot de passe par défaut en production !
