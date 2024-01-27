using Peer.Domain.Projects;

namespace Peer.API.Models.Display;

public class DurationDisplayModel
{
    public string StartDate { get; set; }
    public string? EndDate { get; set; }

    public DurationDisplayModel(ProjectDuration duration)
    {
        StartDate = duration?.StartDate.ToShortDateString();
        EndDate = duration?.EndDate?.ToShortDateString();
    }
}
