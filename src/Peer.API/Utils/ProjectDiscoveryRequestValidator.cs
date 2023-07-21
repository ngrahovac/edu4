using System.Reflection;
using MongoDB.Driver;
using Peer.Domain.Contributors;

namespace Peer.API.Utils;

public static class ProjectDiscoveryRequestValidator
{
    // if a hatType is specified, enforces all necessary hat parameters are specified as well
    public static bool RequestIsValid(IQueryCollection query)
    {
        var hatType = HatParametersExtractor.GetHatTypeFromQueryString(query);

        if (hatType is not null && !HatQueryParamsForTheGivenHatTypeAreValid((HatType)hatType, query))
        {
            return false;
        }

        return true;
    }

    private static bool HatQueryParamsForTheGivenHatTypeAreValid(HatType hatType, IQueryCollection query)
    {
        var hatClass = Assembly.GetAssembly(typeof(Hat))
            ?.GetTypes()
            .Where(t => t.IsSubclassOf(typeof(Hat)))
            .Single(t => t.Name.StartsWith(hatType.ToString(), StringComparison.OrdinalIgnoreCase));

        var hatProperties = hatClass!.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(p => !p.Name.Equals(nameof(Hat.Type), StringComparison.OrdinalIgnoreCase));

        foreach (var hatProperty in hatProperties)
        {
            if (!query.ContainsKey(hatProperty.Name))
            {
                return false;
            }
            else if (query[hatProperty.Name].Count > 1)
            {
                return false;
            }
        }

        return true;
    }
}
