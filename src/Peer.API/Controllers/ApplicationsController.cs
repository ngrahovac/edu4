using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Peer.API.Utils;
using Peer.Application.Services;
using Peer.Domain.Applications;
using Peer.Domain.Projects;

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

    [HttpPost]
    public async Task<ActionResult> SubmitAsync(Guid projectId, Guid positionId)
    {
        var requesterAccountId = _accountIdExtractionService.ExtractAccountIdFromHttpRequest(Request);
        var requesterId = await _contributors.GetUserIdFromAccountId(requesterAccountId);

        await _applications.SubmitAsync(requesterId, projectId, positionId);

        return Ok();
    }

    [HttpDelete("{applicationId}")]
    public async Task<ActionResult> RevokeAsync(Guid applicationId)
    {
        var requesterAccountId = _accountIdExtractionService.ExtractAccountIdFromHttpRequest(Request);
        var requesterId = await _contributors.GetUserIdFromAccountId(requesterAccountId);

        await _applications.RevokeAsync(requesterId, applicationId);

        return Ok();
    }

    [HttpPut("{applicationId}")]
    public async Task<ActionResult> AcceptAsync(Guid applicationId, ApplicationStatus status)
    {
        var requesterAccountId = _accountIdExtractionService.ExtractAccountIdFromHttpRequest(Request);
        var requesterId = await _contributors.GetUserIdFromAccountId(requesterAccountId);

        if (status is not ApplicationStatus.Accepted)
        {
            return BadRequest("This endpoint is used only for accepting an application");
        }

        await _applications.AcceptAsync(requesterId, applicationId);

        return Ok();
    }
}
