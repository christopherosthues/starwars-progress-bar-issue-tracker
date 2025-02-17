using System.Runtime.CompilerServices;
using StarWarsProgressBarIssueTracker.Domain.Issues;
using StarWarsProgressBarIssueTracker.Domain.Labels;
using StarWarsProgressBarIssueTracker.Domain.Vehicles;
using StarWarsProgressBarIssueTracker.TestHelpers.TUnit;
using TUnit.Assertions.AssertConditions.Interfaces;
using TUnit.Assertions.AssertionBuilders;

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

    public static InvokableValueAssertionBuilder<IEnumerable<Issue>> ContainsEquivalentOf(
        this IValueSource<List<Issue>> valueSource, Issue issue)
    {
        return valueSource.Contains(value => value.Id.Equals(issue.Id) &&
                                             value.Title.Equals(issue.Title) &&
                                             (value.Description?.Equals(issue.Description) ??
                                              issue.Description == null) &&
                                             value.Priority.Equals(issue.Priority) &&
                                             value.State.Equals(issue.State) &&
                                             (value.Milestone?.Id.Equals(issue.Milestone?.Id) ?? issue.Milestone == null) &&
                                             (value.Release?.Id.Equals(issue.Release?.Id) ?? issue.Release == null) &&
                                             (value.Vehicle?.Id.Equals(issue.Vehicle?.Id) ?? issue.Vehicle == null) &&
                                             value.Labels.Select(label => label.Id).Intersect(issue.Labels.Select(label => label.Id)).Count() == value.Labels.Count &&
                                             value.LinkedIssues.Select(linkedIssue => linkedIssue.Id).Intersect(issue.LinkedIssues.Select(linkedIssue => linkedIssue.Id)).Count() == value.LinkedIssues.Count &&
                                             (value.GitlabId?.Equals(issue.GitlabId) ?? issue.GitlabId == null) &&
                                             (value.GitHubId?.Equals(issue.GitHubId) ?? issue.GitHubId == null) &&
                                             DateTimeEquals(value.CreatedAt, issue.CreatedAt) &&
                                             DateTimeEquals(value.LastModifiedAt, issue.LastModifiedAt));
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
                                             value.Issues.Select(issue => issue.Id).Intersect(label.Issues.Select(issue => issue.Id)).Count() == value.Issues.Count &&
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
