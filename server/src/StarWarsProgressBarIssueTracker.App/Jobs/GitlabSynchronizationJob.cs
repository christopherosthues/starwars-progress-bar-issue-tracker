using System.Text.Json;
using StarWarsProgressBarIssueTracker.Domain.Issues;
using StarWarsProgressBarIssueTracker.Domain.Labels;
using StarWarsProgressBarIssueTracker.Domain.Milestones;
using StarWarsProgressBarIssueTracker.Domain.Releases;
using StarWarsProgressBarIssueTracker.Domain.Vehicles;
using StarWarsProgressBarIssueTracker.Infrastructure.Gitlab.GraphQL;
using StarWarsProgressBarIssueTracker.Infrastructure.Gitlab.Models;
using StarWarsProgressBarIssueTracker.Infrastructure.Gitlab.Networking;
using StarWarsProgressBarIssueTracker.Infrastructure.Gitlab.Networking.Models;
using StarWarsProgressBarIssueTracker.Infrastructure.Gitlab.Services;
using IssueState = StarWarsProgressBarIssueTracker.Infrastructure.Gitlab.GraphQL.IssueState;

namespace StarWarsProgressBarIssueTracker.App.Jobs;

public class GitlabSynchronizationJob(
    GitlabSynchronizationService gitlabSynchronizationService
)
    : IJob
{
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await gitlabSynchronizationService.SynchronizeAsync(cancellationToken);
    }
}
