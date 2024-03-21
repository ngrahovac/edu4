using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Peer.API.Models.Display;
using Peer.API.Utils;
using Peer.Application.Services;

namespace Peer.API.Controllers;


[Route("api/[controller]")]
[Authorize]
[ApiController]
public class NotificationsController : ControllerBase
{
    private readonly IAccountIdExtractionService _accountIdExtractionService;
    private readonly NotificationsService _notifications;
    private readonly ContributorsService _contributors;

    public NotificationsController(
        IAccountIdExtractionService accountIdExtractionService,
        NotificationsService notifications,
        ContributorsService contributors)
    {
        _accountIdExtractionService = accountIdExtractionService;
        _notifications = notifications;
        _contributors = contributors;
    }

    [HttpGet]
    public async Task<List<NotificationDisplayModel>> GetUnprocessedAsync()
    {
        var requesterAccountId = _accountIdExtractionService.ExtractAccountIdFromHttpRequest(
        Request
                );
        var requesterId = await _contributors.GetUserIdFromAccountId(requesterAccountId);

        var notifs = await _notifications.GetForRequesterAsync(requesterId);
        var models = notifs.Select(n => new NotificationDisplayModel(n))
            .ToList();

        return models;
    }
}
