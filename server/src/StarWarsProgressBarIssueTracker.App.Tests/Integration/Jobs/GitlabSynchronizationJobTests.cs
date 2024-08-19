using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StarWarsProgressBarIssueTracker.App.Jobs;
using StarWarsProgressBarIssueTracker.Common.Tests;
using StarWarsProgressBarIssueTracker.Infrastructure.Models;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

namespace StarWarsProgressBarIssueTracker.App.Tests.Integration.Jobs;

[TestFixture(TestOf = typeof(GitlabSynchronizationJob))]
[Category(TestCategory.Integration)]
public class GitlabSynchronizationJobTests : IntegrationTestBase
{
    private WireMockServer _server = default!;

    [SetUp]
    public void SetUp()
    {
        _server = WireMockServer.Start(new WireMockServerSettings
        {
            Port = 8081,
            UseSSL = true,
        });
    }

    [TearDown]
    public void TearDown()
    {
        _server.Stop();
        _server.Dispose();
    }

    [Test]
    public async Task ExecuteAsyncShouldUpdateLabels()
    {
        // Arrange
        var expectedDbLabels = GitlabMockData.AddedLabels();
        var dbIssue = new DbIssue { Title = "NotDeleted", };
        var deletedLabel = new DbLabel
        {
            Title = "Deleted",
            Color = "#fffff1",
            TextColor = "#fffff1",
            GitlabId = "gid://gitlab/ProjectLabel/4",
            Issues = [dbIssue]
        };
        dbIssue.Labels.Add(deletedLabel);
        var githubLabel = new DbLabel { Title = "GitHub", Color = "#fffffe", TextColor = "#fffffe", GitHubId = "gid://github/ProjectLabel/5" };
        expectedDbLabels.Add(githubLabel);
        await SeedDatabaseAsync(context =>
        {
            context.Labels.Add(expectedDbLabels[0]);
            context.Labels.Add(deletedLabel);
            context.Labels.Add(githubLabel);
            context.Issues.Add(dbIssue);
        });
        using var scope = ApiFactory.Services.CreateScope();
        var job = scope.ServiceProvider.GetRequiredService<GitlabSynchronizationJob>();
        _server.Given(Request.Create().WithPath("/api/graphql").UsingPost())
            .RespondWith(Response.Create().WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody(GitlabMockData.LabelResponse));

        CheckDbContent(context =>
        {
            context.Labels.Should().ContainEquivalentOf(deletedLabel,
                options => options.Excluding(dbLabel => dbLabel.Id).Excluding(dbLabel => dbLabel.CreatedAt)
                    .Excluding(dbLabel => dbLabel.Issues));
            context.Labels.Should().ContainEquivalentOf(githubLabel,
                options => options.Excluding(dbLabel => dbLabel.Id).Excluding(dbLabel => dbLabel.CreatedAt));
            context.Labels.Should().ContainEquivalentOf(expectedDbLabels[0],
                options => options.Excluding(dbLabel => dbLabel.Id).Excluding(dbLabel => dbLabel.CreatedAt));
            context.Issues.Should().ContainEquivalentOf(dbIssue,
                options => options.Excluding(issue => issue.Id).Excluding(issue => issue.CreatedAt)
                    .Excluding(issue => issue.Labels));
            context.Issues.Include(entity => entity.Labels).First().Labels.Should().NotBeEmpty();
        });

        // Act
        await job.ExecuteAsync(CancellationToken.None);

        // Assert
        CheckDbContent(context =>
        {
            context.Labels.Should().NotBeEmpty();
            var resultLabels = context.Labels.ToList();

            resultLabels.Should().HaveCount(expectedDbLabels.Count);
            resultLabels.Should().NotContainEquivalentOf(deletedLabel,
                options => options.Excluding(dbLabel => dbLabel.Id).Excluding(dbLabel => dbLabel.CreatedAt)
                    .Excluding(dbLabel => dbLabel.Issues));
            resultLabels.Should().BeEquivalentTo(expectedDbLabels,
                options => options.Excluding(dbLabel => dbLabel.Id).Excluding(dbLabel => dbLabel.CreatedAt));

            var issues = context.Issues.Include(issue => issue.Labels).ToList();
            issues.Should().ContainEquivalentOf(dbIssue, options => options.Excluding(issue => issue.Id)
                .Excluding(issue => issue.CreatedAt).Excluding(issue => issue.Labels));
            issues[0].Labels.Should().BeEmpty();
        });
    }

