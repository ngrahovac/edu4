using Peer.Domain.Contributors;
using Microsoft.Extensions.Logging;
using Peer.Application.Contracts;
using Peer.Application.Models;

namespace Peer.Application.Services;

public class ContributorsService
{
    private readonly IContributorsRepository _contributors;
    private readonly IAccountManagementService _accountManagement;
    private readonly IDomainEventsRepository _domainEventsRepository;
    private readonly ILogger<ContributorsService> _logger;

    public ContributorsService(
        IContributorsRepository users,
        IAccountManagementService accountManagement,
        IDomainEventsRepository domainEventsRepository,
        ILogger<ContributorsService> logger)
    {
        _contributors = users;
        _accountManagement = accountManagement;
        _domainEventsRepository = domainEventsRepository;
        _logger = logger;
    }

    /// <summary>
    /// Assumes a valid account id registered under the tenant.
    /// </summary>
    /// <param name="accountId"></param>
    /// <param name="contactEmail"></param>
    /// <param name="hatData"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<Contributor> SignUpAsync(
        string accountId,
        string fullName,
        string contactEmail,
        List<HatDTO> hatData)
    {
        var user = new Contributor(
            accountId,
            fullName,
            contactEmail,
            hatData.Select(
                h => HatDTO.ToHat(h)).ToList());

        if (await _contributors.GetByAccountIdAsync(accountId) is not null)
        {
            _logger.LogError("User with {AccountId} already signed up", accountId);
            throw new InvalidOperationException($"User with account id {accountId} already signed up");
        }

        await _contributors.AddAsync(user);
        await _accountManagement.MarkUserSignedUpAsync(accountId);  // Q: cross-process transaction?

        _logger.LogInformation("User {User} successfully signed in", user);

        return user;
    }

    /// <summary>
    /// Note: This method is implemented here for convenience.
    /// Getting user's id from their account id in order to provide it
    /// to application services might change in the future.
    /// </summary>
    /// <param name="accountId"></param>
    /// <returns></returns>
    public async Task<Guid> GetUserIdFromAccountId(string accountId)
    {
        var user = await _contributors.GetByAccountIdAsync(accountId);

        if (user is null)
        {
            _logger.LogError("User with account id {AccountId} does not exist", accountId);
            throw new InvalidOperationException($"User with account id {accountId} does not exist");
        }

        return user.Id;
    }

    public async Task<Contributor> GetByIdAsync(Guid id)
    {
        var user = await _contributors.GetByIdAsync(id);

        if (user is null)
        {
            _logger.LogError("User with account id {AccountId} does not exist", id);
            throw new InvalidOperationException($"User with account id {id} does not exist");
        }

        return user;
    }

    public async Task UpdateSelfAsync(Guid requesterId, Guid contributorId, string fullName, string contactEmail, List<HatDTO> hats)
    {
        var contributor = await _contributors.GetByIdAsync(contributorId) ??
            throw new InvalidOperationException("The contributor with the given Id doesn't exist");

        if (requesterId != contributor.Id)
        {
            throw new InvalidOperationException("The requester doesn't have permissions to update the given contributor");
        }

        contributor.UpdateFullName(fullName);
        contributor.UpdateContactEmail(contactEmail);
        contributor.UpdateHats(hats.Select(h => HatDTO.ToHat(h)).ToList());

        await _contributors.UpdateAsync(contributor);
    }

    public async Task RemoveSelfAsync(Guid requesterId, Guid contributorId)
    {
        var contributor = await _contributors.GetByIdAsync(contributorId) ??
            throw new InvalidOperationException("The contributor with the given Id doesn't exist");

        if (requesterId != contributor.Id)
        {
            throw new InvalidOperationException("The requester doesn't have permission to remove the contributor");
        }

        contributor.Remove();

        // TODO: wrap in a transaction
        await _contributors.UpdateAsync(contributor);
        contributor.DomainEvents.ToList().ForEach(async e => await _domainEventsRepository.AddAsync(e));

        await _accountManagement.RemoveAccountAsync(contributor.AccountId);
    }
}
