using Peer.API.Models.Display;
using Peer.API.Models.Input;
using Peer.API.Utils;
using Peer.Application.Models;
using Peer.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Peer.API.Controllers;
[Route("api/[controller]")]
[Authorize]
[ApiController]
public class ContributorsController : ControllerBase
{
    private readonly ContributorsService _contributors;
    private readonly IAccountIdExtractionService _accountIdExtractionService;

    public ContributorsController(ContributorsService contributors, IAccountIdExtractionService accountIdExtractionService)
    {
        _contributors = contributors;
        _accountIdExtractionService = accountIdExtractionService;
    }


    [HttpGet("{id}")]
    [Authorize(Policy = "Contributor")]
    public async Task<ActionResult<ContributorDisplayModel>> GetByIdAsync(Guid id)
    {
        var contributor = await _contributors.GetByIdAsync(id);

        if (contributor is null)
        {
            return NotFound();
        }

        var requesterAccountId = _accountIdExtractionService.ExtractAccountIdFromHttpRequest(Request);
        var requesterId = await _contributors.GetUserIdFromAccountId(requesterAccountId);
        var requester = await _contributors.GetByIdAsync(requesterId);

        return new ContributorDisplayModel(
            contributor,
            requester.Equals(contributor));
    }


    [HttpGet("me")]
    [Authorize(Policy = "Contributor")]
    public async Task<ActionResult<ContributorDisplayModel>> GetMeAsync()
    {
        var accountId = _accountIdExtractionService.ExtractAccountIdFromHttpRequest(Request);
        var userId = await _contributors.GetUserIdFromAccountId(accountId);

        var user = await _contributors.GetByIdAsync(userId);

        return new ContributorDisplayModel(user, true);
    }

    [HttpPost]
    [Authorize(Policy = "NonContributor")]
    public async Task<ActionResult> SignUpAsync(UserSignupInputModel model)
    {
        var accountId = _accountIdExtractionService.ExtractAccountIdFromHttpRequest(Request);

        await _contributors.SignUpAsync(
                accountId,
                model.FullName!,
                model.ContactEmail!,
                model.Hats!.Select(h => new HatDTO(h.Type, h.Parameters)).ToList());

        return Ok(); // TODO: replace with Created
    }

    [HttpPut("me")]
    [Authorize(Policy = "Contributor")]
    public async Task<ActionResult> UpdateSelfAsync(UserSignupInputModel model)
    {
        var requesterAccountId = _accountIdExtractionService.ExtractAccountIdFromHttpRequest(Request);
        var requesterId = await _contributors.GetUserIdFromAccountId(requesterAccountId);

        await _contributors.UpdateSelfAsync(
            requesterId,
            requesterId,
            model!.FullName,
            model!.ContactEmail,
            model!.Hats.Select(h => new HatDTO(h.Type, h.Parameters)).ToList());

        return Ok();
    }

    [HttpDelete("me")]
    [Authorize(Policy = "Contributor")]
    public async Task<ActionResult> DeleteSelfAsync()
    {
        var requesterAccountId = _accountIdExtractionService.ExtractAccountIdFromHttpRequest(Request);
        var requesterId = await _contributors.GetUserIdFromAccountId(requesterAccountId);

        await _contributors.RemoveSelfAsync(requesterId, requesterId);

        return Ok();
    }
}
