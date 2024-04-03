using Microsoft.Extensions.Configuration;

namespace Common.DbEventStore.Helpers;

public static class ConfigurationHelper
{
    public static T GetConfiguration<T>(this IConfiguration configuration, string sectionName) where T : class
    {
        var settings = configuration.GetSection(sectionName).Get<T>();
        if (settings is null)
        {
            throw new ArgumentNullException(sectionName, $"Configuration section '{sectionName}' is missing.");
        }

        return settings;
    }
}
