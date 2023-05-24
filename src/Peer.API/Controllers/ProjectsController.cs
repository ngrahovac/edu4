using Peer.API.Models.Display;
using Peer.API.Models.Input;
using Peer.API.Utils;
using Peer.Application.Models;
using Peer.Application.Services;
using Peer.Domain.Contributors;
using Peer.Domain.Projects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Peer.API.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class ProjectsController : ControllerBase
{
    private readonly ProjectsService _projects;
    private readonly ContributorsService _users;
    private readonly IAccountIdExtractionService _accountIdExtractionService;

    public ProjectsController(ProjectsService projects, ContributorsService users, IAccountIdExtractionService accountIdExtractionService)
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
    public async Task<IReadOnlyList<ProjectDisplayModel>> DiscoverAsync(string? keyword, HatType? hatType, ProjectsSortOption? sort)
    {
        var userAccountId = _accountIdExtractionService.ExtractAccountIdFromHttpRequest(Request);
        var userId = await _users.GetUserIdFromAccountId(userAccountId);
        var user = await _users.GetByIdAsync(userId);

        var projects = await _projects.DiscoverAsync(
            keyword,
            sort ?? ProjectsSortOption.Default,
            hatType is null ? null : user.Hats.FirstOrDefault(h => h.Type.Equals(hatType)));

        return projects.Select(p => new ProjectDisplayModel(p, user))
            .ToList();
    }


    [HttpPost("{projectId}/positions")]
    [Authorize(Policy = "Contributor")]
    public async Task<ActionResult> AddPositionsAsync(Guid projectId, IReadOnlyList<PositionInputModel> positions)
    {
        var requesterAccountId = _accountIdExtractionService.ExtractAccountIdFromHttpRequest(Request);
        var requesterId = await _users.GetUserIdFromAccountId(requesterAccountId);

        foreach (var positionModel in positions)
        {
            await _projects.AddPositionAsync(
                projectId,
                requesterId,
                positionModel.Name,
                positionModel.Description,
                new HatDTO(positionModel.Requirements.Type, positionModel.Requirements.Parameters));
        }

        return Ok();
    }


    [HttpPut("{projectId}/details")]
    [Authorize(Policy = "Contributor")]
    public async Task<ActionResult> UpdateDetailsAsync(Guid projectId, string title, string description)
    {
        var requesterAccountId = _accountIdExtractionService.ExtractAccountIdFromHttpRequest(Request);
        var requesterId = await _users.GetUserIdFromAccountId(requesterAccountId);

        await _projects.UpdateDetailsAsync(
            projectId,
            requesterId,
            title,
            description);

        return Ok();
    }


    [HttpDelete("{projectId}")]
    [Authorize(Policy = "Contributor")]
    public async Task<ActionResult> RemoveAsync(Guid projectId)
    {
        var requesterAccountId = _accountIdExtractionService.ExtractAccountIdFromHttpRequest(Request);
        var requesterId = await _users.GetUserIdFromAccountId(requesterAccountId);

        await _projects.RemoveAsync(projectId, requesterId);

        return Ok();
    }
}