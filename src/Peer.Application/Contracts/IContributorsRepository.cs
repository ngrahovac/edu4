using Peer.Domain.Contributors;

namespace Peer.Application.Contracts;
public interface IContributorsRepository
{
    public Task AddAsync(Contributor user);

    public Task<Contributor> GetByIdAsync(Guid id);
    public Task<Contributor> GetByAccountIdAsync(string accountId);
    public Task UpdateAsync(Contributor contributor);
    Task RemoveAsync(Contributor contributor);
}
