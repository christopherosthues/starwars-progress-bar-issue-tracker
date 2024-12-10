using StarWarsProgressBarIssueTracker.Domain.Errors;

namespace StarWarsProgressBarIssueTracker.Domain.Exceptions;

public class IssueTrackerBaseException(string message) : Exception(message)
{
    public ErrorCodes UserErrorCode { get; } // TODO: init error code
}
