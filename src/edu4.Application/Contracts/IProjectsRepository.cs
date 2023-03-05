using edu4.Domain.Projects;

namespace edu4.Application.Contracts;

public interface IProjectsRepository
{
    public Task AddAsync(Project project);

    public Task<Project> GetByIdAsync(Guid id);
}
