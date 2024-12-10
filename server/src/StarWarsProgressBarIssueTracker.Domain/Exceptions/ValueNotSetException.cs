namespace StarWarsProgressBarIssueTracker.Domain.Exceptions;

public class ValueNotSetException(string fieldName) : IssueTrackerBaseException($"The value for {fieldName} is not set.");
