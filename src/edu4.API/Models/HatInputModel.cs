using System.ComponentModel.DataAnnotations;

namespace edu4.API.Models;

#nullable disable
public class HatInputModel
{
    [Required]
    public string Type { get; set; }

    [Required]
    public Dictionary<string, object> Parameters { get; set; }
}
#nullable restore
