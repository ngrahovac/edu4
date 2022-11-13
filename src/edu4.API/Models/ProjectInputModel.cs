using System.ComponentModel.DataAnnotations;

namespace edu4.API.Models;

#nullable disable
public class ProjectInputModel
{
    [Required]
    public string Title { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public List<PositionInputModel> Positions { get; set; }
}
#nullable restore
