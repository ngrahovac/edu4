using System.ComponentModel.DataAnnotations;

namespace Peer.API.Models.Input;

#nullable disable
public class PositionInputModel
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public HatInputModel Requirements { get; set; }
}
#nullable restore
