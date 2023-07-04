using Peer.Domain.Collaborations;

namespace Peer.Application.Contracts;
public interface ICollaborationsRepository
{
    Task AddAsync(Collaboration collaboration);
    Task<List<Collaboration>> GetAllByCollaboratorAsync(Guid collaboratorId);
    Task<Collaboration> GetByIdAsync(Guid collaborationId);
    Task UpdateAsync(Collaboration collaboration);

    Task<List<Collaboration>> GetAllForProjectAsync(Guid projectId);
}
