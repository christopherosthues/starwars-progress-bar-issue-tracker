using FluentAssertions;
using FluentAssertions.Execution;
using GraphQL;
using StarWarsProgressBarIssueTracker.App.Queries;
using StarWarsProgressBarIssueTracker.App.Tests.Helpers.GraphQL.Payloads.Milestones;
using StarWarsProgressBarIssueTracker.Common.Tests;
using StarWarsProgressBarIssueTracker.Domain.Issues;
using StarWarsProgressBarIssueTracker.Domain.Milestones;
using StarWarsProgressBarIssueTracker.Domain.Releases;
using StarWarsProgressBarIssueTracker.Infrastructure.Models;

namespace StarWarsProgressBarIssueTracker.App.Tests.Integration.Queries;

[TestFixture(TestOf = typeof(IssueTrackerQueries))]
[Category(TestCategory.Integration)]
public class MilestoneQueriesTests : IntegrationTestBase
{
    [Test]
    public async Task GetMilestonesShouldReturnEmptyListIfNoReleaseExist()
    {
        // Arrange
        CheckDbContent(context =>
        {
            context.Milestones.Should().BeEmpty();
        });
        var request = CreateGetMilestonesRequest();

        // Act
        var response = await GraphQLClient.SendQueryAsync<GetMilestonesResponse>(request);

        // Assert
        using (new AssertionScope())
        {
            response.Should().NotBeNull();
            response.Errors.Should().BeNullOrEmpty();
            response.Data.Should().NotBeNull();
            response.Data.Milestones.Should().BeEmpty();
        }
    }

    [Test]
    public async Task GetMilestonesShouldReturnAllMilestones()
    {
        // Arrange
        var dbMilestone = new DbMilestone
        {
            Title = "Milestone 1",
            State = MilestoneState.Open
        };

        var dbIssue = new DbIssue
        {
            Title = "issue title",
            State = IssueState.Closed,
            Release = new DbRelease { Title = "milestone title", State = ReleaseState.Planned },
            Vehicle = new DbVehicle
            {
                Appearances =
                [
                    new DbAppearance { Title = "Appearance title", Color = "112233", TextColor = "334455" }
                ],
                Translations = [new DbTranslation { Country = "en", Text = "translation" }],
                Photos = [new DbPhoto { FilePath = string.Empty }]
            }
        };
        var dbMilestone2 = new DbMilestone
        {
            Title = "Milestone 2",
            Description = "Notes 2",
            State = MilestoneState.Closed,
            LastModifiedAt = DateTime.UtcNow.AddDays(-1),
            Issues =
            [
                dbIssue
            ]
        };
        dbIssue.Milestone = dbMilestone2;
        await SeedDatabaseAsync(context =>
        {
            context.Milestones.Add(dbMilestone);
            context.Milestones.Add(dbMilestone2);
        });
        CheckDbContent(context =>
        {
            var dbMilestones = context.Milestones.ToList();
            dbMilestones.Any(milestone => milestone.Id.Equals(dbMilestone.Id)).Should().BeTrue();
            dbMilestones.Any(milestone => milestone.Id.Equals(dbMilestone2.Id)).Should().BeTrue();
            var dbIssues = context.Issues.ToList();
            dbIssues.Any(issue => issue.Id.Equals(dbIssue.Id)).Should().BeTrue();
        });
        var request = CreateGetMilestonesRequest();

        // Act
        var response = await GraphQLClient.SendQueryAsync<GetMilestonesResponse>(request);

        // Assert
        using (new AssertionScope())
        {
            response.Should().NotBeNull();
            response.Errors.Should().BeNullOrEmpty();
            response.Data.Should().NotBeNull();
            var releases = response.Data.Milestones.ToList();
            releases.Count.Should().Be(2);

            var release = releases.Single(entity => entity.Id.Equals(dbMilestone.Id));
            release.Id.Should().Be(dbMilestone.Id);
            release.Title.Should().Be(dbMilestone.Title);
            release.Description.Should().BeNull();
            release.State.Should().Be(dbMilestone.State);
            release.CreatedAt.Should().BeCloseTo(dbMilestone.CreatedAt, TimeSpan.FromMilliseconds(300));
            release.LastModifiedAt.Should().Be(dbMilestone.LastModifiedAt);
            release.Issues.Should().BeEmpty();

            var release2 = releases.Single(entity => entity.Id.Equals(dbMilestone2.Id));
            release2.Id.Should().Be(dbMilestone2.Id);
            release2.Title.Should().Be(dbMilestone2.Title);
            release2.Description.Should().Be(dbMilestone2.Description);
            release2.State.Should().Be(dbMilestone2.State);
            release2.CreatedAt.Should().BeCloseTo(dbMilestone2.CreatedAt, TimeSpan.FromMilliseconds(300));
            release2.LastModifiedAt.Should().BeCloseTo(dbMilestone2.LastModifiedAt!.Value, TimeSpan.FromMilliseconds(300));
            release2.Issues.Should().NotBeEmpty();
            release2.Issues.Count.Should().Be(1);
            var issue = release2.Issues.First();
            issue.Id.Should().Be(dbIssue.Id);
            issue.Release.Should().NotBeNull();
            issue.Release!.Id.Should().Be(dbIssue.Release.Id);
            issue.Vehicle.Should().NotBeNull();
            issue.Vehicle!.Id.Should().Be(dbIssue.Vehicle.Id);
            issue.Vehicle!.Translations.Should().BeEmpty();
            issue.Vehicle!.Photos.Should().BeEmpty();
        }
    }

