using System.Text.Json;
using edu4.Domain.Users;

namespace edu4.API.Models;

public static class HatFactory
{
    public static Hat FromHatInputModel(HatInputModel model)
    {
        Hat? hat;
        var jsonString = JsonSerializer.Serialize(model.Parameters);
        var type = model.Type!.Equals("Academic", StringComparison.Ordinal) ? typeof(AcademicHat) :
                   model.Type.Equals("Student", StringComparison.Ordinal) ? typeof(StudentHat) :
                   throw new NotImplementedException();

        hat = (Hat?)JsonSerializer.Deserialize(
            jsonString,
            type,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ??
            throw new InvalidCastException();

        return hat;
    }
}
