using System.ComponentModel.DataAnnotations;

namespace Peer.API.Models.Input;

#nullable disable
public class ProjectInputModel
{
    [Required]
    public string Title { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    [MinLength(1)]
    public List<PositionInputModel> Positions { get; set; }
}
#nullable restore
