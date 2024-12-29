using Microsoft.Extensions.DependencyInjection;
using Uper.Domain.Abstraction.Repository.Common;

namespace Uper.Repository.Common;

public  static class Services
{
    public static IServiceCollection AddCommonRepositoryServices(this IServiceCollection services)
    {
        return services.AddScoped<ISqlGenerator, SqlGenerator>();
    }
}
