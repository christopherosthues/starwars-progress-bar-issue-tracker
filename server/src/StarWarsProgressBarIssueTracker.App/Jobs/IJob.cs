namespace StarWarsProgressBarIssueTracker.App.Jobs;

public interface IJob
{
    Task ExecuteAsync(CancellationToken cancellationToken);
}
