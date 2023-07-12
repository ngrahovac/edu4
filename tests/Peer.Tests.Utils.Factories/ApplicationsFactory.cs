using System.Reflection;
using Peer.Domain.Applications;

namespace Peer.Tests.Utils.Factories;
public class ApplicationsFactory
{
    private Guid _applicantId = Guid.NewGuid();
    private Guid _projectId = Guid.NewGuid();
    private Guid _positionId = Guid.NewGuid();
    private ApplicationStatus _status = ApplicationStatus.Submitted;

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

    public Application Build()
    {
        var application = new Application(
            _applicantId,
            _projectId,
            _positionId);

        SetStatusViaReflection(application);

        return application;
    }

    private void SetStatusViaReflection(Application application)
    {
        var applicationStatusProp = typeof(Application).GetProperty(
            nameof(Application.Status),
            BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public) ??
            throw new InvalidOperationException("Error setting application status via reflection");

        applicationStatusProp.SetValue(application, _status);
    }
}
