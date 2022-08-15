namespace edu4.Domain.Common;

internal abstract class AbstractEntity
{
    public Guid Id { get; private set; }

    public override bool Equals(object? obj) =>
        obj is AbstractEntity other &&
        GetType() == other.GetType() &&
        Id != Guid.Empty &&
        Id == other.Id;

    public override int GetHashCode() => HashCode.Combine(Id);
}
