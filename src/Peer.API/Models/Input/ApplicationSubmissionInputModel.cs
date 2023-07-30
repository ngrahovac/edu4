using System.ComponentModel.DataAnnotations;

namespace Peer.API.Models.Input;

#nullable disable
public class ApplicationSubmissionInputModel
{
    [Required]
    public Guid ProjectId { get; set; }

    [Required]
    public Guid PositionId { get; set; }
}

#nullable restore
