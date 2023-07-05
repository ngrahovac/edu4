using Peer.Application.Contracts;
using Peer.Domain.Projects;

namespace Peer.Application.Handlers;

public class ContributorRemovedHandler
{
    private readonly IAccountManagementService _accountManagementService;
    private readonly IProjectsRepository _projects;
    private readonly IApplicationsRepository _applications;
    private readonly ICollaborationsRepository _collaborations;
    private readonly IContributorsRepository _contributors;
    private readonly IDomainEventsRepository _domainEvents;

    public ContributorRemovedHandler(
        IAccountManagementService accountManagementService,
        IProjectsRepository projects,
        IApplicationsRepository applications,
        ICollaborationsRepository collaborations,
        IContributorsRepository contributors,
        IDomainEventsRepository domainEvents)
    {
        _accountManagementService = accountManagementService;
        _projects = projects;
        _applications = applications;
        _collaborations = collaborations;
        _contributors = contributors;
        _domainEvents = domainEvents;
    }

    public async Task RemoveAccountAsync(Guid contributorId)
    {
        var contributor = await _contributors.GetByIdAsync(contributorId);
        await _accountManagementService.RemoveAccountAsync(contributor.AccountId);
    }

    public async Task RemoveProjectsAuthoredByAuthorAsync(Guid authorId)
    {
        var projectsToRemove = await _projects.GetByAuthorAsync(authorId);

        if (projectsToRemove.Any())
        {
            // TODO: wrap in a transaction
            projectsToRemove.ForEach(p => p.Remove());
            await Task.WhenAll(projectsToRemove.Select(p => _projects.UpdateAsync(p)));
            await Task.WhenAll(projectsToRemove.Select(p => _domainEvents.AddAsync(new ProjectRemoved(p))));
        }
    }

    public async Task RemoveApplicationsSubmittedByApplicantAsync(Guid applicantId)
    {
        var applicationsToRemove = await _applications.GetByApplicantAsync(applicantId);

        if (applicationsToRemove.Any())
        {
            applicationsToRemove.ForEach(a => a.Remove());
            await Task.WhenAll(applicationsToRemove.Select(a => _applications.UpdateAsync(a)));
        }
    }

    public async Task RemoveCollaborationsByCollaborator(Guid collaboratorId)
    {
        var collaborationsToRemove = await _collaborations.GetAllByCollaboratorAsync(collaboratorId);

        if (collaborationsToRemove.Any())
        {
            collaborationsToRemove.ForEach(c => c.Remove());
            await Task.WhenAll(collaborationsToRemove.Select(c => _collaborations.UpdateAsync(c)));
        }
    }
}
