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
public class ProjectsController : ControllerBase
{
    private readonly ProjectsService _projects;
    private readonly UsersService _users;

    public ProjectsController(ProjectsService projects, UsersService users)
    {
        _projects = projects;
        _users = users;
    }

    [HttpPost]
    [Authorize(Policy = "Contributor")]
    public async Task<ActionResult> PublishAsync(ProjectInputModel model)
    {
        var authorAccountId = AuthorizationUtils.ExtractAccountId(Request);
        var authorId = await _users.GetUserIdFromAccountId(authorAccountId);

        var project = await _projects.PublishProjectAsync(
            model.Title,
            model.Description,
            authorId,
            model.Positions.Select(m => new PositionDTO(
                m.Name,
                m.Description,
                new HatDTO(m.Requirements.Type, m.Requirements.Parameters)))
            .ToList());

        return Ok(); // TODO: replace with Created
    }
}
