namespace StarWarsProgressBarIssueTracker.Domain.Exceptions;

public class StringTooLongException(string value, string fieldName, string validRangeMessage)
    : IssueTrackerBaseException($"The value '{value}' for {fieldName} is long short. {validRangeMessage}");
