using edu4.API.Models.Display;
using edu4.API.Models.Input;
using edu4.API.Utils;
using edu4.Application.Models;
using edu4.Application.Services;
using edu4.Domain.Projects;
using edu4.Domain.Users;
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
    private readonly IAccountIdExtractionService _accountIdExtractionService;

    public ProjectsController(ProjectsService projects, UsersService users, IAccountIdExtractionService accountIdExtractionService)
    {
        _projects = projects;
        _users = users;
        _accountIdExtractionService = accountIdExtractionService;
    }

    [HttpPost]
    [Authorize(Policy = "Contributor")]
    public async Task<ActionResult> PublishAsync(ProjectInputModel model)
    {
        var authorAccountId = _accountIdExtractionService.ExtractAccountIdFromHttpRequest(Request);
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


    [HttpGet]
    [Authorize(Policy = "Contributor")]
    public async Task<IReadOnlyList<ProjectDisplayModel>> DiscoverAsync(string? keyword, HatType? selectedHat, ProjectsSortOption? sortBy)
    {
        var userAccountId = _accountIdExtractionService.ExtractAccountIdFromHttpRequest(Request);
        var userId = await _users.GetUserIdFromAccountId(userAccountId);
        var user = await _users.GetByIdAsync(userId);

        var projects = await _projects.DiscoverAsync(
            keyword,
            sortBy ?? ProjectsSortOption.Default,
            selectedHat is null ? null : user.Hats.FirstOrDefault(h => h.Type.Equals(selectedHat)));

        return projects.Select(p => new ProjectDisplayModel(p))
            .ToList();
    }
}
