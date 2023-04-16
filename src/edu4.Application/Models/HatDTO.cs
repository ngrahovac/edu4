using edu4.Application.Utils;
using edu4.Domain.Users;
using System.Reflection;
using System.Text.Json;

namespace edu4.Application.Models;

public record HatDTO(HatType Type, Dictionary<string, object> Parameters)
{
    public static Hat ToHat(HatDTO model)
    {
        Hat? hat;
        var jsonString = JsonSerializer.Serialize(model.Parameters);
        var type = model.Type switch
        {
            HatType.Student => typeof(StudentHat),
            HatType.Academic => typeof(AcademicHat),
            _ => throw new NotImplementedException()
        };

        hat =
            (Hat?)JsonSerializer.Deserialize(jsonString, type, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ??
            throw new InvalidCastException();

        return hat;
    }

    public static HatDTO FromHat(Hat hat)
    {
        var type = hat.Type;

        var parameters = new Dictionary<string, object>();
        var properties = hat.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

        foreach(var property in properties)
        {
            parameters.Add(property.Name.ToCamelCase(), property.GetValue(hat)!);
        }


        var hatDto = new HatDTO(type, parameters);

        return hatDto;
    }
}
