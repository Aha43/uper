using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Uper.Backend.Common;
using Uper.Backend.Domain.Abstraction.Repository;
using Uper.Backend.Repository.Common;

namespace Uper.Backend.Repository.Turso;

public static class Services
{
    public static IServiceCollection AddTursoRepository(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddTursoHttpClient(configuration)
            .AddScoped<IRepository, TursoRepository>()
            .AddCommonRepositoryServices();
    }

    private static IServiceCollection AddTursoHttpClient(this IServiceCollection services, IConfiguration configuration)
    {
        var tursoClientConfiguration = configuration.GetRequiredAs<TursoClientConfiguration>();
        services.AddSingleton(tursoClientConfiguration);

        services.AddHttpClient<TursoClient>(client =>
        {
            client.BaseAddress = new Uri(tursoClientConfiguration.BaseUrl);
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", tursoClientConfiguration.BearerToken);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        });

        return services;
    }
}

public sealed class TursoClientConfiguration
{
    public string BaseUrl { get; set; } = string.Empty;
    public string BearerToken { get; set; } = string.Empty;
}
