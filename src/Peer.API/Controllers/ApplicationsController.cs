using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Peer.API.Models.Display;
using Peer.API.Models.Input;
using Peer.API.Utils;
using Peer.Application.Contracts;
using Peer.Application.Services;
using Peer.Domain.Applications;
using Peer.Domain.Contributors;
using Peer.Domain.Projects;

namespace Peer.API.Controllers;

[Route("api/[controller]")]
[Authorize(Policy = "Contributor")]
[ApiController]
public class ApplicationsController : ControllerBase
{
    private readonly ApplicationsService _applications;
    private readonly ContributorsService _contributors;
    private readonly ProjectsService _projects;
    private readonly IAccountIdExtractionService _accountIdExtractionService;

    public ApplicationsController(
        ApplicationsService applications,
        ContributorsService contributors,
        ProjectsService projectsService,
        IAccountIdExtractionService accountIdExtractionService
    )
    {
        _applications = applications;
        _contributors = contributors;
        _projects = projectsService;
        _accountIdExtractionService = accountIdExtractionService;
    }

    [HttpGet("project/{projectId}")]
    public async Task<
        ActionResult<ICollection<ApplicationDisplayModel>>
    > GetProjectApplicationsAsync(Guid projectId)
    {
        var requesterAccountId = _accountIdExtractionService.ExtractAccountIdFromHttpRequest(
            Request
        );
        var requesterId = await _contributors.GetUserIdFromAccountId(requesterAccountId);
        var requester = await _contributors.GetByIdAsync(requesterId);

        var project = await _projects.GetByIdAsync(projectId);

        var requesterIsProjectAuthor = requesterId.Equals(project.AuthorId);

        var applications = requesterIsProjectAuthor
            ? throw new NotImplementedException()
            : await _applications.GetSentAsync(requesterId, projectId, pageSize: int.MaxValue);

        return applications.Items.Select(a => new ApplicationDisplayModel(a, requester)).ToList();
    }

    [HttpGet("sent")]
    public async Task<ActionResult<PagedList<SentApplicationDisplayModel>>> GetSentAsync(
        Guid? projectId,
        Guid? positionId,
        ApplicationsSortOption? sort,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5
    )
    {
        var requesterAccountId = _accountIdExtractionService.ExtractAccountIdFromHttpRequest(
            Request
        );
        var requesterId = await _contributors.GetUserIdFromAccountId(requesterAccountId);
        var requester = await _contributors.GetByIdAsync(requesterId);

        var applications = await _applications.GetSentAsync(
            requesterId,
            projectId,
            positionId,
            sort ?? ApplicationsSortOption.Default,
            page,
            pageSize
        );

        var uniqueProjectIds = applications.Items
            .Select(a => a.ProjectId)
            .Distinct();

        var uniqueProjects = new List<Project>();

        foreach (var id in uniqueProjectIds)
        {
            var project = await _projects.GetByIdAsync(id);
            uniqueProjects.Add(project);
        }

        return new PagedList<SentApplicationDisplayModel>(
            applications.TotalItems,
            applications.Page,
            applications.TotalPages,
            applications.Items.Select(a => new SentApplicationDisplayModel(
                a,
                requester,
                uniqueProjects.Single(p => p.Id == a.ProjectId)))
            .ToList()
        );
    }

    [HttpGet("sent/projects")]
    public async Task<ActionResult<ICollection<ProjectDisplayModel>>> GetSubmittedApplicationsProjectsAsync()
    {
        var requesterAccountId = _accountIdExtractionService.ExtractAccountIdFromHttpRequest(
            Request
        );
        var requesterId = await _contributors.GetUserIdFromAccountId(requesterAccountId);
        var requester = await _contributors.GetByIdAsync(requesterId);

        var applications = await _applications.GetSentAsync(
            requesterId,
            null,
            null,
            ApplicationsSortOption.Default,
            pageSize: int.MaxValue
        );

        var projectsIds = applications.Items.Select(a => a.ProjectId).Distinct().ToList();

        List<Project> projects = new();
        foreach (var id in projectsIds)
        {
            var project = await _projects.GetByIdAsync(id);
            projects.Add(project);
        }

        return projects.Select(p => new ProjectDisplayModel(p, requester)).ToList();
    }

    [HttpGet("received")]
    public async Task<ActionResult<PagedList<ReceivedApplicationDisplayModel>>> GetReceivedAsync(
        Guid? projectId,
        Guid? positionId,
        ApplicationsSortOption? sort,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5
    )
    {
        var requesterAccountId = _accountIdExtractionService.ExtractAccountIdFromHttpRequest(
            Request
        );
        var requesterId = await _contributors.GetUserIdFromAccountId(requesterAccountId);
        var requester = await _contributors.GetByIdAsync(requesterId);

        var applications = await _applications.GetReceivedAsync(
            requesterId,
            projectId,
            positionId,
            sort ?? ApplicationsSortOption.Default,
            page,
            pageSize
        );

        var uniqueProjectIds = applications.Items
            .Select(a => a.ProjectId)
            .Distinct();

        var uniqueProjects = new List<Project>();

        foreach (var id in uniqueProjectIds)
        {
            var project = await _projects.GetByIdAsync(id);
            uniqueProjects.Add(project);
        }

        var uniqueApplicantIds = applications.Items
            .Select(a => a.ApplicantId)
            .Distinct();

        var uniqueApplicants = new List<Contributor>();

        foreach (var id in uniqueApplicantIds)
        {
            var applicant = await _contributors.GetByIdAsync(id);
            uniqueApplicants.Add(applicant);
        }

        return new PagedList<ReceivedApplicationDisplayModel>(
            applications.TotalItems,
            applications.Page,
            applications.TotalPages,
            applications.Items.Select(a => new ReceivedApplicationDisplayModel(
                a,
                requester,
                uniqueProjects.Single(p => p.Id == a.ProjectId),
                uniqueApplicants.Single(c => c.Id == a.ApplicantId))
            ).ToList());
    }

    [HttpPost]
    public async Task<ActionResult> SubmitAsync(ApplicationSubmissionInputModel model)
    {
        var requesterAccountId = _accountIdExtractionService.ExtractAccountIdFromHttpRequest(
            Request
        );
        var requesterId = await _contributors.GetUserIdFromAccountId(requesterAccountId);

        await _applications.SubmitAsync(requesterId, model.ProjectId, model.PositionId);

        return Ok();
    }

    [HttpDelete("{applicationId}")]
    public async Task<ActionResult> RevokeOrRejectAsync(Guid applicationId)
    {
        var requesterAccountId = _accountIdExtractionService.ExtractAccountIdFromHttpRequest(
            Request
        );
        var requesterId = await _contributors.GetUserIdFromAccountId(requesterAccountId);

        await _applications.RevokeOrRejectAsync(requesterId, applicationId);

        return Ok();
    }

    [HttpPut("{applicationId}")]
    public async Task<ActionResult> AcceptAsync(
        Guid applicationId,
        [FromBody] AcceptingApplicationInputModel model
    )
    {
        var requesterAccountId = _accountIdExtractionService.ExtractAccountIdFromHttpRequest(
            Request
        );
        var requesterId = await _contributors.GetUserIdFromAccountId(requesterAccountId);

        if (model.Status is not ApplicationStatus.Accepted)
        {
            return BadRequest("This endpoint is used only for accepting an application");
        }

        await _applications.AcceptAsync(requesterId, applicationId);

        return Ok();
    }
}
