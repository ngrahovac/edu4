using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Peer.Domain.Contributors;

namespace Peer.API.Models.Input;

#nullable disable
public class HatInputModel : IValidatableObject
{
    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public HatType Type { get; set; }

    [Required]
    public Dictionary<string, object> Parameters { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();
        var keys = Parameters.Keys;

        switch (Type)
        {
            case HatType.Student:
            {
                if (keys.Count > 2)
                {
                    results.Add(new ValidationResult("Too many hat parameters"));
                }
                else if (!keys.Contains(nameof(StudentHat.StudyField), StringComparer.OrdinalIgnoreCase))
                {
                    results.Add(new ValidationResult("Must specify a study field for the student"));
                }
                break;
            };
            case HatType.Academic:
            {
                if (keys.Count > 1)
                {
                    results.Add(new ValidationResult("Too many hat parameters"));
                }
                else if (!keys.Contains(nameof(AcademicHat.ResearchField), StringComparer.OrdinalIgnoreCase))
                {
                    results.Add(new ValidationResult("Must specify a research field for the academic"));
                }
                break;
            };
            default:
                throw new NotImplementedException();
        }

        return results;
    }
}
#nullable restore
