using System.ComponentModel.DataAnnotations;

namespace edu4.API.Models;

public class UserSignupInputModel
{
    [Required]
    public string? AccountId { get; set; }

    [Required]
    public string? ContactEmail { get; set; }

    [Required]
    [MinLength(1)]
    public List<HatInputModel>? Hats { get; set; }
}
