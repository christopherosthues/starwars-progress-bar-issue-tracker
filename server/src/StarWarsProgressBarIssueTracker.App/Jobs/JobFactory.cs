using StarWarsProgressBarIssueTracker.Infrastructure.Models;

namespace StarWarsProgressBarIssueTracker.App.Jobs;

public class JobFactory(IServiceProvider serviceProvider)
{
    public IJob CreateJob(JobType jobType)
    {
        switch (jobType)
        {
            case JobType.GitlabSync:
                return serviceProvider.GetRequiredService<GitlabSynchronizationJob>();
            case JobType.GitHubSync:
                return serviceProvider.GetRequiredService<GitHubSynchronizationJob>();
            default:
                throw new ArgumentOutOfRangeException(nameof(jobType));
        }
    }
}
