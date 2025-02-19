using CategoryAttribute = TUnit.Core.CategoryAttribute;

namespace StarWarsProgressBarIssueTracker.TestHelpers;

public class ErrorAttribute : CategoryAttribute
{
    public ErrorAttribute(string errorMessage) : base(TestCategory.Error)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(errorMessage);
        ErrorMessage = errorMessage;
    }

    public string ErrorMessage { get; }
}
