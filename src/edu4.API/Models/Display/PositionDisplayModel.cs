using edu4.Domain.Contributors;
using edu4.Domain.Projects;

namespace edu4.API.Models.Display;

public class PositionDisplayModel
{
    public Guid Id { get; set; }
    public DateTime DatePosted { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public HatDisplayModel Requirements { get; set; }
    public bool Recommended { get; set; }

    public PositionDisplayModel(Position position, Contributor requester)
    {
        Id = position.Id;
        DatePosted = position.DatePosted;
        Name = position.Name;
        Description = position.Description;
        Requirements = new HatDisplayModel(position.Requirements);
        Recommended = position.IsRecommendedFor(requester);
    }
}
