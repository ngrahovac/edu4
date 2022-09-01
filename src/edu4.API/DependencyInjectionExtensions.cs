using edu4.Application.External;
using edu4.Application;
using edu4.Infrastructure;

namespace edu4.API;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddIAMServices(this IServiceCollection services)
    {
        // registering a typed HttpClient used by the IAM service
        // and configuring it to fetch an access token
        // on each instatiation
        var config = new ConfigurationBuilder()
            .AddUserSecrets(typeof(DependencyInjectionExtensions).Assembly)
            .Build();

        var tokenFetchingService = new Auth0ManagementApiAccessTokenFetchingService(config);

        services.AddHttpClient<IAccountManagementService, Auth0AccountManagementService>(async client =>
        {
            client.BaseAddress = new Uri($"https://{config["Auth0:Domain"]}/api/v2/");
            var token = await tokenFetchingService.FetchAsync();

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        });

        services.AddScoped<IAccountManagementService, Auth0AccountManagementService>();

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        MongoDBSetupUtils.RegisterClassMaps();

        services.AddScoped<IUsersRepository, MongoDbUsersRepository>();

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<UsersService>();

        return services;
    }
}
