using Microsoft.Extensions.Configuration;
using Peer.Application.Contracts;
using Peer.Domain.Applications;
using Peer.Infrastructure;
using Peer.Tests.Utils.Factories;

namespace Peer.Test.Utils.Seeders;

public class ApplicationsSeeder
{
    private readonly IApplicationsRepository _applications;
    private readonly ApplicationsFactory _applicationsFactory;

    public ApplicationsSeeder(IConfiguration configuration)
    {
        _applications = new MongoDbApplicationsRepository(configuration);
        _applicationsFactory = new ApplicationsFactory();
    }

    public ApplicationsSeeder WithApplicantId(Guid applicantId)
    {
        _applicationsFactory.WithApplicantId(applicantId);
        return this;
    }

    public ApplicationsSeeder WithProjectId(Guid projectId)
    {
        _applicationsFactory.WithProjectId(projectId);
        return this;
    }

    public ApplicationsSeeder WithPositionId(Guid positionId)
    {
        _applicationsFactory.WithPositionId(positionId);
        return this;
    }

    public ApplicationsSeeder WithStatus(ApplicationStatus status)
    {
        _applicationsFactory.WithStatus(status);
        return this;
    }

    public async Task<Domain.Applications.Application> SeedAsync()
    {
        var application = _applicationsFactory.Build();

        await _applications.AddAsync(application);

        return application;
    }
}
