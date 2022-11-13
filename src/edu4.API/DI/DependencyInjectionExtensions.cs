using edu4.Infrastructure;
using edu4.Application.Contracts;
using edu4.Application.Services;

namespace edu4.API.DI;

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

        // typed client has a transient lifetime
        // token is fetched from the service using it
        services.AddScoped<IAccountManagementService, Auth0AccountManagementService>();

        services.AddHttpClient<IAccountManagementService, Auth0AccountManagementService>(async client =>
        {
            client.BaseAddress = new Uri($"https://{config["Auth0:Domain"]}/api/v2/");

            var tokenFetchingService = new Auth0ManagementApiAccessTokenFetchingService(config);
            var token = await tokenFetchingService.FetchAsync();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        });

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        MongoDBSetupUtils.RegisterClassMaps();

        services.AddScoped<IUsersRepository, MongoDbUsersRepository>();
        services.AddScoped<IProjectsRepository, MongoDBProjectsRepository>();

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<UsersService>();
        services.AddScoped<ProjectsService>();

        return services;
    }
}
