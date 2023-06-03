using Peer.Domain.Collaborations;

namespace Peer.Application.Contracts;
public interface ICollaborationsRepository
{
    Task AddAsync(Collaboration collaboration);
    Task<Collaboration> GetByIdAsync(Guid collaborationId);
    Task UpdateAsync(Collaboration collaboration);
}