    [Test]
    public async Task GetMilestoneShouldReturnNullIfReleaseWithGivenIdDoesNotExist()
    {
        // Arrange
        await SeedDatabaseAsync(context =>
        {
            context.Milestones.Add(new DbMilestone
            {
                Id = new Guid("5888CDB6-57E2-4774-B6E8-7AABE82E2A5F"),
                Title = "Milestone 1",
                Description = "Notes 1",
                State = MilestoneState.Closed,
                LastModifiedAt = DateTime.UtcNow.AddDays(-1)
            });
        });
        const string id = "F1378377-9846-4168-A595-E763CD61CD9F";
        var request = CreateGetMilestoneRequest(id);

        // Act
        var response = await GraphQLClient.SendQueryAsync<GetMilestoneResponse>(request);

        // Assert
        using (new AssertionScope())
        {
            response.Should().NotBeNull();
            response.Errors.Should().BeNull();
            response.Data.Should().NotBeNull();
            response.Data.Milestone.Should().BeNull();
        }
    }

    [Test]
    public async Task GetMilestoneShouldReturnNullIfMilestonesAreEmpty()
    {
        // Arrange
        CheckDbContent(context =>
        {
            context.Milestones.Should().BeEmpty();
        });
        const string id = "F1378377-9846-4168-A595-E763CD61CD9F";
        var request = CreateGetMilestoneRequest(id);

        // Act
        var response = await GraphQLClient.SendQueryAsync<GetMilestoneResponse>(request);

        // Assert
        using (new AssertionScope())
        {
            response.Should().NotBeNull();
            response.Errors.Should().BeNull();
            response.Data.Should().NotBeNull();
            response.Data.Milestone.Should().BeNull();
        }
    }

