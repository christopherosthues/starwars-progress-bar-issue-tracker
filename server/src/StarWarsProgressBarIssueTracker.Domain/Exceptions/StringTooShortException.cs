namespace StarWarsProgressBarIssueTracker.Domain.Exceptions;

public class StringTooShortException(string value, string fieldName, string validRangeMessage)
    : IssueTrackerBaseException($"The value '{value}' for {fieldName} is too short. {validRangeMessage}");
