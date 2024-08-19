namespace StarWarsProgressBarIssueTracker.Domain.Exceptions;

public class ValueNotSetException(string fieldName) : Exception($"The value for {fieldName} is not set.");
