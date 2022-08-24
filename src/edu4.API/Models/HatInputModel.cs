using System.ComponentModel.DataAnnotations;

namespace edu4.API.Models;

public class HatInputModel
{
    [Required]
    public string? Type { get; set; }

    public Dictionary<string, object>? Parameters { get; set; }
}
