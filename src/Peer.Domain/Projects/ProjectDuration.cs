using Peer.Domain.Common;

namespace Peer.Domain.Projects;

public class ProjectDuration : AbstractValueObject<ProjectDuration>
{
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public bool LastsIndefinitely => EndDate is null;

    public ProjectDuration(DateTime startDate, DateTime? endDate = null)
    {
        if (endDate < startDate)
        {
            throw new InvalidOperationException(
                "Project end date must not be earlier than start date"
            );
        }

        StartDate = startDate;
        EndDate = endDate;
    }

#nullable disable

    [Obsolete("Used by ORM only")]
    public ProjectDuration() { }

#nullable restore


    protected override bool EqualsCore(ProjectDuration other) =>
        StartDate.Equals(other.StartDate) && EndDate.Equals(other.EndDate);

    protected override int GetHashCodeCore() => HashCode.Combine(StartDate, EndDate);
}
