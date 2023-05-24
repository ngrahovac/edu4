namespace Peer.Application.Contracts;

public interface IApplicationsRepository
{
    Task AddAsync(Domain.Applications.Application application);
    public Task<Domain.Applications.Application> GetByApplicantAndPosition(Guid applicantId, Guid positionId);
}
