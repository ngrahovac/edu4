using Peer.Application.Contracts;
using Peer.Application.Handlers;
using Peer.Infrastructure;
using Peer.Jobs;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<DomainEventsProcessor>();

        services.AddSingleton<IDomainEventsRepository, MongoDbDomainEventsRepository>();
        services.AddSingleton<ContributorRemovedHandler>();
        services.AddSingleton<ProjectRemovedHandler>();
        services.AddSingleton<PositionRemovedHandler>();
        services.AddSingleton<PositionClosedHandler>();
        services.AddSingleton<ApplicationAcceptedHandler>();
        services.AddSingleton<ApplicationSubmittedHandler>();

        services.AddSingleton<IAccountManagementService, Auth0AccountManagementService>();

        services.AddSingleton<IContributorsRepository, MongoDbContributorsRepository>();
        services.AddSingleton<IProjectsRepository, MongoDBProjectsRepository>();
        services.AddSingleton<IApplicationsRepository, MongoDbApplicationsRepository>();
        services.AddSingleton<ICollaborationsRepository, MongoDbCollaborationsRepository>();
        services.AddSingleton<IDomainEventsRepository, MongoDbDomainEventsRepository>();
        services.AddSingleton<INotificationsRepository, MongoDbNotificationsRepository>();

        services.AddSingleton(serviceProvider =>
        {
            var client = new HttpClient();
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            client.BaseAddress = new Uri($"https://{config["Auth0:Domain"]}/api/v2/");

            var tokenFetchingService = new Auth0ManagementApiAccessTokenFetchingService(config);
            var token = tokenFetchingService.FetchAsync().Result;
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            return client;
        });

        MongoDBSetupUtils.RegisterClassMaps();
    })
    .Build();

await host.RunAsync();
