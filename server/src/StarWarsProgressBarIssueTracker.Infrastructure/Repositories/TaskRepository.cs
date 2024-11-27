using HotChocolate.Pagination;
using Microsoft.EntityFrameworkCore;
using StarWarsProgressBarIssueTracker.Infrastructure.Database;
using StarWarsProgressBarIssueTracker.Infrastructure.Models;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Repositories;

public class TaskRepository(IssueTrackerContext context) : IssueTrackerRepositoryBase<DbTask>(context), ITaskRepository
{
    public override async Task<Page<DbTask>> GetAllAsync(PagingArguments pagingArguments,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking()
            .Include(task => task.Job)
            .OrderBy(task => task.ExecuteAt)
            .ThenBy(task => task.Id)
            .ToPageAsync(pagingArguments, cancellationToken);
    }

    public async Task<IEnumerable<DbTask>> GetScheduledTasksAsync(JobType jobType,
        CancellationToken cancellationToken = default)
    {
        return await GetIncludingFields().Where(task => task.Job.JobType == jobType &&
                                                        task.Status != Models.TaskStatus.Unknown &&
                                                        task.Status != Models.TaskStatus.Completed &&
                                                        task.Status != Models.TaskStatus.Error)
            .ToListAsync(cancellationToken);
    }

    protected override IQueryable<DbTask> GetIncludingFields()
    {
        return DbSet.Include(task => task.Job);
    }
}
