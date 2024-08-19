using Microsoft.EntityFrameworkCore;
using Quartz;
using StarWarsProgressBarIssueTracker.Infrastructure.Database;
using StarWarsProgressBarIssueTracker.Infrastructure.Models;
using StarWarsProgressBarIssueTracker.Infrastructure.Repositories;
using TaskStatus = StarWarsProgressBarIssueTracker.Infrastructure.Models.TaskStatus;

namespace StarWarsProgressBarIssueTracker.App.Jobs;

public class JobSchedulingService
{
    private readonly IRepository<DbJob> _jobRepository;
    private readonly ITaskRepository _taskRepository;

    public JobSchedulingService(IRepository<DbJob> jobRepository, ITaskRepository taskRepository, IssueTrackerContext dbContext)
    {
        _jobRepository = jobRepository;
        _jobRepository.Context = dbContext;
        _taskRepository = taskRepository;
        _taskRepository.Context = dbContext;
    }

    public async Task ScheduleTasksAsync(CancellationToken cancellationToken)
    {
        var jobs = await _jobRepository.GetAll().ToListAsync(cancellationToken);

        IList<Exception> exceptions = [];

        foreach (var job in jobs)
        {
            if (!job.IsPaused && ShouldSchedule(job))
            {
                try
                {
                    var task = new DbTask
                    {
                        ExecuteAt = DateTime.UtcNow,
                        Job = job,
                        Status = TaskStatus.Planned
                    };
                    await _taskRepository.AddAsync(task, cancellationToken);

                    job.NextExecution = DateTime.UtcNow;
                    await _jobRepository.UpdateAsync(job, cancellationToken);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }
        }

        if (exceptions.Count > 0)
        {
            throw new AggregateException(exceptions);
        }
    }

    private static bool ShouldSchedule(DbJob job)
    {
        if (job.NextExecution is null)
        {
            return true;
        }

        var cronExpression = new CronExpression(job.CronInterval);

        var nextScheduledExecution = cronExpression.GetNextValidTimeAfter(job.NextExecution.Value);
        return nextScheduledExecution!.Value.Date <= DateTime.UtcNow;
    }
}
