using System.ComponentModel.DataAnnotations;

namespace MangoTaikaDistrict.Models.Public;

public class ContactVm
{
    [Required(ErrorMessage = "Le nom est obligatoire")]
    [Display(Name = "Nom complet")]
    public string Nom { get; set; } = default!;

    [Required(ErrorMessage = "L'email est obligatoire")]
    [EmailAddress(ErrorMessage = "Format d'email invalide")]
    [Display(Name = "Email")]
    public string Email { get; set; } = default!;

    [Display(Name = "Téléphone")]
    public string? Telephone { get; set; }

    [Required(ErrorMessage = "Le message est obligatoire")]
    [Display(Name = "Message")]
    public string Message { get; set; } = default!;

    [Required(ErrorMessage = "Vous devez accepter le consentement")]
    [Display(Name = "J'accepte que mes données soient utilisées pour traiter ma demande")]
    public bool Consentement { get; set; }
}