    [Test]
    public async Task ExecuteAsyncShouldUpdateAppearances()
    {
        // Arrange
        var expectedDbAppearances = GitlabMockData.AddedAppearances();
        var dbIssue = new DbIssue { Title = "NotDeleted", };
        var deletedAppearance = new DbAppearance
        {
            Title = "Deleted",
            Color = "#fffff1",
            TextColor = "#fffff1",
            GitlabId = "gid://gitlab/ProjectLabel/7",
        };
        var dbVehicle = new DbVehicle
        {
            Appearances = [deletedAppearance]
        };
        dbIssue.Vehicle = dbVehicle;
        var githubAppearance = new DbAppearance { Title = "GitHub", Color = "#fffffe", TextColor = "#fffffe", GitHubId = "gid://github/ProjectLabel/8" };
        expectedDbAppearances.Add(githubAppearance);
        await SeedDatabaseAsync(context =>
        {
            context.Appearances.Add(expectedDbAppearances[0]);
            context.Appearances.Add(deletedAppearance);
            context.Appearances.Add(githubAppearance);
            context.Issues.Add(dbIssue);
        });
        using var scope = ApiFactory.Services.CreateScope();
        var job = scope.ServiceProvider.GetRequiredService<GitlabSynchronizationJob>();
        _server.Given(Request.Create().WithPath("/api/graphql").UsingPost())
            .RespondWith(Response.Create().WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody(GitlabMockData.AppearanceResponse));

        CheckDbContent(context =>
        {
            context.Appearances.Should().ContainEquivalentOf(deletedAppearance,
                options => options.Excluding(dbAppearance => dbAppearance.Id).Excluding(dbAppearance => dbAppearance.CreatedAt));
            context.Appearances.Should().ContainEquivalentOf(githubAppearance,
                options => options.Excluding(dbAppearance => dbAppearance.Id).Excluding(dbAppearance => dbAppearance.CreatedAt));
            context.Appearances.Should().ContainEquivalentOf(expectedDbAppearances[0],
                options => options.Excluding(dbAppearance => dbAppearance.Id).Excluding(dbAppearance => dbAppearance.CreatedAt));
            context.Issues.Should().ContainEquivalentOf(dbIssue,
                options => options.Excluding(issue => issue.Id).Excluding(issue => issue.CreatedAt)
                    .Excluding(issue => issue.Vehicle));
            context.Issues.Include(entity => entity.Vehicle).ThenInclude(entity => entity!.Appearances).First().Vehicle!.Appearances.Should().NotBeEmpty();
        });

        // Act
        await job.ExecuteAsync(CancellationToken.None);

        // Assert
        CheckDbContent(context =>
        {
            context.Appearances.Should().NotBeEmpty();
            var resultAppearances = context.Appearances.ToList();

            resultAppearances.Should().HaveCount(expectedDbAppearances.Count);
            resultAppearances.Should().NotContainEquivalentOf(deletedAppearance,
                options => options.Excluding(dbAppearance => dbAppearance.Id).Excluding(dbAppearance => dbAppearance.CreatedAt));
            resultAppearances.Should().BeEquivalentTo(expectedDbAppearances,
                options => options.Excluding(dbAppearance => dbAppearance.Id).Excluding(dbAppearance => dbAppearance.CreatedAt));

            var issues = context.Issues.Include(issue => issue.Vehicle).ThenInclude(vehicle => vehicle!.Appearances).ToList();
            issues.Should().ContainEquivalentOf(dbIssue, options => options.Excluding(issue => issue.Id)
                .Excluding(issue => issue.CreatedAt).Excluding(issue => issue.Vehicle));
            issues[0].Vehicle!.Appearances.Should().BeEmpty();
        });
    }

    [Test]
    public async Task ExecuteAsyncShouldUpdateMilestones()
    {
        // Arrange
        var expectedDbMilestones = GitlabMockData.AddedMilestones();
        var dbIssue = new DbIssue { Title = "NotDeleted", };
        var deletedMilestone = new DbMilestone
        {
            Title = "Deleted",
            GitlabId = "gid://gitlab/Milestone/4",
            GitlabIid = "4",
            Issues = [dbIssue]
        };
        dbIssue.Milestone = deletedMilestone;
        var githubMilestone = new DbMilestone { Title = "GitHub", GitHubId = "gid://github/Milestone/5", };
        expectedDbMilestones.Add(githubMilestone);
        await SeedDatabaseAsync(context =>
        {
            context.Milestones.Add(expectedDbMilestones[0]);
            context.Milestones.Add(deletedMilestone);
            context.Milestones.Add(githubMilestone);
            context.Issues.Add(dbIssue);
        });
        using var scope = ApiFactory.Services.CreateScope();
        var job = scope.ServiceProvider.GetRequiredService<GitlabSynchronizationJob>();
        _server.Given(Request.Create().WithPath("/api/graphql").UsingPost())
            .RespondWith(Response.Create().WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody(GitlabMockData.MilestoneResponse));

        CheckDbContent(context =>
        {
            context.Milestones.Should().ContainEquivalentOf(deletedMilestone,
                options => options.Excluding(dbMilestone => dbMilestone.Id).Excluding(dbMilestone => dbMilestone.CreatedAt)
                    .Excluding(dbMilestone => dbMilestone.Issues));
            context.Milestones.Should().ContainEquivalentOf(githubMilestone,
                options => options.Excluding(dbMilestone => dbMilestone.Id).Excluding(dbMilestone => dbMilestone.CreatedAt));
            context.Milestones.Should().ContainEquivalentOf(expectedDbMilestones[0],
                options => options.Excluding(dbMilestone => dbMilestone.Id).Excluding(dbMilestone => dbMilestone.CreatedAt));
            context.Issues.Should().ContainEquivalentOf(dbIssue,
                options => options.Excluding(issue => issue.Id).Excluding(issue => issue.CreatedAt)
                    .Excluding(issue => issue.Milestone));
            context.Issues.Include(entity => entity.Milestone).First().Milestone.Should().NotBeNull();
        });

        // Act
        await job.ExecuteAsync(CancellationToken.None);

        // Assert
        CheckDbContent(context =>
        {
            context.Milestones.Should().NotBeEmpty();
            var resultMilestones = context.Milestones.ToList();

            resultMilestones.Should().HaveCount(expectedDbMilestones.Count);
            resultMilestones.Should().NotContainEquivalentOf(deletedMilestone,
                options => options.Excluding(dbMilestone => dbMilestone.Id).Excluding(dbMilestone => dbMilestone.CreatedAt)
                    .Excluding(dbMilestone => dbMilestone.Issues));
            resultMilestones.Should().BeEquivalentTo(expectedDbMilestones,
                options => options.Excluding(dbMilestone => dbMilestone.Id).Excluding(dbMilestone => dbMilestone.CreatedAt));

            var issues = context.Issues.Include(issue => issue.Milestone).ToList();
            issues.Should().ContainEquivalentOf(dbIssue, options => options.Excluding(issue => issue.Id)
                .Excluding(issue => issue.LastModifiedAt).Excluding(issue => issue.CreatedAt)
                .Excluding(issue => issue.Milestone));
            issues[0].Milestone.Should().BeNull();
        });
    }

