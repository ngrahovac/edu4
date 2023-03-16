using System.ComponentModel.DataAnnotations;

namespace edu4.API.Models.Input;

public class UserSignupInputModel
{
    [Required]
    public string? ContactEmail { get; set; }

    [Required]
    public string? FullName { get; set; }

    [Required]
    [MinLength(1)]
    public List<HatInputModel>? Hats { get; set; }
}
