namespace MangoTaikaDistrict.Domain.Enums;

public enum StatutInscription
{
    INSCRIT = 0,      // Inscrit mais pas encore commencé
    EN_COURS = 1,     // Formation en cours
    COMPLETE = 2,     // Formation complétée
    ABANDONNE = 3,    // Abandonnée
    SUSPENDU = 4      // Suspendue par l'admin
}
