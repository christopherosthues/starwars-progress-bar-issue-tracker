using System.ComponentModel;

namespace StarWarsProgressBarIssueTracker.TestHelpers;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class ErrorAttribute : CategoryAttribute
{
    public ErrorAttribute(string errorMessage) : base(TestCategory.Error)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(errorMessage);
        ErrorMessage = errorMessage;
    }

    public string ErrorMessage { get; }
}
