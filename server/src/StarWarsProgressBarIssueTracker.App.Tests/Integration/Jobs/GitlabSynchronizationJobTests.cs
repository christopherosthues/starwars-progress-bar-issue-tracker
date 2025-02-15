using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StarWarsProgressBarIssueTracker.App.Jobs;
using StarWarsProgressBarIssueTracker.Domain.Issues;
using StarWarsProgressBarIssueTracker.Domain.Labels;
using StarWarsProgressBarIssueTracker.Domain.Milestones;
using StarWarsProgressBarIssueTracker.Domain.Releases;
using StarWarsProgressBarIssueTracker.Domain.Vehicles;
using StarWarsProgressBarIssueTracker.TestHelpers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

namespace StarWarsProgressBarIssueTracker.App.Tests.Integration.Jobs;

[Category(TestCategory.Integration)]
[Skip("Needs to be fixed after Gitlab synchronization is migrated")]
public class GitlabSynchronizationJobTests : IntegrationTestBase
{
    private WireMockServer _server = null!;

    [Before(Test)]
    public void SetUp()
    {
        _server = WireMockServer.Start(new WireMockServerSettings
        {
            Port = 8081,
            UseSSL = true,
        });
    }

    [After(Test)]
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
        var dbIssue = new Issue { Title = "NotDeleted", };
        var deletedLabel = new Label
        {
            Title = "Deleted",
            Color = "#fffff1",
            TextColor = "#fffff1",
            GitlabId = "gid://gitlab/ProjectLabel/4",
            Issues = [dbIssue]
        };
        dbIssue.Labels.Add(deletedLabel);
        var githubLabel = new Label { Title = "GitHub", Color = "#fffffe", TextColor = "#fffffe", GitHubId = "gid://github/ProjectLabel/5" };
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

        await CheckDbContentAsync(async context =>
        {
            using (Assert.Multiple())
            {
                await Assert.That(context.Labels).Contains(deletedLabel);
                // context.Labels.Should().ContainEquivalentOf(deletedLabel,
                // options => options.Excluding(dbLabel => dbLabel.Id).Excluding(dbLabel => dbLabel.CreatedAt)
                // .Excluding(dbLabel => dbLabel.Issues));
                await Assert.That(context.Labels).Contains(githubLabel);
                // context.Labels.Should().ContainEquivalentOf(githubLabel,
                // options => options.Excluding(dbLabel => dbLabel.Id).Excluding(dbLabel => dbLabel.CreatedAt));
                await Assert.That(context.Labels).Contains(expectedDbLabels[0]);
                // context.Labels.Should().ContainEquivalentOf(expectedDbLabels[0],
                // options => options.Excluding(dbLabel => dbLabel.Id).Excluding(dbLabel => dbLabel.CreatedAt));
                await Assert.That(context.Issues).Contains(dbIssue);
                // context.Issues.Should().ContainEquivalentOf(dbIssue,
                // options => options.Excluding(issue => issue.Id).Excluding(issue => issue.CreatedAt)
                // .Excluding(issue => issue.Labels));
                await Assert.That(context.Issues.Include(entity => entity.Labels).First().Labels).IsNotEmpty();
            }
        });

        // Act
        await job.ExecuteAsync(CancellationToken.None);

