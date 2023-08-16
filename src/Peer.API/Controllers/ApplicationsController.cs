using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Peer.API.Models.Display;
using Peer.API.Models.Input;
using Peer.API.Utils;
using Peer.Application.Services;
using Peer.Domain.Applications;

namespace Peer.API.Controllers;
[Route("api/[controller]")]
[Authorize(Policy = "Contributor")]
[ApiController]
public class ApplicationsController : ControllerBase
{
    private readonly ApplicationsService _applications;
    private readonly ContributorsService _contributors;
    private readonly IAccountIdExtractionService _accountIdExtractionService;

    public ApplicationsController(
        ApplicationsService applications,
        ContributorsService contributors,
        IAccountIdExtractionService accountIdExtractionService)
    {
        _applications = applications;
        _contributors = contributors;
        _accountIdExtractionService = accountIdExtractionService;
    }

    [HttpGet("sent")]
    public async Task<ActionResult<ICollection<ApplicationDisplayModel>>> GetSentAsync(
        Guid? projectId,
        Guid? positionId,
        ApplicationsSortOption? sort)
    {
        var requesterAccountId = _accountIdExtractionService.ExtractAccountIdFromHttpRequest(Request);
        var requesterId = await _contributors.GetUserIdFromAccountId(requesterAccountId);

        var applications = await _applications.GetSentAsync(
            requesterId,
            projectId,
            positionId,
            sort ?? ApplicationsSortOption.Default);

        return applications.Select(a => new ApplicationDisplayModel(a)).ToList();
    }

    [HttpGet("sent/projects")]
    public async Task<ActionResult<ICollection<Guid>>> GetSubmittedApplicationsProjectsAsync()
    {
        var requesterAccountId = _accountIdExtractionService.ExtractAccountIdFromHttpRequest(Request);
        var requesterId = await _contributors.GetUserIdFromAccountId(requesterAccountId);

        var applications = await _applications.GetSentAsync(
            requesterId,
            null,
            null,
            ApplicationsSortOption.Default);

        return applications.Select(a => a.ProjectId).Distinct().ToList();
    }

    [HttpGet("received")]
    public async Task<ActionResult<ICollection<ApplicationDisplayModel>>> GetReceivedAsync(
        Guid? projectId,
        Guid? positionId,
        ApplicationsSortOption? sort)
    {
        var requesterAccountId = _accountIdExtractionService.ExtractAccountIdFromHttpRequest(Request);
        var requesterId = await _contributors.GetUserIdFromAccountId(requesterAccountId);

        var applications = await _applications.GetReceivedAsync(
            requesterId,
            projectId,
            positionId,
            sort ?? ApplicationsSortOption.Default);

        return applications.Select(a => new ApplicationDisplayModel(a)).ToList();
    }

    [HttpPost]
    public async Task<ActionResult> SubmitAsync(ApplicationSubmissionInputModel model)
    {
        var requesterAccountId = _accountIdExtractionService.ExtractAccountIdFromHttpRequest(Request);
        var requesterId = await _contributors.GetUserIdFromAccountId(requesterAccountId);

        await _applications.SubmitAsync(requesterId, model.ProjectId, model.PositionId);

        return Ok();
    }

    [HttpDelete("{applicationId}")]
    public async Task<ActionResult> RevokeOrRejectAsync(Guid applicationId)
    {
        var requesterAccountId = _accountIdExtractionService.ExtractAccountIdFromHttpRequest(Request);
        var requesterId = await _contributors.GetUserIdFromAccountId(requesterAccountId);

        await _applications.RevokeOrRejectAsync(requesterId, applicationId);

        return Ok();
    }

    [HttpPut("{applicationId}")]
    public async Task<ActionResult> AcceptAsync(Guid applicationId, [FromBody] AcceptingApplicationInputModel model)
    {
        var requesterAccountId = _accountIdExtractionService.ExtractAccountIdFromHttpRequest(Request);
        var requesterId = await _contributors.GetUserIdFromAccountId(requesterAccountId);

        if (model.Status is not ApplicationStatus.Accepted)
        {
            return BadRequest("This endpoint is used only for accepting an application");
        }

        await _applications.AcceptAsync(requesterId, applicationId);

        return Ok();
    }
}
