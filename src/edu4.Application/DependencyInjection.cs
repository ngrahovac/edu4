using Microsoft.Extensions.DependencyInjection;

namespace edu4.Application;
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<UsersService>();

        return services;
    }
}
