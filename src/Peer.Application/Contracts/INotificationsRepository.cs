using Peer.Domain.Notifications;

namespace Peer.Application.Contracts;
public interface INotificationsRepository
{
    Task AddAsync(AbstractNotification notification);
}