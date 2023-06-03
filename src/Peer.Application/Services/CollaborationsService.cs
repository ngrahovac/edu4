using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Peer.Application.Contracts;
using Peer.Domain.Collaborations;

namespace Peer.Application.Services;
public class CollaborationsService
{
    private readonly ICollaborationsRepository _collaborations;
    private readonly IProjectsRepository _projects;
    private readonly IContributorsRepository _contributors;
    private readonly IApplicationsRepository _applications;

    public CollaborationsService(
        IProjectsRepository projects,
        ICollaborationsRepository collaborations,
        IContributorsRepository contributors,
        IApplicationsRepository applications)
    {
        _collaborations = collaborations;
        _projects = projects;
        _contributors = contributors;
        _applications = applications;
    }
    public async Task MakeApplicantACollaboratorAsync(
        Guid applicantId,
        Guid projectId,
        Guid positionId)
    {
        var applicant = await _contributors.GetByIdAsync(applicantId) ??
            throw new InvalidOperationException("The contributor with the given Id doesn't exist");

        var project = await _projects.GetByIdAsync(projectId) ??
            throw new InvalidOperationException("The project with the given Id doesn't exist");

        var position = project.GetPositionById(positionId) ??
            throw new InvalidOperationException("The project doesn't have a position with the given Id");

        var application = await _applications.GetByApplicantAndPositionAsync(applicantId, positionId) ??
            throw new InvalidOperationException("Application of the given applicant for the given position doesn't exist");

        if (application.Status is not Domain.Applications.ApplicationStatus.Accepted)
        {
            throw new InvalidOperationException("The applicant can become a collaborator if and only if their application was accepted");
        }

        var collaboration = new Collaboration(applicantId, projectId, positionId);

        await _collaborations.AddAsync(collaboration);
    }

    public async Task TerminateAsync(
        Guid requesterId,
        Guid collaborationId)
    {
        var requester = await _contributors.GetByIdAsync(requesterId) ??
            throw new InvalidOperationException("The contributor with the given Id doesn't exist");

        var collaboration = await _collaborations.GetByIdAsync(collaborationId) ??
            throw new InvalidOperationException("An active collaboration with the given Id doesn't exist");

        if (collaboration.CollaboratorId == requester.Id)
        {
            collaboration.TerminateByCollaborator();
        }
        else
        {
            var project = await _projects.GetByIdAsync(collaboration.ProjectId);

            if (project.AuthorId != requesterId)
            {
                throw new InvalidOperationException("The requester doesn't have permission to terminate the collaboration");
            }

            collaboration.TerminateByAuthor();
        }

        await _collaborations.UpdateAsync(collaboration);
    }
}
