using edu4.Domain.Common;

namespace edu4.Domain.Users;
public abstract class Hat : AbstractValueObject<Hat>
{
    public abstract HatType Type { get; }
    public abstract bool Fits(Hat positionRequirements);
}
