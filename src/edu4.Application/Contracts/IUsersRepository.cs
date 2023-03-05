using edu4.Domain.Users;

namespace edu4.Application.Contracts;
public interface IUsersRepository
{
    public Task AddAsync(User user);

    public Task<User?> GetByIdAsync(Guid id);
    public Task<User?> GetByAccountIdAsync(string accountId);
}
