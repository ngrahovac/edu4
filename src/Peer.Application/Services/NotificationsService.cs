using Peer.Application.Contracts;
using Peer.Domain.Notifications;

namespace Peer.Application.Services;
public class NotificationsService
{
    private readonly INotificationsRepository _notifications;

    public NotificationsService(INotificationsRepository notifications)
    {
        _notifications = notifications;
    }

    public Task<List<AbstractNotification>> GetForRequesterAsync(Guid requesterId) => _notifications.GetForRequesterAsync(requesterId);
}
