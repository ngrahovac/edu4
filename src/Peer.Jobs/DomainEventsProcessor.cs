using Peer.Application.Contracts;
using Peer.Application.Handlers;
using Peer.Domain.Applications;
using Peer.Domain.Contributors;
using Peer.Domain.Projects;

namespace Peer.Jobs;

public class DomainEventsProcessor : BackgroundService
{
    private readonly ILogger<DomainEventsProcessor> _logger;
    private readonly IDomainEventsRepository _domainEvents;
    private readonly ContributorRemovedHandler _contributorRemovedHandler;
    private readonly ProjectRemovedHandler _projectRemovedHandler;
    private readonly PositionRemovedHandler _positionRemovedHandler;
    private readonly PositionClosedHandler _positionClosedHandler;
    private readonly ApplicationAcceptedHandler _applicationAcceptedHandler;

    public DomainEventsProcessor(
        ILogger<DomainEventsProcessor> logger,
        IDomainEventsRepository domainEvents,
        ContributorRemovedHandler contributorRemovedHandler,
        ProjectRemovedHandler projectRemovedHandler,
        PositionRemovedHandler positionRemovedHandler,
        PositionClosedHandler positionClosedHandler,
        ApplicationAcceptedHandler applicationAcceptedHandler)
    {
        _logger = logger;
        _domainEvents = domainEvents;
        _contributorRemovedHandler = contributorRemovedHandler;
        _projectRemovedHandler = projectRemovedHandler;
        _positionRemovedHandler = positionRemovedHandler;
        _positionClosedHandler = positionClosedHandler;
        _applicationAcceptedHandler = applicationAcceptedHandler;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var unprocessedDomainEvents = await _domainEvents.GetUnprocessedBatchAsync(10);

            unprocessedDomainEvents.AsParallel().ForAll(async de =>
            {
                if (de is ContributorRemoved cr)
                {
                    await Task.WhenAll(
                        _contributorRemovedHandler.RemoveAccountAsync(cr.ContributorId),
                        _contributorRemovedHandler.RemoveProjectsAuthoredByAuthorAsync(cr.ContributorId),
                        _contributorRemovedHandler.RemoveApplicationsSubmittedByApplicantAsync(cr.ContributorId),
                        _contributorRemovedHandler.RemoveCollaborationsByCollaborator(cr.ContributorId));
                }
                else if (de is ProjectRemoved pr)
                {
                    await Task.WhenAll(
                        _projectRemovedHandler.RemoveApplicationsSubmittedForPositionsOnTheProjectAsync(pr.ProjectId),
                        _projectRemovedHandler.RemoveCollaborationsOnTheProjectAsync(pr.ProjectId));
                }
                else if (de is PositionRemoved ppr)
                {
                    await _positionRemovedHandler.RemoveAllApplicationsSubmittedForThePositionAsync(ppr.ProjectId, ppr.PositionId);
                }
                else if (de is PositionClosed ppc)
                {
                    await _positionClosedHandler.RejectAllApplicationsSubmittedForThePositionAsync(ppc.ProjectId, ppc.PositionId);
                }
                else if (de is ApplicationAccepted aa)
                {
                    await _applicationAcceptedHandler.MakeApplicantACollaboratorOnTheProjectAsync(aa.ApplicationId);
                }
                else
                {
                    throw new NotImplementedException($"Handling domain event of type {de.GetType().Name} is not implemented yet");
                }

                de.Process();
                await _domainEvents.UpdateAsync(de);
            });

            _logger.LogInformation("Processed {ProcessedDomainEvents} domain events", unprocessedDomainEvents.Count);
            await Task.Delay(10000, stoppingToken);
        }
    }
}
