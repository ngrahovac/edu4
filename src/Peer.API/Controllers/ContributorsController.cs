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

    [HttpGet("me")]
    [Authorize(Policy = "Contributor")]
    public async Task<ActionResult<UserDisplayModel>> GetMeAsync()
    {
        var accountId = _accountIdExtractionService.ExtractAccountIdFromHttpRequest(Request);
        var userId = await _contributors.GetUserIdFromAccountId(accountId);

        var user = await _contributors.GetByIdAsync(userId);

        return new UserDisplayModel(user);
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
}
