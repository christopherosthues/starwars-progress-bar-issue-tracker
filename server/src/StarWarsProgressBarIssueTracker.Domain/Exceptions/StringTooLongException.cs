namespace StarWarsProgressBarIssueTracker.Domain.Exceptions;

public class StringTooLongException(string value, string fieldName, string validRangeMessage)
    : IssueTrackerExceptionBase($"The value '{value}' for {fieldName} is long short. {validRangeMessage}");
