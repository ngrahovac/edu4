using edu4.API.Models.Display;
using edu4.API.Models.Input;
using edu4.API.Utils;
using edu4.Application.Models;
using edu4.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace edu4.API.Controllers;
[Route("api/[controller]")]
[Authorize]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly ContributorsService _contributors;
    private readonly IAccountIdExtractionService _accountIdExtractionService;

    public UsersController(ContributorsService contributors, IAccountIdExtractionService accountIdExtractionService)
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
