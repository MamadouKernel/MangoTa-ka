# 📑 Conformité Cahier des Charges - MangoTaikaDistrict

**Date d'analyse** : 16 janvier 2026  
**Version Cahier des Charges** : 1.0  
**Date du document** : 13/01/2026  
**Statut projet** : En développement  

---

## 📊 Vue d'ensemble de la conformité

| Section | Complétion | Statut |
|---------|------------|--------|
| **2.1 Portail d'informations Générales** | 30% | ⚠️ Partiel |
| **2.2 Base de données district** | 75% | ⚠️ Partiel |
| **2.3 Automatisation Gestion Administrative** | 80% | ✅ Bien avancé |
| **2.4 Historique & Reporting** | 95% | ✅ Presque complet |
| **2.5 LMS (Learning Management System)** | 0% | ❌ Non implémenté |
| **2.6 Gestion du centre support** | 90% | ✅ Presque complet |
| **3. Exigences techniques** | 70% | ⚠️ Partiel |
| **4. Utilisateurs & rôles** | 85% | ✅ Bien avancé |
| **7. Critères de validation** | 85% | ✅ Bien avancé |

**Complétion globale estimée : ~65%**

---

## 1. Contexte & objectifs

### Objectifs principaux ✅

- ✅ Unicité, intégrité et fiabilité des données de base du district
- ✅ Digitalisation de l'administration (adressée par lot)
- ✅ Tableaux de bord adaptés pour le pilotage et la vue d'ensemble instantanée
- ⚠️ Opportunités sociales et lucratives (modulaire AGR) - **Partiel**
- ✅ Gestion support par Ticketing sur incidents et requêtes

---

## 2. Périmètre fonctionnel

### 2.1 Portail d'informations Générales ⚠️ (30% complet)

| Fonctionnalité | Statut | Détails |
|----------------|--------|---------|
| **Mot du commissaire** | ❌ **MANQUANT** | Pas de gestion du mot du commissaire avec photo, actualisable annuellement |
| **Bannière pleine page** | ❌ **MANQUANT** | Pas de bannière avec image d'actualité sur la page d'accueil |
| **Galerie d'images** | ✅ **FAIT** | Contrôleur `GalleryController` existant, mais pas de lecture seule restreinte |
| **Vue Maps GPS** | ❌ **MANQUANT** | Pas de carte Maps avec positions GPS des groupes et infos au survol |
| **Livre d'or numérique** | ✅ **PARTIEL** | `GuestbookController` existe avec modération, mais **pas de pages préremplies** avec images d'anciens commissaires/CG/CAD |
| **Contactez-nous** | ❌ **MANQUANT** | Pas de formulaire pour envoi de mail à `contact@mangotaika.ci` |
| **Avis, commentaires ou suggestions** | ❌ **MANQUANT** | Pas de formulaire séparé pour envoi de mail à `contact@mangotaika.ci` |
| **S'enregistrer** | ❌ **MANQUANT** | Pas de page d'enregistrement public avec numéro de téléphone, nom, prénoms, rôle + validation admin |
| **Se connecter** | ✅ **FAIT** | Connexion par téléphone + mot de passe (⚠️ **MFA manquant**) |

**Actions requises :**
1. Créer une entité `MotCommissaire` avec photo, texte, année, dates
2. Ajouter gestion bannière dans `Actualite` ou créer entité dédiée
3. Intégrer Google Maps ou Leaflet pour affichage GPS des groupes
4. Créer pages préremplies du livre d'or (images + textes statiques)
5. Créer formulaires de contact (avec envoi email)
6. Créer page d'enregistrement public avec workflow de validation admin
7. Implémenter MFA (Multi-Factor Authentication)

---

### 2.2 Base de données district ⚠️ (75% complet)

#### Champs Scout requis vs implémentés

| Champ requis | Statut | Détails |
|--------------|--------|---------|
| ID généré | ✅ **FAIT** | `Guid Id` |
| Groupe Scoute | ✅ **FAIT** | `GroupeId`, `Groupe` |
| Matricule | ✅ **FAIT** | `Matricule` |
| Numéro de carte | ⚠️ **PARTIEL** | `CarteScout` existe mais pas directement dans `Scout` |
| Assurance annuelle | ⚠️ **PARTIEL** | `Assurance` existe mais pas booléen direct |
| Nom | ✅ **FAIT** | `Nom` |
| Prénoms | ✅ **FAIT** | `Prenoms` |
| Fonction Scoute (année en cours) | ✅ **FAIT** | Via `Nomination` avec `DateDebut`/`DateFin` |
| Date et lieu de naissance | ⚠️ **PARTIEL** | `DateNaissance` existe mais **pas de lieu** |
| Adresse géographique (GPS) | ❌ **MANQUANT** | Pas de champs `Adresse`, `GpsLat`, `GpsLng` dans `Scout` |
| Chef (booléen) | ⚠️ **PARTIEL** | Peut être dérivé de `Nomination` mais pas de champ direct |
| **Compétences Scoutes** | ❌ **MANQUANT** | Pas de champ ni de table dédiée |
| **Compétences Académiques** | ❌ **MANQUANT** | Pas de champ ni de table dédiée |
| **Autres Compétences** | ❌ **MANQUANT** | Pas de champ ni de table dédiée |

