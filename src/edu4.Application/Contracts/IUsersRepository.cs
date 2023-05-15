using edu4.Domain.Contributors;

namespace edu4.Application.Contracts;
public interface IUsersRepository
{
    public Task AddAsync(Contributor user);

    public Task<Contributor?> GetByIdAsync(Guid id);
    public Task<Contributor?> GetByAccountIdAsync(string accountId);
}
