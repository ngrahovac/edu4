using edu4.Application;
using Microsoft.Extensions.DependencyInjection;

namespace edu4.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        MongoDBSetupUtils.RegisterClassMaps();

        services.AddScoped<IUsersRepository, MongoDbUsersRepository>();

        return services;
    }
}
