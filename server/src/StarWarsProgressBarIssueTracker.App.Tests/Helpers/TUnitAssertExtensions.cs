using System.Runtime.CompilerServices;
using StarWarsProgressBarIssueTracker.Domain.Labels;
using StarWarsProgressBarIssueTracker.Domain.Vehicles;
using StarWarsProgressBarIssueTracker.TestHelpers.TUnit;
using TUnit.Assertions.AssertConditions.Chronology;
using TUnit.Assertions.AssertConditions.Interfaces;
using TUnit.Assertions.AssertionBuilders;
using TUnit.Assertions.AssertionBuilders.Wrappers;

namespace StarWarsProgressBarIssueTracker.App.Tests.Helpers;

public static class TUnitAssertExtensions
{
    public static InvokableValueAssertionBuilder<IEnumerable<Appearance>> ContainsEquivalentOf(
        this IValueSource<List<Appearance>> valueSource, Appearance appearance)
    {
        return valueSource.Contains(value => value.Id.Equals(appearance.Id) &&
                                             value.Title.Equals(appearance.Title) &&
                                             (value.Description?.Equals(appearance.Description) ?? appearance.Description == null) &&
                                             value.TextColor.Equals(appearance.TextColor) &&
                                             value.Color.Equals(appearance.Color) &&
                                             (value.GitlabId?.Equals(appearance.GitlabId) ?? appearance.GitlabId == null) &&
                                             (value.GitHubId?.Equals(appearance.GitHubId) ?? appearance.GitHubId == null) &&
                                             DateTimeEquals(value.CreatedAt, appearance.CreatedAt) &&
                                             DateTimeEquals(value.LastModifiedAt, appearance.LastModifiedAt));
    }

    public static InvokableValueAssertionBuilder<IEnumerable<Label>> ContainsEquivalentOf(
        this IValueSource<List<Label>> valueSource, Label label)
    {
        return valueSource.Contains(value => value.Id.Equals(label.Id) &&
                                             value.Title.Equals(label.Title) &&
                                             (value.Description?.Equals(label.Description) ??
                                              label.Description == null) &&
                                             value.TextColor.Equals(label.TextColor) &&
                                             value.Color.Equals(label.Color) &&
                                             (value.GitlabId?.Equals(label.GitlabId) ?? label.GitlabId == null) &&
                                             (value.GitHubId?.Equals(label.GitHubId) ?? label.GitHubId == null) &&
                                             value.Issues.Intersect(label.Issues).Count() == value.Issues.Count &&
                                             DateTimeEquals(value.CreatedAt, label.CreatedAt) &&
                                             DateTimeEquals(value.LastModifiedAt, label.LastModifiedAt));
    }

    public static InvokableValueAssertionBuilder<DateTime> IsEquivalentTo(this IValueSource<DateTime> valueSource,
        DateTime expected, [CallerArgumentExpression(nameof(expected))] string? doNotPopulateThisValue1 = null)
    {
        return valueSource.RegisterAssertion(new DateTimeEqualToIgnoreMillisecondsCondition(expected),
            [doNotPopulateThisValue1]);
    }

    public static InvokableValueAssertionBuilder<DateTime?> IsEquivalentTo(this IValueSource<DateTime?> valueSource,
        DateTime? expected, [CallerArgumentExpression(nameof(expected))] string? doNotPopulateThisValue1 = null)
    {
        return valueSource.RegisterAssertion(new NullableDateTimeEqualToIgnoreMillisecondsCondition(expected),
            [doNotPopulateThisValue1]);
    }

    public static InvokableValueAssertionBuilder<DateTime?> IsEquivalentTo(this IValueSource<DateTime?> valueSource,
        DateTime expected, [CallerArgumentExpression(nameof(expected))] string? doNotPopulateThisValue1 = null)
    {
        return valueSource.RegisterAssertion(new NullableDateTimeEqualToIgnoreMillisecondsCondition(expected),
            [doNotPopulateThisValue1]);
    }

    private static bool DateTimeEquals(this DateTime? actual, DateTime? expected)
    {
        if (actual == null && expected == null)
        {
            return true;
        }

        if (actual == null && expected != null)
        {
            return false;
        }

        if (actual != null && expected == null)
        {
            return false;
        }

        DateTime actualDateTime = new DateTime(actual!.Value.Year, actual.Value.Month, actual.Value.Day,
            actual.Value.Hour, actual.Value.Minute, actual.Value.Second, DateTimeKind.Utc);
        DateTime expectedDateTime = new DateTime(expected!.Value.Year, expected.Value.Month, expected.Value.Day,
            expected.Value.Hour, expected.Value.Minute, expected.Value.Second, DateTimeKind.Utc);
        return actualDateTime.Equals(expectedDateTime);
    }
}
