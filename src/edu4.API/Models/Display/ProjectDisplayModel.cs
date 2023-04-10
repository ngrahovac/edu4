using edu4.Domain.Projects;

namespace edu4.API.Models.Display;

public class ProjectDisplayModel
{
    public Guid Id { get; set; }
    public DateTime DatePosted { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public Guid AuthorId { get; set; }
    public IReadOnlyCollection<PositionDisplayModel> Positions { get; set; }

    public ProjectDisplayModel(Project project)
    {
        Id = project.Id;
        DatePosted = project.DatePosted;
        Title = project.Title;
        Description = project.Description;
        AuthorId = project.Author.Id;
        Positions = project.Positions.Select(p => new PositionDisplayModel(p)).ToList();
    }
}
