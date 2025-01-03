using Microsoft.Extensions.DependencyInjection;
using Uper.Backend.Domain.Abstraction.Repository.Common;

namespace Uper.Backend.Repository.Common;

public static class Services
{
    public static IServiceCollection AddCommonRepositoryServices(this IServiceCollection services)
    {
        return services.AddScoped<ISqlGenerator, SqlGenerator>();
    }
}
