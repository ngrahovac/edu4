using edu4.API.Models;
using edu4.API.Utils;
using edu4.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace edu4.API.Controllers;
[Route("api/[controller]")]
[Authorize]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly UsersService _users;

    public UsersController(UsersService users) =>
        _users = users;

    [HttpPost]
    public async Task<ActionResult> SignUpAsync(UserSignupInputModel model)
    {
        var accountId = AuthorizationUtils.ExtractAccountId(Request);

        await _users.SignUpAsync(
                accountId,
                model.ContactEmail!,
                model.Hats!.Select(hatModel => HatFactory.FromHatInputModel(hatModel)).ToList());

        return Ok(); // TODO: replace with Created
    }
}
