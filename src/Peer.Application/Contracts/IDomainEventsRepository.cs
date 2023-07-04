using Peer.Domain.Common;

namespace Peer.Application.Contracts;
public interface IDomainEventsRepository
{
    Task AddAsync(AbstractDomainEvent domainEvent);
}