    [Test]
    public async Task ExecuteAsyncShouldUpdateReleases()
    {
        // Arrange
        var expectedDbReleases = GitlabMockData.AddedReleases();
        var dbIssue = new DbIssue { Title = "NotDeleted", };
        var deletedRelease = new DbRelease
        {
            Title = "Deleted",
            GitlabId = "gid://gitlab/Issue/4",
            GitlabIid = "4",
            Issues = [dbIssue]
        };
        dbIssue.Release = deletedRelease;
        var githubRelease = new DbRelease { Title = "GitHub", GitHubId = "gid://github/Issue/5", };
        expectedDbReleases.Add(githubRelease);
        await SeedDatabaseAsync(context =>
        {
            context.Releases.Add(expectedDbReleases[0]);
            context.Releases.Add(deletedRelease);
            context.Releases.Add(githubRelease);
            context.Issues.Add(dbIssue);
        });
        using var scope = ApiFactory.Services.CreateScope();
        var job = scope.ServiceProvider.GetRequiredService<GitlabSynchronizationJob>();
        _server.Given(Request.Create().WithPath("/api/graphql").UsingPost())
            .RespondWith(Response.Create().WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody(GitlabMockData.ReleaseResponse));

        CheckDbContent(context =>
        {
            context.Releases.Should().ContainEquivalentOf(deletedRelease,
                options => options.Excluding(dbRelease => dbRelease.Id).Excluding(dbRelease => dbRelease.CreatedAt)
                    .Excluding(dbRelease => dbRelease.Issues));
            context.Releases.Should().ContainEquivalentOf(githubRelease,
                options => options.Excluding(dbRelease => dbRelease.Id).Excluding(dbRelease => dbRelease.CreatedAt));
            context.Releases.Should().ContainEquivalentOf(expectedDbReleases[0],
                options => options.Excluding(dbRelease => dbRelease.Id).Excluding(dbRelease => dbRelease.CreatedAt));
            context.Issues.Should().ContainEquivalentOf(dbIssue,
                options => options.Excluding(issue => issue.Id).Excluding(issue => issue.CreatedAt)
                    .Excluding(issue => issue.Release));
            context.Issues.Include(entity => entity.Release).First().Release.Should().NotBeNull();
        });

        // Act
        await job.ExecuteAsync(CancellationToken.None);

        // Assert
        CheckDbContent(context =>
        {
            context.Releases.Should().NotBeEmpty();
            var resultReleases = context.Releases.ToList();

            resultReleases.Should().HaveCount(expectedDbReleases.Count);
            resultReleases.Should().NotContainEquivalentOf(deletedRelease,
                options => options.Excluding(dbRelease => dbRelease.Id).Excluding(dbRelease => dbRelease.CreatedAt)
                    .Excluding(dbRelease => dbRelease.Issues));
            resultReleases.Should().BeEquivalentTo(expectedDbReleases,
                options => options.Excluding(dbRelease => dbRelease.Id).Excluding(dbRelease => dbRelease.CreatedAt));

            var issues = context.Issues.Include(issue => issue.Release).ToList();
            issues.Should().ContainEquivalentOf(dbIssue, options => options.Excluding(issue => issue.Id)
                .Excluding(issue => issue.LastModifiedAt).Excluding(issue => issue.CreatedAt)
                .Excluding(issue => issue.Release));
            issues[0].Release.Should().BeNull();
        });
    }
}
