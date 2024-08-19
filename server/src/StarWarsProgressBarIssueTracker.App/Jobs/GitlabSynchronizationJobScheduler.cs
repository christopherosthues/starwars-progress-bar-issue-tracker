using Quartz;
using StarWarsProgressBarIssueTracker.Infrastructure.Models;

namespace StarWarsProgressBarIssueTracker.App.Jobs;

[DisallowConcurrentExecution]
public class GitlabSynchronizationJobScheduler(JobExecutionService jobExecutionService, ILogger<GitlabSynchronizationJobScheduler> logger) : Quartz.IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            logger.LogInformation("Executing pending Gitlab synchronization tasks.");
            await jobExecutionService.ExecuteTask(JobType.GitlabSync, context.CancellationToken);
            logger.LogInformation("Gitlab synchronization tasks executed.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to execute Gitlab synchronization tasks.");
        }
    }
}
