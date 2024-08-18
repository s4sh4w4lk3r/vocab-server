using System.Text.RegularExpressions;
using Throw;

namespace Vocab.WebApi.Extensions
{
    public static partial class ConfigurationExtensions
    {
        public static string[] GetCorsOrigins(this IConfiguration configuration)
        {
            string? corsOriginsStr = configuration.GetRequiredSection("Cors:Origins").Value;
            return !string.IsNullOrEmpty(corsOriginsStr) 
                ? UrlSplitterPattern().Matches(corsOriginsStr).Select(x => x.Value).ToArray() 
                : [];
        }

        public static string[] GetJwtValidIssuers(this IConfiguration configuration)
        {
            string validIssuersStr = configuration.GetRequiredSection("Auth:ValidIssuers").Value.ThrowIfNull().IfEmpty().IfWhiteSpace();
            return UrlSplitterPattern().Matches(validIssuersStr).Select(x => x.Value).ToArray();
        }

        [GeneratedRegex(@"((?>[\w:\/.]+))")]
        private static partial Regex UrlSplitterPattern();
    }
}
