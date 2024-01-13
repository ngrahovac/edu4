namespace Peer.Domain.Common;

public class AbstractDomainEvent : AbstractEntity
{
    public DateTime TimeRaised { get; private set; }

    public bool Processed { get; private set; }

    public AbstractDomainEvent(DateTime timeRaised)
    {
        TimeRaised = timeRaised;
        Processed = false;
    }

    public void Process()
    {
        if (Processed)
        {
            throw new InvalidOperationException("The domain event has already been processed");
        }

        Processed = true;
    }
}
