namespace edu4.Domain.Common;

internal abstract class AbstractValueObject<T>
{
    public override bool Equals(object? obj) =>
        obj is T other &&
        GetType() == other.GetType() &&
        EqualsCore(other);

    public override int GetHashCode() => GetHashCodeCore();

    public abstract bool EqualsCore(T other);

    public abstract int GetHashCodeCore();
}
