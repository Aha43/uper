using Microsoft.Extensions.Configuration;

namespace Uper.Backend.Common;

public static class ConfigurationExtension
{
    public static T? GetAs<T>(this IConfiguration configuration, string? name = null) where T : class
    {
        name ??= typeof(T).Name;
        var section = configuration.GetSection(name);
        return section?.Get<T>();
    }

    public static T GetRequiredAs<T>(this IConfiguration configuration, string? name = default) where T : class
    {
        var retVal = configuration.GetAs<T>(name) ?? throw new ArgumentException($"Configuration section named '{name ?? nameof(T)}' not found");
        return retVal;
    }
}
