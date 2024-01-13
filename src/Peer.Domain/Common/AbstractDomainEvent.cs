namespace Peer.Domain.Common;
public class AbstractDomainEvent : AbstractEntity
{
    public bool Processed { get; private set; }

    public AbstractDomainEvent() => Processed = false;

    public void Process()
    {
        if (Processed)
        {
            throw new InvalidOperationException("The domain event has already been processed");
        }

        Processed = false;
    }
}
