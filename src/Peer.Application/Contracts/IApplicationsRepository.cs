namespace Peer.Application.Contracts;

public interface IApplicationsRepository
{
    Task AddAsync(Domain.Applications.Application application);
    public Task<Domain.Applications.Application> GetByApplicantAndPositionAsync(Guid applicantId, Guid positionId);
}
