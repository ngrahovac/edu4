using System.Reflection;
using Peer.Domain.Contributors;

namespace Peer.API.Utils;

public static class HatParametersExtractor
{
    public static HatType? GetHatTypeFromQueryString(IQueryCollection query)
    {
        var hatTypeQueryParamExists = query.ContainsKey("hatType");

        if (!hatTypeQueryParamExists)
        {
            return null;
        }

        var hatTypeQueryParams = query["hatType"];

        if (hatTypeQueryParams.Count > 1)
        {
            throw new InvalidOperationException("Expected one hat type value in the query string");
        }

        var hatTypeQueryParam = hatTypeQueryParams[0];

        var hatTypeParamIsValid = Enum.GetNames(typeof(HatType))
            .ToList()
            .Any(ht => hatTypeQueryParam.Equals(ht, StringComparison.Ordinal));

        return hatTypeParamIsValid ?
            (HatType)Enum.Parse(typeof(HatType), hatTypeQueryParam) :
            throw new InvalidOperationException("Supplied invalid hat type in the query string");
    }

    public static Dictionary<string, object> ExtractHatParametersFromQueryString(HatType hatType, IQueryCollection query)
    {
        try
        {
            var hatParameters = new Dictionary<string, object>();

            var hatClass = Assembly.GetAssembly(typeof(Hat))
                ?.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(Hat)))
                .Single(t => t.Name.StartsWith(hatType.ToString(), StringComparison.OrdinalIgnoreCase));

            var hatProperties = hatClass!.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(p => !p.Name.Equals(nameof(Hat.Type), StringComparison.OrdinalIgnoreCase));

            foreach (var hatProperty in hatProperties)
            {
                hatParameters[hatProperty.Name] = query[hatProperty.Name][0];
            }

            return hatParameters;
        }
        catch (Exception ex)
        {
            throw new ArgumentException("Error extracting hat parameters from query string", ex);
        }
    }
}
