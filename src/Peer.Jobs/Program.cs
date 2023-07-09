using Peer.Application.Contracts;
using Peer.Application.Handlers;
using Peer.Infrastructure;
using Peer.Jobs;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<DomainEventsProcessor>();

        services.AddScoped<IDomainEventsRepository, MongoDbDomainEventsRepository>();
        services.AddScoped<ContributorRemovedHandler>();
        services.AddScoped<ProjectRemovedHandler>();
        services.AddScoped<PositionRemovedHandler>();
        services.AddScoped<PositionClosedHandler>();
        services.AddScoped<ApplicationAcceptedHandler>();
        services.AddScoped<ApplicationSubmittedHandler>();

        // TODO: register repositories etc
    })
    .Build();

await host.RunAsync();
