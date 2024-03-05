using Peer.Domain.Contributors;
using Peer.Domain.Projects;

namespace Peer.Application.Contracts;

public interface IProjectsRepository
{
    public Task AddAsync(Project project);
    public Task<IReadOnlyList<Project>> GetRecommendedForUserWearing(Hat hat);
    public Task<Project> GetByIdAsync(Guid id);

    public Task<PagedList<Project>> DiscoverAsync(
        Guid requesterId,
        string? keyword,
        ProjectsSortOption sortOption,
        Hat? usersHat,
        int page = 1,
        int pageSize = 5);

    public Task UpdateAsync(Project project);
    Task<List<Project>> GetByAuthorAsync(Guid authorId);
}
