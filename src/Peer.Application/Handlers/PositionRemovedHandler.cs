using Peer.Application.Contracts;

namespace Peer.Application.Handlers;
public class PositionRemovedHandler
{
    private readonly IApplicationsRepository _applications;

    public PositionRemovedHandler(IApplicationsRepository applications) =>
        _applications = applications;

    public async Task RemoveAllApplicationsSubmittedForThePositionAsync(Guid projectId, Guid positionId)
    {
        var applicationsToRemove = await _applications.GetByPositionAsync(projectId, positionId);

        if (applicationsToRemove.Any())
        {
            applicationsToRemove.ForEach(a => a.Remove());
            await Task.WhenAll(applicationsToRemove.Select(a => _applications.UpdateAsync(a)));
        }
    }
}
