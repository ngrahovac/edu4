using Peer.Domain.Notifications;

namespace Peer.Application.Contracts;
public interface INotificationsRepository
{
    Task AddAsync(AbstractNotification notification);
    Task<List<AbstractNotification>> GetForRequesterAsync(Guid requesterId);
}
