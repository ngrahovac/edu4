using Peer.Application.Contracts;
using Peer.Domain.Projects;

namespace Peer.Application.Handlers;

public class ContributorRemovedHandler
{
    private readonly IAccountManagementService _accountManagementService;
    private readonly IProjectsRepository _projects;
    private readonly IApplicationsRepository _applications;
    private readonly ICollaborationsRepository _collaborations;
    private readonly IDomainEventsRepository _domainEvents;

    public ContributorRemovedHandler(
        IAccountManagementService accountManagementService,
        IProjectsRepository projects,
        IApplicationsRepository applications,
        ICollaborationsRepository collaborations,
        IDomainEventsRepository domainEvents)
    {
        _accountManagementService = accountManagementService;
        _projects = projects;
        _applications = applications;
        _collaborations = collaborations;
        _domainEvents = domainEvents;
    }

    public Task RemoveAccountAsync(string accountId) => _accountManagementService.RemoveAccountAsync(accountId);

    public async Task RemoveProjectsAuthoredByAuthorAsync(Guid authorId)
    {
        var projectsToRemove = await _projects.GetByAuthorAsync(authorId);

        if (projectsToRemove.Any())
        {
            // TODO: wrap in a transaction
            projectsToRemove.ForEach(p => p.Remove());
            projectsToRemove.ForEach(async p => await _projects.UpdateAsync(p));
            projectsToRemove.ForEach(async p => await _domainEvents.AddAsync(new ProjectRemoved(p)));
        }
    }

    public async Task RemoveApplicationsSubmittedByApplicantAsync(Guid applicantId)
    {
        var applicationsToRemove = await _applications.GetByApplicantAsync(applicantId);

        if (applicationsToRemove.Any())
        {
            applicationsToRemove.ForEach(a => a.Remove());
            applicationsToRemove.ForEach(async a => await _applications.UpdateAsync(a));
        }
    }

    public async Task RemoveCollaborationsByCollaborator(Guid collaboratorId)
    {
        var collaborationsToRemove = await _collaborations.GetAllByCollaboratorAsync(collaboratorId);

        if (collaborationsToRemove.Any())
        {
            collaborationsToRemove.ForEach(c => c.Remove());
            collaborationsToRemove.ForEach(async c => await _collaborations.UpdateAsync(c));
        }
    }
}
