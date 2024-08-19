using StarWarsProgressBarIssueTracker.Domain.Models;

namespace StarWarsProgressBarIssueTracker.Domain;

public interface IDataPort<TDomain> where TDomain : DomainBase
{
    Task<TDomain?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IEnumerable<TDomain>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<TDomain> AddAsync(TDomain domain, CancellationToken cancellationToken = default);

    Task<TDomain> UpdateAsync(TDomain domain, CancellationToken cancellationToken = default);

    Task<TDomain> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddRangeAsync(IEnumerable<TDomain> domains, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    Task DeleteRangeByGitlabIdAsync(IEnumerable<TDomain> domains, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    Task UpdateRangeByGitlabIdAsync(IEnumerable<TDomain> domains, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;
}
