namespace MangoTaikaDistrict.Domain.Enums;

/// <summary>
/// Branches scoutes de l'ASCCI avec leurs caractéristiques spécifiques
/// </summary>
public enum BrancheScout
{
    /// <summary>
    /// Oisillons (4-7 ans) - Couleur Bleu ciel
    /// </summary>
    OISILLONS = 1,

    /// <summary>
    /// Louveteaux (8-11 ans) - Couleur Jaune
    /// </summary>
    LOUVETEAUX = 2,

    /// <summary>
    /// Éclaireurs (12-14 ans) - Couleur Vert
    /// </summary>
    ECLAIREURS = 3,

    /// <summary>
    /// Cheminots (15-17 ans) - Couleur Orange - Innovation pédagogique ivoirienne
    /// </summary>
    CHEMINOTS = 4,

    /// <summary>
    /// Routiers (18-21 ans et +) - Couleur Rouge
    /// </summary>
    ROUTIERS = 5
}

public static class BrancheScoutExtensions
{
    public static string GetLibelle(this BrancheScout branche)
    {
        return branche switch
        {
            BrancheScout.OISILLONS => "Oisillons",
            BrancheScout.LOUVETEAUX => "Louveteaux",
            BrancheScout.ECLAIREURS => "Éclaireurs",
            BrancheScout.CHEMINOTS => "Cheminots",
            BrancheScout.ROUTIERS => "Routiers",
            _ => branche.ToString()
        };
    }

    public static string GetCouleur(this BrancheScout branche)
    {
        return branche switch
        {
            BrancheScout.OISILLONS => "#87CEEB", // Bleu ciel
            BrancheScout.LOUVETEAUX => "#FFD700", // Jaune
            BrancheScout.ECLAIREURS => "#228B22", // Vert
            BrancheScout.CHEMINOTS => "#FF8C00", // Orange
            BrancheScout.ROUTIERS => "#DC143C", // Rouge
            _ => "#808080" // Gris par défaut
        };
    }

    public static string GetTrancheAge(this BrancheScout branche)
    {
        return branche switch
        {
            BrancheScout.OISILLONS => "4-7 ans",
            BrancheScout.LOUVETEAUX => "8-11 ans",
            BrancheScout.ECLAIREURS => "12-14 ans",
            BrancheScout.CHEMINOTS => "15-17 ans",
            BrancheScout.ROUTIERS => "18-21 ans et +",
            _ => "N/A"
        };
    }

    public static string GetDescription(this BrancheScout branche)
    {
        return branche switch
        {
            BrancheScout.OISILLONS => "Initiation au jeu et à la vie en groupe",
            BrancheScout.LOUVETEAUX => "Apprentissage de la vie en 'Meute' basé sur l'imaginaire du Livre de la Jungle",
            BrancheScout.ECLAIREURS => "Vie en patrouille, apprentissage des techniques scoutes et de l'autonomie",
            BrancheScout.CHEMINOTS => "Branche unique inspirée des 'classes d'âge' des traditions ivoiriennes. Prépare l'adolescent à devenir un citoyen actif",
            BrancheScout.ROUTIERS => "Branche aînée axée sur le service à la communauté et l'engagement spirituel",
            _ => ""
        };
    }
}
