using Quartz;
using StarWarsProgressBarIssueTracker.Infrastructure.Models;

namespace StarWarsProgressBarIssueTracker.App.Jobs;

[DisallowConcurrentExecution]
public class GitHubSynchronizationJobScheduler(JobExecutionService jobExecutionService, ILogger<GitHubSynchronizationJobScheduler> logger) : Quartz.IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            logger.LogInformation("Executing pending GitHub synchronization tasks.");
            await jobExecutionService.ExecuteTask(JobType.GitHubSync, context.CancellationToken);
            logger.LogInformation("GitHub synchronization tasks executed.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to execute GitHub synchronization tasks.");
        }
    }
}
