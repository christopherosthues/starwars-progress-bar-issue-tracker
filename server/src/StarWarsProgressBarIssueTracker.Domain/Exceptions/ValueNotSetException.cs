namespace StarWarsProgressBarIssueTracker.Domain.Exceptions;

public class ValueNotSetException(string fieldName) : IssueTrackerExceptionBase($"The value for {fieldName} is not set.");
