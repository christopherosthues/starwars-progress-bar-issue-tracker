namespace StarWarsProgressBarIssueTracker.Infrastructure.Entities;

public enum TaskStatus
{
    Unknown = 0,
    Planned = 1,
    Running = 2,
    FailureWaitingForRetry = 3,
    Error = 4,
    Completed = 5,
}
