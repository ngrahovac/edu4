using System.Reflection;
using Microsoft.Extensions.Configuration;
using Peer.Domain.Applications;
using Peer.Infrastructure;

namespace Peer.Tests.Application.TestData;
internal class ApplicationsFactory
{
    private Guid _applicantId = Guid.NewGuid();
    private Guid _projectId = Guid.NewGuid();
    private Guid _positionId = Guid.NewGuid();
    private ApplicationStatus _status = ApplicationStatus.Submitted;
    private readonly MongoDbApplicationsRepository _applications;

    public ApplicationsFactory()
    {
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly)
            .Build();

        _applications = new MongoDbApplicationsRepository(config);
    }

    public ApplicationsFactory WithApplicantId(Guid applicantId)
    {
        _applicantId = applicantId;
        return this;
    }

    public ApplicationsFactory WithProjectId(Guid projectId)
    {
        _projectId = projectId;
        return this;
    }

    public ApplicationsFactory WithPositionId(Guid positionId)
    {
        _positionId = positionId;
        return this;
    }

    public ApplicationsFactory WithStatus(ApplicationStatus status)
    {
        _status = status;
        return this;
    }

    public async Task<Domain.Applications.Application> SeedAsync()
    {
        var application = new Domain.Applications.Application(
            _applicantId,
            _projectId,
            _positionId);

        SetStatusViaReflection(application);

        await _applications.AddAsync(application);

        return application;
    }

    private void SetStatusViaReflection(Domain.Applications.Application application)
    {
        var applicationStatusProp = typeof(Domain.Applications.Application).GetProperty(
            nameof(Domain.Applications.Application.Status),
            BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public) ??
            throw new InvalidOperationException("Error setting application status via reflection");

        applicationStatusProp.SetValue(application, _status);
    }
}
