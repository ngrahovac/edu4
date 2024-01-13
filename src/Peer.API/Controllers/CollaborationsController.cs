using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Peer.API.Models.Display;
using Peer.API.Utils;
using Peer.Application.Services;

namespace Peer.API.Controllers;

[Route("api/[controller]")]
[Authorize(Policy = "Contributor")]
[ApiController]
public class CollaborationsController : ControllerBase
{
    private readonly CollaborationsService _collaborations;
    private readonly ProjectsService _projects;
    private readonly ContributorsService _contributors;
    private readonly IAccountIdExtractionService _accountIdExtractionService;

    public CollaborationsController(
        CollaborationsService collaborations,
        ProjectsService projects,
        ContributorsService contributors,
        IAccountIdExtractionService accountsIdExtractionService
    )
    {
        _collaborations = collaborations;
        _projects = projects;
        _contributors = contributors;
        _accountIdExtractionService = accountsIdExtractionService;
    }

    [HttpGet("project/{projectId}")]
    public async Task<List<CollaborationDisplayModel>> GetAllForProjectAsync(Guid projectId)
    {
        var project =
            await _projects.GetByIdAsync(projectId)
            ?? throw new InvalidOperationException(
                "Can't fetch collaborations on a project that doesn't exist"
            );

        var collaborations = await _collaborations.GetAllForProjectAsync(projectId);

        return collaborations.Select(c => new CollaborationDisplayModel(c)).ToList();
    }

    [HttpGet("mine")]
    public async Task<List<CollaborationDisplayModel>> GetMineAsync()
    {
        var requesterAccountId = _accountIdExtractionService.ExtractAccountIdFromHttpRequest(
            Request
        );
        var requesterId = await _contributors.GetUserIdFromAccountId(requesterAccountId);

        var collaborations = await _collaborations.GetAllForCollaboratorAsync(requesterId);

        return collaborations.Select(c => new CollaborationDisplayModel(c)).ToList();
    }
}
