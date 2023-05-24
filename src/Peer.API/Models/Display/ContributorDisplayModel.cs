using Peer.Domain.Contributors;

namespace Peer.API.Models.Display;

#nullable disable
public class ContributorDisplayModel
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public IReadOnlyCollection<HatDisplayModel> Hats { get; set; }

    public ContributorDisplayModel(Contributor user)
    {
        FullName = user.FullName;
        Email = user.ContactEmail;
        Hats = user.Hats.Select(h => new HatDisplayModel(h)).ToList();
    }
}
#nullable restore
