using Peer.Domain.Applications;

namespace Peer.Application.Contracts;

public interface IApplicationsRepository
{
    Task<Domain.Applications.Application> GetByIdAsync(Guid id);
    Task AddAsync(Domain.Applications.Application application);
    public Task<Domain.Applications.Application> GetByApplicantAndPositionAsync(Guid applicantId, Guid positionId);
    Task UpdateAsync(Domain.Applications.Application application);
    Task<List<Domain.Applications.Application>> GetReceivedAsync(
        Guid requesterId,
        Guid? projectId,
        Guid? positionId,
        ApplicationsSortOption applicationsSortOption);
    Task<List<Domain.Applications.Application>> GetSentAsync(
        Guid requesterId,
        Guid? projectId,
        Guid? positionId,
        ApplicationsSortOption applicationsSortOption);
    Task<List<Domain.Applications.Application>> GetByApplicantAsync(Guid applicantId);
    Task<List<Domain.Applications.Application>> GetByProjectAsync(Guid projectId);

    Task<List<Domain.Applications.Application>> GetByPositionAsync(Guid projectId, Guid positionId);
}
