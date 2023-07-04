namespace Peer.Domain.Common;

public class AbstractAggregateRoot : AbstractEntity
{
    private readonly List<AbstractDomainEvent> _domainEvents = new();
    public IReadOnlyList<AbstractDomainEvent> DomainEvents => _domainEvents;

    protected void RaiseDomainEvent(AbstractDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}
