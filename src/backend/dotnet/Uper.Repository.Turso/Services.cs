using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using Uper.Domain.Abstraction.Repository;
using Uper.Repository.Common;

namespace Uper.Repository.Turso;

public static class Services
{
    public static IServiceCollection AddTursoRepository(this IServiceCollection services)
    {
        return services
            .AddTursoHttpClient()
            .AddScoped<IRepository, TursoRepository>()
            .AddCommonRepositoryServices();
    }

    private static IServiceCollection AddTursoHttpClient(this IServiceCollection services)
    {
        services.AddHttpClient<TursoClient>(client =>
        {
            client.BaseAddress = new Uri("https://your-turso-instance.turso.io");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "your-api-key");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        });

        return services;
    }
}
