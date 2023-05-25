using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Peer.API.Utils;
using Peer.Application.Services;

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
}
