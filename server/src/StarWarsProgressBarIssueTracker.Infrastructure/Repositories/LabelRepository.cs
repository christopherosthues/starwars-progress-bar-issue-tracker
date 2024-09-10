using HotChocolate.Pagination;
using Microsoft.EntityFrameworkCore;
using StarWarsProgressBarIssueTracker.Domain.Labels;
using StarWarsProgressBarIssueTracker.Infrastructure.Database;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Repositories;

public class LabelRepository(IssueTrackerContext context) : IssueTrackerRepositoryBase<Label>(context), ILabelRepository
{
    public override async Task<Page<Label>> GetAllAsync(PagingArguments pagingArguments,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            // .Include(dbLabel => dbLabel.Issues)
            .OrderBy(label => label.Title)
            .ThenBy(label => label.Id)
            .ToPageAsync(pagingArguments, cancellationToken);
    }

    public IQueryable<Label> GetLabelByIds(IReadOnlyList<Guid> ids)
    {
        return Context.Labels
            .AsNoTracking()
            .Where(label => ids.Contains(label.Id));
    }

    public async Task DeleteRangeByGitlabIdAsync(IEnumerable<Label> domains, CancellationToken cancellationToken = default)
    {
        var labels = await DbSet.ToListAsync(cancellationToken);
        var toBeDeleted = labels.Where(dbLabel => domains.Any(label => label.GitlabId?.Equals(dbLabel.GitlabId) ?? false));
        await DeleteRangeAsync(toBeDeleted, cancellationToken);
    }
}