**Actions requises :**
1. Ajouter `LieuNaissance` à l'entité `Scout`
2. Ajouter `Adresse`, `GpsLat`, `GpsLng` à l'entité `Scout`
3. Créer une entité `CompetenceScout` ou table de compétences avec catégories (Scoutes, Académiques, Autres)
4. Lier compétences aux scouts via relation many-to-many
5. Créer champ `IsChef` calculé ou booléen dans `Scout`

---

### 2.3 Automatisation Gestion Administrative ✅ (80% complet)

| Fonctionnalité | Statut | Détails |
|----------------|--------|---------|
| **Vérification statut ASCCI** | ❌ **MANQUANT** | Pas d'intégration ASCCI Status Checker |
| **Redirection SYGESCA** | ❌ **MANQUANT** | Pas de lien/redirection vers `https://adhesion.scoutascci.org/` |
| **Demande d'autorisation activités** | ✅ **FAIT** | `DemandeAutorisation`, `CircuitValidation`, workflow complet avec tracking |
| **Création en ligne d'un nouveau groupe** | ✅ **FAIT** | Via `GroupesController` (admin uniquement, peut être ouvert au public) |

**Actions requises :**
1. Intégrer API ou service ASCCI Status Checker
2. Ajouter bouton/lien vers SYGESCA dans l'interface
3. Ouvrir création de groupe au public avec workflow de validation (optionnel selon besoin)

---

### 2.4 Historique & Reporting ✅ (95% complet)

| Fonctionnalité | Statut | Détails |
|----------------|--------|---------|
| **Consultation historique activités** | ✅ **FAIT** | Via `Activite`, `DemandeAutorisation`, `Validation` |
| **Évolution scout** | ⚠️ **PARTIEL** | Historique via `AuditLog` général, pas spécifique scout |
| **Suivi académique** | ❌ **MANQUANT** | Pas de suivi académique dédié (nécessite compétences académiques) |
| **Reportings et tableaux de bord modulables** | ✅ **FAIT** | Dashboard avec graphiques, exports Excel/CSV/PDF |

**Actions requises :**
1. Créer vue dédiée "Historique scout" avec évolution complète
2. Implémenter suivi académique après ajout des compétences académiques

---

### 2.5 LMS (Learning Management System) ❌ (0% complet)

| Fonctionnalité | Statut | Détails |
|----------------|--------|---------|
| **Plateforme intégrée d'apprentissage en ligne** | ❌ **MANQUANT** | Aucune implémentation LMS |
| **Type MoodleCloud** | ❌ **MANQUANT** | Pas d'intégration LMS externe ou interne |

**Actions requises :**
1. **Décision architecturale** : Intégration externe (MoodleCloud) ou développement interne
2. Si intégration externe : Créer pont SSO et redirection
3. Si développement interne : Créer module complet LMS (cours, leçons, quiz, certificats, etc.)
4. Gestion des inscriptions aux cours par scout/chef
5. Suivi de progression

**Estimation : Grande fonctionnalité - nécessite planification dédiée**

---

### 2.6 Gestion du centre support ✅ (90% complet)

| Fonctionnalité | Statut | Détails |
|----------------|--------|---------|
| **Gestion de tickets incidents** | ✅ **FAIT** | `Ticket`, `TicketMessage`, `TicketService` complets |
| **Gestion de tickets requêtes** | ✅ **FAIT** | Types de tickets avec priorités |
| **Suivi et résolution** | ✅ **FAIT** | Statuts, assignation, historique |

**Actions requises :**
1. Améliorer interface utilisateur tickets (peut être fait dans le futur)
2. Ajouter notifications email (optionnel)

---

## 3. Exigences techniques ⚠️ (70% complet)

### Technologies

