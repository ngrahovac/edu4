using Peer.Application.Contracts;
using Peer.Domain.Collaborations;

namespace Peer.Application.Handlers;
public class ApplicationAcceptedHandler
{
    private readonly IApplicationsRepository _applications;
    private readonly ICollaborationsRepository _collaborations;

    public ApplicationAcceptedHandler(IApplicationsRepository applications, ICollaborationsRepository collaborations)
    {
        _applications = applications;
        _collaborations = collaborations;
    }

    public async Task MakeApplicantACollaboratorOnTheProjectAsync(Guid applicationId)
    {
        var acceptedApplication = await _applications.GetByIdAsync(applicationId);

        var collaboration = new Collaboration(
            acceptedApplication.ApplicantId,
            acceptedApplication.ProjectId,
            acceptedApplication.PositionId);

        await _collaborations.AddAsync(collaboration);
    }
}
