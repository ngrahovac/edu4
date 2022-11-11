using edu4.Domain.Common;

namespace edu4.Domain.Projects;

public class Author : AbstractValueObject<Author>
{
    public Guid Id { get; }

    public Author(Guid id)
        => Id = id;

    protected override bool EqualsCore(Author other)
        => Id.Equals(other.Id);

    protected override int GetHashCodeCore()
        => HashCode.Combine(Id);
}
