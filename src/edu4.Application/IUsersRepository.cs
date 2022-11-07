using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using edu4.Domain.Users;

namespace edu4.Application;
public interface IUsersRepository
{
    public Task AddAsync(User user);

    public Task<User?> GetByIdAsync(Guid id);
    public Task<User?> GetByAccountIdAsync(string accountId);
}
