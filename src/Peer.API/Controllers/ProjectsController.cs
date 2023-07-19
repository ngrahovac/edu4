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
    private readonly ContributorsService _contributors;
    private readonly IAccountIdExtractionService _accountIdExtractionService;

    public ProjectsController(ProjectsService projects, ContributorsService users, IAccountIdExtractionService accountIdExtractionService)
    {
        _projects = projects;
        _contributors = users;
        _accountIdExtractionService = accountIdExtractionService;
    }

    [HttpPost]
    [Authorize(Policy = "Contributor")]
    public async Task<ActionResult> PublishAsync(ProjectInputModel model)
    {
        var authorAccountId = _accountIdExtractionService.ExtractAccountIdFromHttpRequest(Request);
        var authorId = await _contributors.GetUserIdFromAccountId(authorAccountId);

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
    public async Task<IReadOnlyList<ProjectDisplayModel>> DiscoverAsync(
        string? keyword,
        HatType? hatType,
        ProjectsSortOption? sort)
    {
        var requesterAccountId = _accountIdExtractionService.ExtractAccountIdFromHttpRequest(Request);
        var requesterId = await _contributors.GetUserIdFromAccountId(requesterAccountId);
        var requester = await _contributors.GetByIdAsync(requesterId);

        var projects = await _projects.DiscoverAsync(
            keyword,
            sort ?? ProjectsSortOption.Unspecified,
            hatType is null ? null : requester.Hats.FirstOrDefault(h => h.Type.Equals(hatType)));

        return projects.Select(p => new ProjectDisplayModel(p, requester))
            .ToList();
    }


    [HttpPost("{projectId}/positions")]
    [Authorize(Policy = "Contributor")]
    public async Task<ActionResult> AddPositionsAsync(Guid projectId, IReadOnlyList<PositionInputModel> positions)
    {
        var requesterAccountId = _accountIdExtractionService.ExtractAccountIdFromHttpRequest(Request);
        var requesterId = await _contributors.GetUserIdFromAccountId(requesterAccountId);

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
        var requesterId = await _contributors.GetUserIdFromAccountId(requesterAccountId);

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
        var requesterId = await _contributors.GetUserIdFromAccountId(requesterAccountId);

        await _projects.RemoveAsync(projectId, requesterId);

        return Ok();
    }

    [HttpPut("{projectId}/positions/{positionId}")]
    [Authorize(Policy = "Contributor")]
    public async Task<ActionResult> ClosePositionAsync(Guid projectId, Guid positionId, bool open)
    {
        var requesterAccountId = _accountIdExtractionService.ExtractAccountIdFromHttpRequest(Request);
        var requesterId = await _contributors.GetUserIdFromAccountId(requesterAccountId);

        await (open ?
            _projects.ReopenPositionAsync(requesterId, projectId, positionId) :
            _projects.ClosePositionAsync(requesterId, projectId, positionId));

        return Ok();
    }


    [HttpDelete("{projectId}/positions/{positionId}")]
    [Authorize(Policy = "Contributor")]
    public async Task<ActionResult> RemovePositionAsync(Guid projectId, Guid positionId)
    {
        var requesterAccountId = _accountIdExtractionService.ExtractAccountIdFromHttpRequest(Request);
        var requesterId = await _contributors.GetUserIdFromAccountId(requesterAccountId);

        await _projects.RemovePositionAsync(requesterId, projectId, positionId);

        return Ok();
    }
}
