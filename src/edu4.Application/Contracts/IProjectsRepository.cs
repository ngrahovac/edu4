using edu4.Domain.Contributors;
using edu4.Domain.Projects;

namespace edu4.Application.Contracts;

public interface IProjectsRepository
{
    public Task AddAsync(Project project);
    public Task<IReadOnlyList<Project>> GetRecommendedForUserWearing(Hat hat);
    public Task<Project> GetByIdAsync(Guid id);

    public Task<IReadOnlyList<Project>> DiscoverAsync(
        string? keyword,
        ProjectsSortOption sortOption,
        Hat? usersHat);
    public Task UpdateAsync(Project project);
    public Task DeleteAsync(Guid projectId);
}