        // Assert
        await CheckDbContentAsync(async context =>
        {
            var resultLabels = context.Labels.ToList();
            var issues = context.Issues.Include(issue => issue.Labels).ToList();
            using (Assert.Multiple())
            {
                await Assert.That(context.Labels).IsNotEmpty();
                await Assert.That(resultLabels.Count).IsEqualTo(expectedDbLabels.Count);
                await Assert.That(resultLabels).DoesNotContain(deletedLabel);
                // resultLabels.Should().NotContainEquivalentOf(deletedLabel,
                // options => options.Excluding(dbLabel => dbLabel.Id).Excluding(dbLabel => dbLabel.CreatedAt)
                // .Excluding(dbLabel => dbLabel.Issues));
                await Assert.That(resultLabels).IsEquivalentTo(expectedDbLabels);
                // resultLabels.Should().BeEquivalentTo(expectedDbLabels,
                // options => options.Excluding(dbLabel => dbLabel.Id).Excluding(dbLabel => dbLabel.CreatedAt));
                await Assert.That(issues).Contains(dbIssue);
                // issues.Should().ContainEquivalentOf(dbIssue, options => options.Excluding(issue => issue.Id)
                // .Excluding(issue => issue.CreatedAt).Excluding(issue => issue.Labels));
                await Assert.That(issues[0].Labels).IsEmpty();
            }
        });
    }

    [Test]
    public async Task ExecuteAsyncShouldUpdateAppearances()
    {
        // Arrange
        var expectedAppearances = GitlabMockData.AddedAppearances();
        var dbIssue = new Issue { Title = "NotDeleted", };
        var deletedAppearance = new Appearance
        {
            Title = "Deleted",
            Color = "#fffff1",
            TextColor = "#fffff1",
            GitlabId = "gid://gitlab/ProjectLabel/7",
        };
        var dbVehicle = new Vehicle
        {
            Appearances = [deletedAppearance]
        };
        dbIssue.Vehicle = dbVehicle;
        var githubAppearance = new Appearance { Title = "GitHub", Color = "#fffffe", TextColor = "#fffffe", GitHubId = "gid://github/ProjectLabel/8" };
        expectedAppearances.Add(githubAppearance);
        await SeedDatabaseAsync(context =>
        {
            context.Appearances.Add(expectedAppearances[0]);
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

        await CheckDbContentAsync(async context =>
        {
            var resultAppearances = context.Appearances.ToList();
            var dbIssues = context.Issues.Include(entity => entity.Vehicle).ThenInclude(entity => entity!.Appearances).ToList();
            using (Assert.Multiple())
            {
                await Assert.That(resultAppearances).Contains(deletedAppearance);
                // context.Appearances.Should().ContainEquivalentOf(deletedAppearance,
                // options => options.Excluding(dbAppearance => dbAppearance.Id).Excluding(dbAppearance => dbAppearance.CreatedAt));
                await Assert.That(resultAppearances).Contains(githubAppearance);
                // context.Appearances.Should().ContainEquivalentOf(githubAppearance,
                // options => options.Excluding(dbAppearance => dbAppearance.Id).Excluding(dbAppearance => dbAppearance.CreatedAt));
                await Assert.That(resultAppearances).Contains(expectedAppearances[0]);
                // context.Appearances.Should().ContainEquivalentOf(expectedAppearances[0],
                // options => options.Excluding(dbAppearance => dbAppearance.Id).Excluding(dbAppearance => dbAppearance.CreatedAt));
                await Assert.That(dbIssues).Contains(dbIssue);
                // context.Issues.Should().ContainEquivalentOf(dbIssue,
                // options => options.Excluding(issue => issue.Id).Excluding(issue => issue.CreatedAt)
                // .Excluding(issue => issue.Vehicle));
                await Assert.That(dbIssues.First().Vehicle).IsNotNull();
                await Assert.That(dbIssues.First().Vehicle!.Appearances).IsNotEmpty();
            }
        });

        // Act
        await job.ExecuteAsync(CancellationToken.None);

        // Assert
        await CheckDbContentAsync(async context =>
        {
            var resultAppearances = context.Appearances.ToList();
            var issues = context.Issues.Include(issue => issue.Vehicle).ThenInclude(vehicle => vehicle!.Appearances).ToList();

            using (Assert.Multiple())
            {
                await Assert.That(resultAppearances).IsNotEmpty();
                await Assert.That(resultAppearances.Count).IsEqualTo(expectedAppearances.Count);
                await Assert.That(resultAppearances).DoesNotContain(deletedAppearance);
                // resultAppearances.Should().NotContainEquivalentOf(deletedAppearance,
                // options => options.Excluding(dbAppearance => dbAppearance.Id).Excluding(dbAppearance => dbAppearance.CreatedAt));
                await Assert.That(resultAppearances).IsEquivalentTo(expectedAppearances);
                // resultAppearances.Should().BeEquivalentTo(expectedAppearances,
                // options => options.Excluding(dbAppearance => dbAppearance.Id).Excluding(dbAppearance => dbAppearance.CreatedAt));
                await Assert.That(issues).Contains(dbIssue);
                // issues.Should().ContainEquivalentOf(dbIssue, options => options.Excluding(issue => issue.Id)
                // .Excluding(issue => issue.CreatedAt).Excluding(issue => issue.Vehicle));
                await Assert.That(issues[0].Vehicle).IsNotNull();
                await Assert.That(issues[0].Vehicle!.Appearances).IsEmpty();
            }
        });
    }

    [Test]
    public async Task ExecuteAsyncShouldUpdateMilestones()
    {
        // Arrange
        var expectedMilestones = GitlabMockData.AddedMilestones();
        var dbIssue = new Issue { Title = "NotDeleted", };
        var deletedMilestone = new Milestone
        {
            Title = "Deleted",
            GitlabId = "gid://gitlab/Milestone/4",
            GitlabIid = "4",
            Issues = [dbIssue]
        };
        dbIssue.Milestone = deletedMilestone;
        var githubMilestone = new Milestone { Title = "GitHub", GitHubId = "gid://github/Milestone/5", };
        expectedMilestones.Add(githubMilestone);
        await SeedDatabaseAsync(context =>
        {
            context.Milestones.Add(expectedMilestones[0]);
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

        await CheckDbContentAsync(async context =>
        {
            var milestones = context.Milestones.ToList();
            var issues = context.Issues.Include(entity => entity.Milestone);
            await Assert.That(milestones).Contains(deletedMilestone);
            // milestones.Should().ContainEquivalentOf(deletedMilestone,
            // options => options.Excluding(dbMilestone => dbMilestone.Id).Excluding(dbMilestone => dbMilestone.CreatedAt)
            // .Excluding(dbMilestone => dbMilestone.Issues));
            await Assert.That(milestones).Contains(githubMilestone);
            // milestones.Should().ContainEquivalentOf(githubMilestone,
            // options => options.Excluding(dbMilestone => dbMilestone.Id).Excluding(dbMilestone => dbMilestone.CreatedAt));
            await Assert.That(milestones).Contains(expectedMilestones[0]);
            // milestones.Should().ContainEquivalentOf(expectedMilestones[0],
            // options => options.Excluding(dbMilestone => dbMilestone.Id).Excluding(dbMilestone => dbMilestone.CreatedAt));
            await Assert.That(issues).Contains(dbIssue);
            // context.Issues.Should().ContainEquivalentOf(dbIssue,
            // options => options.Excluding(issue => issue.Id).Excluding(issue => issue.CreatedAt)
            // .Excluding(issue => issue.Milestone));
            await Assert.That(issues.First().Milestone).IsNull();
        });

        // Act
        await job.ExecuteAsync(CancellationToken.None);

        // Assert
        await CheckDbContentAsync(async context =>
        {
            var resultMilestones = context.Milestones.ToList();
            var issues = context.Issues.Include(issue => issue.Milestone).ToList();

            using (Assert.Multiple())
            {
                await Assert.That(resultMilestones).IsNotEmpty();
                await Assert.That(resultMilestones.Count).IsEqualTo(expectedMilestones.Count);
                await Assert.That(resultMilestones).DoesNotContain(deletedMilestone);
                // resultMilestones.Should().NotContainEquivalentOf(deletedMilestone,
                // options => options.Excluding(dbMilestone => dbMilestone.Id).Excluding(dbMilestone => dbMilestone.CreatedAt)
                // .Excluding(dbMilestone => dbMilestone.Issues));
                await Assert.That(resultMilestones).IsEquivalentTo(expectedMilestones);
                // resultMilestones.Should().BeEquivalentTo(expectedMilestones,
                // options => options.Excluding(dbMilestone => dbMilestone.Id).Excluding(dbMilestone => dbMilestone.CreatedAt));
                await Assert.That(issues).Contains(dbIssue);
                // issues.Should().ContainEquivalentOf(dbIssue, options => options.Excluding(issue => issue.Id)
                // .Excluding(issue => issue.LastModifiedAt).Excluding(issue => issue.CreatedAt)
                // .Excluding(issue => issue.Milestone));
                await Assert.That(issues[0].Milestone).IsNotNull();
            }
        });
    }

    [Test]
    public async Task ExecuteAsyncShouldUpdateReleases()
    {
        // Arrange
        var expectedReleases = GitlabMockData.AddedReleases();
        var dbIssue = new Issue { Title = "NotDeleted", };
        var deletedRelease = new Release
        {
            Title = "Deleted",
            GitlabId = "gid://gitlab/Issue/4",
            GitlabIid = "4",
            Issues = [dbIssue]
        };
        dbIssue.Release = deletedRelease;
        var githubRelease = new Release { Title = "GitHub", GitHubId = "gid://github/Issue/5", };
        expectedReleases.Add(githubRelease);
        await SeedDatabaseAsync(context =>
        {
            context.Releases.Add(expectedReleases[0]);
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

        await CheckDbContentAsync(async context =>
        {
            var releases = context.Releases.ToList();
            var issues = context.Issues.Include(entity => entity.Release);
            using (Assert.Multiple())
            {
                await Assert.That(releases).Contains(deletedRelease);
                // releases.Should().ContainEquivalentOf(deletedRelease,
                // options => options.Excluding(dbRelease => dbRelease.Id).Excluding(dbRelease => dbRelease.CreatedAt)
                // .Excluding(dbRelease => dbRelease.Issues));
                await Assert.That(releases).Contains(githubRelease);
                // releases.Should().ContainEquivalentOf(githubRelease,
                // options => options.Excluding(dbRelease => dbRelease.Id).Excluding(dbRelease => dbRelease.CreatedAt));
                await Assert.That(releases).Contains(expectedReleases[0]);
                // releases.Should().ContainEquivalentOf(expectedReleases[0],
                // options => options.Excluding(dbRelease => dbRelease.Id).Excluding(dbRelease => dbRelease.CreatedAt));
                await Assert.That(issues).Contains(dbIssue);
                // context.Issues.Should().ContainEquivalentOf(dbIssue,
                // options => options.Excluding(issue => issue.Id).Excluding(issue => issue.CreatedAt)
                // .Excluding(issue => issue.Release));
                await Assert.That(issues.First().Release).IsNotNull();
            }
        });

        // Act
        await job.ExecuteAsync(CancellationToken.None);

        // Assert
        await CheckDbContentAsync(async context =>
        {
            var resultReleases = context.Releases.ToList();
            var issues = context.Issues.Include(issue => issue.Release).ToList();
            using (Assert.Multiple())
            {
                await Assert.That(resultReleases).IsNotEmpty();
                await Assert.That(resultReleases.Count).IsEqualTo(expectedReleases.Count);
                await Assert.That(resultReleases).DoesNotContain(deletedRelease);
                // resultReleases.Should().NotContainEquivalentOf(deletedRelease,
                // options => options.Excluding(dbRelease => dbRelease.Id).Excluding(dbRelease => dbRelease.CreatedAt)
                // .Excluding(dbRelease => dbRelease.Issues));
                await Assert.That(resultReleases).IsEquivalentTo(expectedReleases);
                // resultReleases.Should().BeEquivalentTo(expectedReleases,
                // options => options.Excluding(dbRelease => dbRelease.Id).Excluding(dbRelease => dbRelease.CreatedAt));
                await Assert.That(issues).Contains(dbIssue);
                // issues.Should().ContainEquivalentOf(dbIssue, options => options.Excluding(issue => issue.Id)
                // .Excluding(issue => issue.LastModifiedAt).Excluding(issue => issue.CreatedAt)
                // .Excluding(issue => issue.Release));
                await Assert.That(issues[0].Release).IsNull();
            }
        });
    }
}