| Exigence | Statut | Détails |
|----------|--------|---------|
| **Performance** | ⚠️ **PARTIEL** | Non mesuré formellement, pas d'optimisations spécifiques documentées |
| **Sécurité** | ✅ **FAIT** | HTTPS, hashage mots de passe (PBKDF2), sessions sécurisées |
| **Gestion des rôles** | ✅ **FAIT** | ADMIN, GESTIONNAIRE, SUPERVISEUR, SCOUT, PARENT, CONSULTANT |
| **Connexion HTTPS (TLS 1.2+)** | ✅ **FAIT** | Configuré dans `launchSettings.json` |
| **Disponibilité 99.9%** | ❌ **N/A** | Dépend de l'hébergement, pas dans le code |
| **RGPD / Loi ivoirienne** | ⚠️ **PARTIEL** | Entité `Consentement` existe mais **pas d'intégration dans les formulaires** |

#### RGPD / Loi ivoirienne - Actions requises

1. **Déclaration préalable** : À faire auprès de l'ARTCI (hors code)
2. **Consentement** : 
   - ✅ Entité `Consentement` créée
   - ❌ **Pas de checkbox "J'accepte..." dans les formulaires**
   - ❌ Pas de versionning des consentements
   - ❌ Pas de gestion des droits (accès, rectification, opposition, oubli)
3. **Finalité** : À documenter (hors code)
4. **Sécurité** : ✅ Déjà en place (hashage, HTTPS)
5. **Droits des citoyens** :
   - ❌ Pas d'interface pour consulter ses données
   - ❌ Pas d'interface pour rectifier
   - ❌ Pas d'interface pour demander suppression (droit à l'oubli)
   - ❌ Pas d'interface pour opposition

