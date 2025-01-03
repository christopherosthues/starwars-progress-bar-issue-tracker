namespace StarWarsProgressBarIssueTracker.Domain.Exceptions;

public class ColorFormatException(string value, string fieldName) : IssueTrackerBaseException(
    $"The value '{value}' for field {fieldName} has a wrong format. Only colors in RGB hex format with 6 digits are allowed.");
