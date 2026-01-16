-- Script SQL pour corriger le mot de passe admin
-- Exécutez ce script dans pgAdmin ou psql

-- IMPORTANT: Ce script nécessite de générer un hash valide avec l'application
-- La meilleure solution est de redémarrer l'application qui corrigera automatiquement le hash

-- Alternative: Supprimer l'utilisateur pour qu'il soit recréé avec le bon hash
-- DELETE FROM public."UtilisateurRoles" WHERE "UtilisateurId" IN (SELECT "Id" FROM public."Utilisateurs" WHERE "Telephone" = '0100000000');
-- DELETE FROM public."Utilisateurs" WHERE "Telephone" = '0100000000';

-- Note: Le DbSeeder corrige maintenant automatiquement les placeholders au démarrage
