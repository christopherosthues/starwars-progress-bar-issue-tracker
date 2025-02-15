using TUnit.Assertions.AssertConditions;

namespace StarWarsProgressBarIssueTracker.TestHelpers.TUnit;

public class NullableDateTimeEqualToIgnoreMillisecondsCondition(DateTime? expected)
    : ExpectedValueAssertCondition<DateTime?, DateTime?>(expected)
{
    protected override string GetExpectation()
    {
        return $"to be equal to {ExpectedValue}";
    }

    protected override Task<AssertionResult> GetResult(DateTime? actualValue, DateTime? expectedValue)
    {
        if (actualValue is not null && expectedValue is not null)
        {
            DateTime actualDateTime = new DateTime(actualValue.Value.Year, actualValue.Value.Month,
                actualValue.Value.Day, actualValue.Value.Hour, actualValue.Value.Minute, actualValue.Value.Second,
                DateTimeKind.Utc);
            DateTime expectedDateTime = new DateTime(expectedValue.Value.Year, expectedValue.Value.Month,
                expectedValue.Value.Day, expectedValue.Value.Hour, expectedValue.Value.Minute,
                expectedValue.Value.Second, DateTimeKind.Utc);
            return AssertionResult.FailIf(actualDateTime != expectedDateTime,
                $"the received value {actualValue} is different");
        }

        return AssertionResult.FailIf(actualValue != ExpectedValue, $"the received value {actualValue} is different");
    }
}
