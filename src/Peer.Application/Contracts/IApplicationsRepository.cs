namespace Peer.Application.Contracts;

public interface IApplicationsRepository
{
    Task<Domain.Applications.Application> GetByIdAsync(Guid id);
    Task AddAsync(Domain.Applications.Application application);
    public Task<Domain.Applications.Application> GetByApplicantAndPositionAsync(Guid applicantId, Guid positionId);
    Task UpdateAsync(Domain.Applications.Application application);
}
