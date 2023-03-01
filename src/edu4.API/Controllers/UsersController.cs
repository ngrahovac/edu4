using edu4.API.Models;
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
    private readonly UsersService _users;
    private readonly IAccountIdExtractionService _accountIdExtractionService;

    public UsersController(UsersService users, IAccountIdExtractionService accountIdExtractionService)
    {
        _users = users;
        _accountIdExtractionService = accountIdExtractionService;
    }

    [HttpPost]
    [Authorize(Policy = "NonContributor")]
    public async Task<ActionResult> SignUpAsync(UserSignupInputModel model)
    {
        var accountId = _accountIdExtractionService.ExtractAccountIdFromHttpRequest(Request);

        await _users.SignUpAsync(
                accountId,
                model.FullName!,
                model.ContactEmail!,
                model.Hats!.Select(h => new HatDTO(h.Type, h.Parameters)).ToList());

        return Ok(); // TODO: replace with Created
    }
}
