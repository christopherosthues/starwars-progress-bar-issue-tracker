using GreenDonut.Data;

namespace StarWarsProgressBarIssueTracker.Domain;

public interface IRepository<TDomain>
{
    Task<TDomain?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Page<TDomain>> GetAllAsync(PagingArguments pagingArguments, CancellationToken cancellationToken = default);

    Task<TDomain> AddAsync(TDomain entity, CancellationToken cancellationToken = default);

    Task AddRangeAsync(IEnumerable<TDomain> entities, CancellationToken cancellationToken = default);

    Task<TDomain> UpdateAsync(TDomain entity, CancellationToken cancellationToken = default);

    Task<TDomain> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task DeleteRangeAsync(IEnumerable<TDomain> entities, CancellationToken cancellationToken = default);

    Task DeleteRangeByGitlabIdAsync(IEnumerable<TDomain> domains, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    Task UpdateRangeByGitlabIdAsync(IEnumerable<TDomain> domains, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;
}
