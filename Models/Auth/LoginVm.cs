using System.ComponentModel.DataAnnotations;

namespace MangoTaikaDistrict.Models.Auth;

public class LoginVm
{
    [Required]
    [Display(Name = "Téléphone")]
    public string Telephone { get; set; } = default!;

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Mot de passe")]
    public string Password { get; set; } = default!;
}
