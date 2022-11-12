using edu4.Domain.Users;
using System.Text.Json;

namespace edu4.Application.Services;

public record HatDTO(string Type, Dictionary<string, object> Parameters)
{
    public static Hat ToHat(HatDTO model)
    {
        Hat? hat;
        var jsonString = JsonSerializer.Serialize(model.Parameters);
        var type =
            model.Type.Equals("Academic", StringComparison.Ordinal) ? typeof(AcademicHat) :
            model.Type.Equals("Student", StringComparison.Ordinal) ? typeof(StudentHat) :
            throw new NotImplementedException();

        hat =
            (Hat?)JsonSerializer.Deserialize(jsonString, type, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ??
            throw new InvalidCastException();

        return hat;
    }
}
