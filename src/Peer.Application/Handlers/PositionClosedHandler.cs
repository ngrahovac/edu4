using Peer.Application.Contracts;

namespace Peer.Application.Handlers;
public class PositionClosedHandler
{
    private readonly IApplicationsRepository _applications;

    public PositionClosedHandler(IApplicationsRepository applications) =>
        _applications = applications;

    public async Task RejectAllApplicationsSubmittedForThePositionAsync(Guid projectId, Guid positionId)
    {
        var applicationsToRemove = await _applications.GetByPositionAsync(projectId, positionId);

        if (applicationsToRemove.Any())
        {
            applicationsToRemove.ForEach(a => a.Reject());
            await Task.WhenAll(applicationsToRemove.Select(a => _applications.UpdateAsync(a)));
        }
    }
}
