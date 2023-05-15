namespace Peer.API.Utils;

public static class StringExtensionMethods
{
    public static string ToCamelCase(this string value) =>
        value[0].ToString().ToLowerInvariant() + value[1..];
}
