namespace Peer.Domain.Common;

public abstract class AbstractEntity
{
    public Guid Id { get; protected set; }

    public override bool Equals(object? obj) =>
        obj is AbstractEntity other &&
        GetType() == other.GetType() &&
        Id != Guid.Empty &&
        Id == other.Id;

    public override int GetHashCode() => HashCode.Combine(Id);
}
