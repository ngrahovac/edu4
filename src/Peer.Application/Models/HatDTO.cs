using Peer.Application.Utils;
using Peer.Domain.Contributors;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Peer.Application.Models;

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

        try
        {
            hat = (Hat?)JsonSerializer.Deserialize(jsonString, type, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            });

            return hat;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error deserializing a hat", ex);
        }
    }

    public static HatDTO FromHat(Hat hat)
    {
        var type = hat.Type;

        var parameters = new Dictionary<string, object>();
        var properties = hat.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

        foreach (var property in properties)
        {
            parameters.Add(property.Name.ToCamelCase(), property.GetValue(hat)!);
        }


        var hatDto = new HatDTO(type, parameters);

        return hatDto;
    }
}
