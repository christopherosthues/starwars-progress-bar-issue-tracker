using TUnit.Assertions.AssertConditions;

namespace StarWarsProgressBarIssueTracker.TestHelpers.TUnit;

public class DateTimeEqualToIgnoreMillisecondsCondition(DateTime expected)
    : ExpectedValueAssertCondition<DateTime, DateTime>(expected)
{
    protected override string GetExpectation()
    {
        return $"to be equal to {ExpectedValue}";
    }

    protected override ValueTask<AssertionResult> GetResult(DateTime actualValue, DateTime expectedValue)
    {
        DateTime actualDateTime = new DateTime(actualValue.Year, actualValue.Month, actualValue.Day, actualValue.Hour,
            actualValue.Minute, actualValue.Second, DateTimeKind.Utc);
        DateTime expectedDateTime = new DateTime(expectedValue.Year, expectedValue.Month, expectedValue.Day,
            expectedValue.Hour, expectedValue.Minute, expectedValue.Second, DateTimeKind.Utc);
        return AssertionResult
            .FailIf(actualDateTime != expectedDateTime,
                $"the received value {actualValue} is different");
    }
}
