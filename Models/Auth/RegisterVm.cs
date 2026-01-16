using System.ComponentModel.DataAnnotations;

namespace MangoTaikaDistrict.Models.Auth;

public class RegisterVm
{
    [Required(ErrorMessage = "Le numéro de téléphone est obligatoire")]
    [Phone(ErrorMessage = "Format de téléphone invalide")]
    [Display(Name = "Numéro de téléphone")]
    public string Telephone { get; set; } = default!;

    [Required(ErrorMessage = "Le nom est obligatoire")]
    [Display(Name = "Nom")]
    public string Nom { get; set; } = default!;

    [Required(ErrorMessage = "Les prénoms sont obligatoires")]
    [Display(Name = "Prénoms")]
    public string Prenoms { get; set; } = default!;

    [EmailAddress(ErrorMessage = "Format d'email invalide")]
    [Display(Name = "Email (optionnel)")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Le rôle est obligatoire")]
    [Display(Name = "Rôle")]
    public string Role { get; set; } = default!;

    [Required(ErrorMessage = "Le mot de passe est obligatoire")]
    [StringLength(100, ErrorMessage = "Le mot de passe doit contenir au moins {2} caractères.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Mot de passe")]
    public string Password { get; set; } = default!;

    [DataType(DataType.Password)]
    [Display(Name = "Confirmer le mot de passe")]
    [Compare("Password", ErrorMessage = "Les mots de passe ne correspondent pas.")]
    public string ConfirmPassword { get; set; } = default!;

    [Required(ErrorMessage = "Vous devez accepter le consentement")]
    [Display(Name = "J'accepte le traitement de mes données conformément à la réglementation en vigueur")]
    public bool Consentement { get; set; }
}
