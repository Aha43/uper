using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Uper.Backend.Repository.Turso;

namespace Uper.Backend.IntegrationTest.Turso.Tools;

public static class Configuration
{
    public static IServiceCollection ConfigureServicesForIntegrationTest(this IServiceCollection services)
    {
        // Build configuration to include user secrets
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<TursoRepositoryTests>() // Use the test class to locate the secrets file
            .Build();

        services.AddTursoRepository(configuration);

        services.AddAuthentication("TestScheme")
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestScheme", options => { });

        return services;
    }
}
