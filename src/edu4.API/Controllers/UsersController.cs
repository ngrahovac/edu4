using edu4.API.Models;
using edu4.Application;
using Microsoft.AspNetCore.Mvc;

namespace edu4.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly UsersService _users;

    public UsersController(UsersService users) =>
        _users = users;

    [HttpPost]
    public async Task<ActionResult> SignUpAsync(UserSignupInputModel model)
    {
        await _users.SignUpAsync(
                model.AccountId!,
                model.ContactEmail!,
                model.Hats!.Select(hatModel => HatFactory.FromHatInputModel(hatModel)).ToList());

        return Ok(); // TODO: replace with Created
    }
}
