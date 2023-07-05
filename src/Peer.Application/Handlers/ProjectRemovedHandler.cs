using Peer.Application.Contracts;

namespace Peer.Application.Handlers;
public class ProjectRemovedHandler
{
    private readonly ICollaborationsRepository _collaborations;
    private readonly IApplicationsRepository _applications;

    public ProjectRemovedHandler(ICollaborationsRepository collaborations, IApplicationsRepository applications)
    {
        _collaborations = collaborations;
        _applications = applications;
    }

    public async Task RemoveApplicationsSubmittedForPositionsOnTheProjectAsync(Guid projectId)
    {
        var applicationsToRemove = await _applications.GetByProjectAsync(projectId);

        if (applicationsToRemove.Any())
        {
            applicationsToRemove.ForEach(a => a.Remove());
            applicationsToRemove.ForEach(async a => await _applications.UpdateAsync(a));
        }
    }

    public async Task RemoveCollaborationsOnTheProjectAsync(Guid projectId)
    {
        var collaborationsToRemove = await _collaborations.GetAllForProjectAsync(projectId);

        if (collaborationsToRemove.Any())
        {
            collaborationsToRemove.ForEach(c => c.Remove());
            collaborationsToRemove.ForEach(async c => await _collaborations.UpdateAsync(c));
        }
    }
}