    [Test]
    public async Task GetMilestoneShouldReturnReleaseWithGivenId()
    {
        // Arrange
        const string id = "F1378377-9846-4168-A595-E763CD61CD9F";
        var dbIssue = new DbIssue
        {
            Id = new Guid("CB547CF5-CB28-412E-8DA4-2A7F10E3A5FE"),
            Title = "issue title",
            State = IssueState.Closed,
            Release = new DbRelease { Title = "milestone title", State = ReleaseState.Planned },
            Vehicle = new DbVehicle
            {
                Appearances =
                [
                    new DbAppearance { Title = "Appearance title", Color = "112233", TextColor = "334455" }
                ],
                Translations = [new DbTranslation { Country = "en", Text = "translation" }],
                Photos = [new DbPhoto { FilePath = string.Empty }]
            }
        };
        var dbMilestone = new DbMilestone
        {
            Id = new Guid(id),
            Title = "Milestone 2",
            Description = "Notes 2",
            State = MilestoneState.Closed,
            LastModifiedAt = DateTime.UtcNow.AddDays(-1),
            Issues = [dbIssue]
        };
        await SeedDatabaseAsync(context =>
        {
            context.Milestones.Add(new DbMilestone
            {
                Id = new Guid("5888CDB6-57E2-4774-B6E8-7AABE82E2A5F"),
                Title = "Milestone 1",
                State = MilestoneState.Open
            });
            context.Milestones.Add(dbMilestone);
        });
        var request = CreateGetMilestoneRequest(id);

        // Act
        var response = await GraphQLClient.SendQueryAsync<GetMilestoneResponse>(request);

        // Assert
        using (new AssertionScope())
        {
            response.Should().NotBeNull();
            response.Errors.Should().BeNull();
            response.Data.Should().NotBeNull();
            var milestone = response.Data.Milestone;

            milestone.Should().NotBeNull();
            milestone!.Id.Should().Be(dbMilestone.Id);
            milestone.Title.Should().Be(dbMilestone.Title);
            milestone.Description.Should().Be(dbMilestone.Description);
            milestone.State.Should().Be(dbMilestone.State);
            milestone.CreatedAt.Should().BeCloseTo(dbMilestone.CreatedAt, TimeSpan.FromMilliseconds(300));
            milestone.LastModifiedAt.Should().BeCloseTo(dbMilestone.LastModifiedAt!.Value, TimeSpan.FromMilliseconds(300));
            milestone.Issues.Should().NotBeEmpty();
            milestone.Issues.Should().HaveCount(1);
            var issue = milestone.Issues.First();
            issue.Id.Should().Be(dbIssue.Id);
            issue.Release.Should().NotBeNull();
            issue.Release!.Id.Should().Be(dbIssue.Release.Id);
            issue.Vehicle.Should().NotBeNull();
            issue.Vehicle!.Id.Should().Be(dbIssue.Vehicle.Id);
            issue.Vehicle!.Translations.Should().BeEmpty();
            issue.Vehicle!.Photos.Should().BeEmpty();
        }
    }

    private static GraphQLRequest CreateGetMilestonesRequest()
    {
        var queryRequest = new GraphQLRequest
        {
            Query = """
                    query milestones
                    {
                        milestones()
                        {
                            id
                            title
                            description
                            state
                            createdAt
                            lastModifiedAt
                            issues
                            {
                                id
                                title
                                release
                                {
                                    id
                                    title
                                }
                                vehicle
                                {
                                    id
                                    appearances
                                    {
                                        id
                                        title
                                        color
                                        textColor
                                    }
                                }
                            }
                        }
                    }
                    """,
            OperationName = "milestones"
        };
        return queryRequest;
    }

    private static GraphQLRequest CreateGetMilestoneRequest(string id)
    {
        var queryRequest = new GraphQLRequest
        {
            Query = $$"""
                    query milestone
                    {
                        milestone(id: "{{id}}")
                        {
                            id
                            title
                            description
                            state
                            createdAt
                            lastModifiedAt
                            issues
                            {
                                id
                                title
                                release
                                {
                                    id
                                    title
                                }
                                vehicle
                                {
                                    id
                                    appearances
                                    {
                                        id
                                        title
                                        color
                                        textColor
                                    }
                                }
                            }
                        }
                    }
                    """,
            OperationName = "milestone"
        };
        return queryRequest;
    }
}
