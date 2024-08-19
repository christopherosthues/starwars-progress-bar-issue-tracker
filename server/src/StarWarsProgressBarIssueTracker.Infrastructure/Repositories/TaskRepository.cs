using Microsoft.EntityFrameworkCore;
using StarWarsProgressBarIssueTracker.Infrastructure.Models;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Repositories;

public class TaskRepository : IssueTrackerRepositoryBase<DbTask>, ITaskRepository
{
    public async Task<IEnumerable<DbTask>> GetScheduledTasksAsync(JobType jobType, CancellationToken cancellationToken = default)
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
