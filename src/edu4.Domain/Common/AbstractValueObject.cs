namespace edu4.Domain.Common;

public abstract class AbstractValueObject<T>
{
    public override bool Equals(object? obj) =>
        obj is T other &&
        GetType() == other.GetType() &&
        EqualsCore(other);

    public override int GetHashCode() => GetHashCodeCore();

    protected abstract bool EqualsCore(T other);

    protected abstract int GetHashCodeCore();
}
