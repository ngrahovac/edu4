using Peer.Domain.Common;

namespace Peer.Application.Contracts;
public interface IDomainEventsRepository
{
    Task<List<AbstractDomainEvent>> GetUnprocessedBatchAsync(int batchSize);
    Task AddAsync(AbstractDomainEvent domainEvent);

    Task UpdateAsync(AbstractDomainEvent domainEvent);
}
