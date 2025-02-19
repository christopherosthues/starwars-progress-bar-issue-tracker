using System.Runtime.CompilerServices;
using StarWarsProgressBarIssueTracker.Domain.Issues;
using StarWarsProgressBarIssueTracker.Domain.Labels;
using StarWarsProgressBarIssueTracker.Domain.Milestones;
using StarWarsProgressBarIssueTracker.Domain.Releases;
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
                                             (value.Milestone?.Id.Equals(issue.Milestone?.Id) ??
                                              issue.Milestone == null) &&
                                             (value.Release?.Id.Equals(issue.Release?.Id) ?? issue.Release == null) &&
                                             (value.Vehicle?.Id.Equals(issue.Vehicle?.Id) ?? issue.Vehicle == null) &&
                                             value.Labels.Select(label => label.Id)
                                                 .Intersect(issue.Labels.Select(label => label.Id)).Count() ==
                                             value.Labels.Count &&
                                             value.LinkedIssues.Select(linkedIssue => linkedIssue.Id)
                                                 .Intersect(issue.LinkedIssues.Select(linkedIssue => linkedIssue.Id))
                                                 .Count() == value.LinkedIssues.Count &&
                                             (value.GitlabId?.Equals(issue.GitlabId) ?? issue.GitlabId == null) &&
                                             (value.GitlabIid?.Equals(issue.GitlabIid) ?? issue.GitlabIid == null) &&
                                             (value.GitHubId?.Equals(issue.GitHubId) ?? issue.GitHubId == null) &&
                                             DateTimeEquals(value.CreatedAt, issue.CreatedAt) &&
                                             DateTimeEquals(value.LastModifiedAt, issue.LastModifiedAt));
    }

    public static InvokableValueAssertionBuilder<IEnumerable<Milestone>> ContainsEquivalentOf(
        this IValueSource<List<Milestone>> valueSource, Milestone milestone)
    {
        return valueSource.Contains(value => value.Id.Equals(milestone.Id) &&
                                             value.Title.Equals(milestone.Title) &&
                                             (value.Description?.Equals(milestone.Description) ??
                                              milestone.Description == null) &&
                                             value.State.Equals(milestone.State) &&
                                             value.Issues.Select(issue => issue.Id).Intersect(milestone.Issues.Select(issue => issue.Id)).Count() == value.Issues.Count &&
                                             (value.GitlabId?.Equals(milestone.GitlabId) ?? milestone.GitlabId == null) &&
                                             (value.GitlabIid?.Equals(milestone.GitlabIid) ?? milestone.GitlabIid == null) &&
                                             (value.GitHubId?.Equals(milestone.GitHubId) ?? milestone.GitHubId == null) &&
                                             DateTimeEquals(value.CreatedAt, milestone.CreatedAt) &&
                                             DateTimeEquals(value.LastModifiedAt, milestone.LastModifiedAt));
    }

    public static InvokableValueAssertionBuilder<IEnumerable<Release>> ContainsEquivalentOf(
        this IValueSource<List<Release>> valueSource, Release release)
    {
        return valueSource.Contains(value => value.Id.Equals(release.Id) &&
                                             value.Title.Equals(release.Title) &&
                                             (value.Notes?.Equals(release.Notes) ?? release.Notes == null) &&
                                             value.State.Equals(release.State) &&
                                             DateTimeEquals(value.Date, release.Date) &&
                                             value.Issues.Select(issue => issue.Id).Intersect(release.Issues.Select(issue => issue.Id)).Count() == value.Issues.Count &&
                                             (value.GitlabId?.Equals(release.GitlabId) ?? release.GitlabId == null) &&
                                             (value.GitlabIid?.Equals(release.GitlabIid) ?? release.GitlabIid == null) &&
                                             (value.GitHubId?.Equals(release.GitHubId) ?? release.GitHubId == null) &&
                                             DateTimeEquals(value.CreatedAt, release.CreatedAt) &&
                                             DateTimeEquals(value.LastModifiedAt, release.LastModifiedAt));
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
