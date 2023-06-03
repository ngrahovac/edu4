using Peer.Domain.Collaborations;

namespace Peer.Application.Contracts;
public interface ICollaborationsRepository
{
    Task AddAsync(Collaboration collaboration);
}
