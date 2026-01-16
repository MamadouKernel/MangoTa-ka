using System.ComponentModel.DataAnnotations;

namespace MangoTaikaDistrict.Models.Auth;

public class MfaVerifyVm
{
    [Required(ErrorMessage = "Le code est obligatoire")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "Le code doit contenir 6 chiffres")]
    [Display(Name = "Code de vérification")]
    public string Code { get; set; } = default!;
}
