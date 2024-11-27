using StarWarsProgressBarIssueTracker.Domain;
using StarWarsProgressBarIssueTracker.Infrastructure.Models;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Repositories;

public interface ITaskRepository : IRepository<DbTask>
{
    Task<IEnumerable<DbTask>> GetScheduledTasksAsync(JobType jobType, CancellationToken cancellationToken = default);
}
