using HotChocolate.Pagination;
using Microsoft.EntityFrameworkCore;
using StarWarsProgressBarIssueTracker.Infrastructure.Database;
using StarWarsProgressBarIssueTracker.Infrastructure.Models;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Repositories;

public class JobRepository(IssueTrackerContext context) : IssueTrackerRepositoryBase<DbJob>(context)
{
    public override async Task<Page<DbJob>> GetAllAsync(PagingArguments pagingArguments,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking().OrderBy(job => job.JobType).ToPageAsync(pagingArguments, cancellationToken);
    }
}
