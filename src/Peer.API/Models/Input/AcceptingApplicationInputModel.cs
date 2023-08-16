using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Peer.Domain.Applications;

namespace Peer.API.Models.Input;

#nullable disable
public class AcceptingApplicationInputModel
{
    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ApplicationStatus Status { get; set; }
}
#nullable restore
