using Peer.API.Utils;
using Peer.Domain.Contributors;

namespace Peer.API.Models.Display;

public class HatDisplayModel
{
    public string Type { get; set; }
    public Dictionary<string, object> Parameters { get; set; } = new();


    public HatDisplayModel(Hat hat)
    {
        Type = hat.GetType().Name[0..^3];

        hat
            .GetType()
            .GetProperties()
            .ToList()
            .ForEach(prop => Parameters.Add(prop.Name.ToCamelCase(), prop.GetValue(hat)!));
    }
}
