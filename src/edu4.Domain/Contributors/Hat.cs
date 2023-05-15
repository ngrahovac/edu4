using Peer.Domain.Common;

namespace Peer.Domain.Contributors;
public abstract class Hat : AbstractValueObject<Hat>
{
    public abstract HatType Type { get; }
    public abstract bool Fits(Hat positionRequirements);
}
