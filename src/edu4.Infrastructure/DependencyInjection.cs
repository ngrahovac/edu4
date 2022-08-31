using edu4.Application;
using edu4.Application.External;
using Microsoft.Extensions.DependencyInjection;

namespace edu4.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        MongoDBSetupUtils.RegisterClassMaps();

        services.AddScoped<IUsersRepository, MongoDbUsersRepository>();
        services.AddScoped<IAccountManagementService, Auth0AccountManagementService>();

        // register a typed HTTP client as transient so it can retrieve a token when needed

        return services;
    }
}
