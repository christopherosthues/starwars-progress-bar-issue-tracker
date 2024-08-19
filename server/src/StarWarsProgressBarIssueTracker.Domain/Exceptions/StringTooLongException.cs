namespace StarWarsProgressBarIssueTracker.Domain.Exceptions;

public class StringTooLongException(string value, string fieldName, string validRangeMessage)
    : Exception($"The value '{value}' for {fieldName} is long short. {validRangeMessage}");
