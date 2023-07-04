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
    })
    .Build();

await host.RunAsync();
