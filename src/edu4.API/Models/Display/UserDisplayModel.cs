using edu4.Domain.Contributors;

namespace edu4.API.Models.Display;

#nullable disable
public class UserDisplayModel
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public IReadOnlyCollection<HatDisplayModel> Hats { get; set; }

    public UserDisplayModel(User user)
    {
        FullName = user.FullName;
        Email = user.ContactEmail;
        Hats = user.Hats.Select(h => new HatDisplayModel(h)).ToList();
    }
}
#nullable restore
