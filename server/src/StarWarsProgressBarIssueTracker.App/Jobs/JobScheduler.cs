using Quartz;

namespace StarWarsProgressBarIssueTracker.App.Jobs;

[DisallowConcurrentExecution]
public class JobScheduler(JobSchedulingService jobSchedulingService, ILogger<JobScheduler> logger) : Quartz.IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            logger.LogInformation("Scheduling job executions.");
            await jobSchedulingService.ScheduleTasksAsync(context.CancellationToken);
            logger.LogInformation("Jobs scheduled.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to schedule tasks.");
        }
    }
}