**Actions requises :**
1. Ajouter checkbox consentement RGPD dans tous les formulaires publics (inscription, contact, livre d'or)
2. Enregistrer consentement avec version, IP, UserAgent, timestamp
3. Créer interface "Mes données" pour les utilisateurs connectés :
   - Voir ses données personnelles
   - Demander rectification
   - Demander suppression (droit à l'oubli)
   - Opposition au traitement
4. Créer processus admin pour traiter les demandes de droits

---

## 4. Utilisateurs & rôles ✅ (85% complet)

| Rôle | Statut | Détails |
|------|--------|---------|
| **Administrateur** | ✅ **FAIT** | Vision et gestion globale, toutes permissions |
| **Gestionnaire** | ✅ **FAIT** | Validation requêtes, gestion centre support |
| **Scout** | ⚠️ **PARTIEL** | Contributeur sur sa fiche (⚠️ **pas implémenté**), demandeur activations (✅ fait via workflow) |
| **Parent / Tuteur** | ⚠️ **PARTIEL** | Entités `Parent`, `ParentScout` existent mais **pas d'interface dédiée** |
| **Consultant** | ✅ **FAIT** | Rôle existe, permissions à définir |
| **Superviseur** | ✅ **FAIT** | Supervision dans workflow de validation |

**Actions requises :**
1. Créer interface "Mon profil" pour scout (modification de sa fiche)
2. Créer interface "Mes enfants" pour parent (consultation des fiches enfants)
3. Définir et implémenter permissions précises pour Consultant

---

## 5. Logo & Charte Graphique ⚠️ (Partiel)

- ✅ Logo présent dans `wwwroot/images/logo.png`
- ✅ Bootstrap intégré avec styles modernes
- ⚠️ Charte graphique complète à appliquer selon documents fournis

---

## 6. Livrables attendus ✅

| Livrable | Statut |
|----------|--------|
| **Plateforme Web fonctionnelle (multi-supports)** | ✅ **FAIT** | Responsive Bootstrap |
| **Tableau de bord temps réel + alertes** | ✅ **FAIT** | Dashboard avec graphiques (⚠️ alertes basiques) |
| **Historique et reporting** | ✅ **FAIT** | Exports Excel/CSV/PDF |
| **Documentation technique** | ⚠️ **PARTIEL** | Code commenté, pas de doc complète |
| **Documentation utilisateur** | ❌ **MANQUANT** | À créer |
| **Formation** | ❌ **N/A** | Hors code |

---

## 7. Critères de validation ⚠️ (85% complet)

| Critère | Statut | Détails |
|---------|--------|---------|
| **Fonctionne sur PC, tablette et smartphone** | ✅ **FAIT** | Bootstrap responsive |
| **Visualisation temps réel** | ⚠️ **PARTIEL** | Dashboard temps réel ✅, mais pas de visualisation "position et vitesse de chaque camion" (❌ hors périmètre scout) |

⚠️ **Note** : Le critère "visualisation position et vitesse de chaque camion" semble être une erreur de copier-coller d'un autre projet. À confirmer avec le client.

---

## 📋 PLAN D'ACTION PRIORITAIRE

### 🔴 Phase 1 : Fonctionnalités Critiques Manquantes (Urgent)

1. **Portail Public - Fonctionnalités de base** (Semaine 1-2)
   - [ ] Mot du commissaire avec photo (CRUD admin + affichage public)
   - [ ] Bannière pleine page avec image d'actualité
   - [ ] Formulaire "Contactez-nous" (email)
   - [ ] Formulaire "Avis, commentaires ou suggestions" (email)
   - [ ] Intégration consentement RGPD dans tous les formulaires

2. **Enregistrement Public + MFA** (Semaine 2-3)
   - [ ] Page d'enregistrement public (téléphone, nom, prénoms, rôle)
   - [ ] Workflow de validation admin pour nouveaux utilisateurs
   - [ ] Implémentation MFA (SMS ou Email TOTP)

3. **Base de données Scout - Champs manquants** (Semaine 3-4)
   - [ ] Ajouter `LieuNaissance`, `Adresse`, `GpsLat`, `GpsLng` à `Scout`
   - [ ] Créer système de compétences (Scoutes, Académiques, Autres)
   - [ ] Migration base de données

### 🟡 Phase 2 : Améliorations Portail Public (Semaine 4-6)

4. **Maps GPS des Groupes** (Semaine 4-5)
   - [ ] Intégrer Google Maps ou Leaflet
   - [ ] Afficher positions GPS des groupes
   - [ ] Infos au survol (CG, Adjoints, Chefs d'unité par branche)

5. **Livre d'Or - Pages préremplies** (Semaine 5)
   - [ ] Créer pages statiques avec images anciens commissaires/CG/CAD
   - [ ] Intégrer dans affichage livre d'or

6. **Galerie - Restrictions** (Semaine 5)
   - [ ] Restreindre galerie en lecture seule sur portail public

### 🟢 Phase 3 : Fonctionnalités Avancées (Semaine 6-8)

7. **Intégrations ASCCI** (Semaine 6)
   - [ ] Intégrer ASCCI Status Checker
   - [ ] Ajouter redirection/lien vers SYGESCA

8. **Interface Parent** (Semaine 7)
   - [ ] Créer vue "Mes enfants" pour parents
   - [ ] Permissions et accès limités

9. **Interface Scout - Mon profil** (Semaine 7)
   - [ ] Permettre modification de sa propre fiche
   - [ ] Restrictions selon rôle

10. **RGPD - Droits des citoyens** (Semaine 8)
    - [ ] Interface "Mes données" pour utilisateurs
    - [ ] Processus admin pour gestion des demandes (accès, rectification, suppression, opposition)

### 🔵 Phase 4 : LMS (Semaine 9-12+)

11. **Décision architecturale LMS** (Semaine 9)
    - [ ] Choisir : Intégration externe (MoodleCloud) ou développement interne
    - [ ] Planification détaillée

12. **Implémentation LMS** (Semaine 10-12+)
    - [ ] Selon décision architecturale

### 📝 Phase 5 : Documentation (Parallèle)

13. **Documentation**
    - [ ] Documentation technique complète
    - [ ] Documentation utilisateur
    - [ ] Guide d'installation et déploiement

---

## 📊 TAUX DE COMPLÉTION DÉTAILLÉ

| Catégorie | Complétion | Priorité |
|-----------|------------|----------|
| Portail d'informations Générales | 30% | 🔴 Haute |
| Base de données district | 75% | 🟡 Moyenne |
| Automatisation Gestion Administrative | 80% | 🟢 Basse |
| Historique & Reporting | 95% | 🟢 Basse |
| LMS | 0% | 🔵 Très basse (décision requise) |
| Gestion du centre support | 90% | 🟢 Basse |
| Exigences techniques | 70% | 🟡 Moyenne |
| Utilisateurs & rôles | 85% | 🟡 Moyenne |
| Critères de validation | 85% | 🟢 Basse |

**Complétion globale estimée : ~65%**

---

## 🎯 FOCUS IMMÉDIAT RECOMMANDÉ

1. **Portail Public** : Mettre en place les fonctionnalités de base (mot commissaire, formulaires contact, enregistrement)
2. **RGPD** : Intégrer consentement dans tous les formulaires
3. **Base de données** : Compléter les champs manquants dans `Scout` (compétences, GPS, lieu naissance)
4. **MFA** : Implémenter authentification à deux facteurs

---

**Document généré le** : 16 janvier 2026  
**Dernière mise à jour** : 16 janvier 2026  
**Basé sur** : Cahier des charges version 1.0 du 13/01/2026