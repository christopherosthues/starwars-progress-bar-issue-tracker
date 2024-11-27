using FluentAssertions;
using FluentAssertions.Execution;
using GraphQL;
using StarWarsProgressBarIssueTracker.App.Queries;
using StarWarsProgressBarIssueTracker.App.Tests.Helpers.GraphQL.Payloads;
using StarWarsProgressBarIssueTracker.App.Tests.Helpers.GraphQL.Payloads.Milestones;
using StarWarsProgressBarIssueTracker.Common.Tests;
using StarWarsProgressBarIssueTracker.Domain.Issues;
using StarWarsProgressBarIssueTracker.Domain.Milestones;
using StarWarsProgressBarIssueTracker.Domain.Releases;
using StarWarsProgressBarIssueTracker.Domain.Vehicles;

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
            response.Data.Milestones.TotalCount.Should().Be(0);
            response.Data.Milestones.PageInfo.HasNextPage.Should().BeFalse();
            response.Data.Milestones.PageInfo.HasPreviousPage.Should().BeFalse();
            response.Data.Milestones.PageInfo.StartCursor.Should().BeNull();
            response.Data.Milestones.PageInfo.EndCursor.Should().BeNull();
            response.Data.Milestones.Edges.Should().BeEmpty();
            response.Data.Milestones.Nodes.Should().BeEmpty();
        }
    }

    [Test]
    public async Task GetMilestonesShouldReturnAllMilestones()
    {
        // Arrange
        var dbMilestone = new Milestone
        {
            Title = "Milestone 1",
            State = MilestoneState.Open
        };

        var dbIssue = new Issue
        {
            Title = "issue title",
            State = IssueState.Closed,
            Release = new Release { Title = "milestone title", State = ReleaseState.Planned },
            Vehicle = new Vehicle
            {
                Appearances =
                [
                    new Appearance { Title = "Appearance title", Color = "112233", TextColor = "334455" }
                ],
                Translations = [new Translation { Country = "en", Text = "translation" }],
                Photos = [new Photo { FilePath = string.Empty }]
            }
        };
        var dbMilestone2 = new Milestone
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
            response.Data.Milestones.TotalCount.Should().Be(2);
            response.Data.Milestones.PageInfo.HasNextPage.Should().BeFalse();
            response.Data.Milestones.PageInfo.HasPreviousPage.Should().BeFalse();
            response.Data.Milestones.PageInfo.StartCursor.Should().NotBeNull();
            response.Data.Milestones.PageInfo.EndCursor.Should().NotBeNull();

            List<Milestone> milestones = response.Data.Milestones.Nodes.ToList();
            milestones.Count.Should().Be(2);

            Milestone milestone = milestones.Single(entity => entity.Id.Equals(dbMilestone.Id));
            milestone.Id.Should().Be(dbMilestone.Id);
            milestone.Title.Should().Be(dbMilestone.Title);
            milestone.Description.Should().BeNull();
            milestone.State.Should().Be(dbMilestone.State);
            milestone.CreatedAt.Should().BeCloseTo(dbMilestone.CreatedAt, TimeSpan.FromMilliseconds(300));
            milestone.LastModifiedAt.Should().Be(dbMilestone.LastModifiedAt);
            milestone.Issues.Should().BeEmpty();

            Milestone milestone2 = milestones.Single(entity => entity.Id.Equals(dbMilestone2.Id));
            milestone2.Id.Should().Be(dbMilestone2.Id);
            milestone2.Title.Should().Be(dbMilestone2.Title);
            milestone2.Description.Should().Be(dbMilestone2.Description);
            milestone2.State.Should().Be(dbMilestone2.State);
            milestone2.CreatedAt.Should().BeCloseTo(dbMilestone2.CreatedAt, TimeSpan.FromMilliseconds(300));
            milestone2.LastModifiedAt.Should().BeCloseTo(dbMilestone2.LastModifiedAt!.Value, TimeSpan.FromMilliseconds(300));
            milestone2.Issues.Should().NotBeEmpty();
            milestone2.Issues.Count.Should().Be(1);
            Issue issue = milestone2.Issues.First();
            issue.Id.Should().Be(dbIssue.Id);
            issue.Release.Should().NotBeNull();
            issue.Release!.Id.Should().Be(dbIssue.Release.Id);
            issue.Vehicle.Should().NotBeNull();
            issue.Vehicle!.Id.Should().Be(dbIssue.Vehicle.Id);
            issue.Vehicle!.Translations.Should().BeEmpty();
            issue.Vehicle!.Photos.Should().BeEmpty();

            List<Edge<Milestone>> edges = response.Data.Milestones.Edges.ToList();
            edges.Count.Should().Be(2);

            Milestone edgeMilestone = edges.Single(entity => entity.Node.Id.Equals(dbMilestone.Id)).Node;
            edgeMilestone.Id.Should().Be(dbMilestone.Id);
            edgeMilestone.Title.Should().Be(dbMilestone.Title);
            edgeMilestone.Description.Should().BeNull();
            edgeMilestone.State.Should().Be(dbMilestone.State);
            edgeMilestone.CreatedAt.Should().BeCloseTo(dbMilestone.CreatedAt, TimeSpan.FromMilliseconds(300));
            edgeMilestone.LastModifiedAt.Should().Be(dbMilestone.LastModifiedAt);
            edgeMilestone.Issues.Should().BeEmpty();

            Milestone edgeMilestone2 = edges.Single(entity => entity.Node.Id.Equals(dbMilestone2.Id)).Node;
            edgeMilestone2.Id.Should().Be(dbMilestone2.Id);
            edgeMilestone2.Title.Should().Be(dbMilestone2.Title);
            edgeMilestone2.Description.Should().Be(dbMilestone2.Description);
            edgeMilestone2.State.Should().Be(dbMilestone2.State);
            edgeMilestone2.CreatedAt.Should().BeCloseTo(dbMilestone2.CreatedAt, TimeSpan.FromMilliseconds(300));
            edgeMilestone2.LastModifiedAt.Should().BeCloseTo(dbMilestone2.LastModifiedAt!.Value, TimeSpan.FromMilliseconds(300));
            edgeMilestone2.Issues.Should().NotBeEmpty();
            edgeMilestone2.Issues.Count.Should().Be(1);
            Issue edgeIssue = edgeMilestone2.Issues.First();
            edgeIssue.Id.Should().Be(dbIssue.Id);
            edgeIssue.Release.Should().NotBeNull();
            edgeIssue.Release!.Id.Should().Be(dbIssue.Release.Id);
            edgeIssue.Vehicle.Should().NotBeNull();
            edgeIssue.Vehicle!.Id.Should().Be(dbIssue.Vehicle.Id);
            edgeIssue.Vehicle!.Translations.Should().BeEmpty();
            edgeIssue.Vehicle!.Photos.Should().BeEmpty();
        }
    }

    [Test]
    public async Task GetMilestoneShouldReturnNullIfReleaseWithGivenIdDoesNotExist()
    {
        // Arrange
        await SeedDatabaseAsync(context =>
        {
            context.Milestones.Add(new Milestone
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
            // response.Errors!.First().Extensions!["message"].Should().Be($"No Label found with id '{id}'.",
            //     StringComparer.InvariantCultureIgnoreCase);
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
            // response.Errors!.First().Extensions!["message"].Should().Be($"No Label found with id '{id}'.",
            //     StringComparer.InvariantCultureIgnoreCase);
            response.Data.Should().NotBeNull();
            response.Data.Milestone.Should().BeNull();
        }
    }

    [Test]
    public async Task GetMilestoneShouldReturnReleaseWithGivenId()
    {
        // Arrange
        const string id = "F1378377-9846-4168-A595-E763CD61CD9F";
        var dbIssue = new Issue
        {
            Id = new Guid("CB547CF5-CB28-412E-8DA4-2A7F10E3A5FE"),
            Title = "issue title",
            State = IssueState.Closed,
            Release = new Release { Title = "milestone title", State = ReleaseState.Planned },
            Vehicle = new Vehicle
            {
                Appearances =
                [
                    new Appearance { Title = "Appearance title", Color = "112233", TextColor = "334455" }
                ],
                Translations = [new Translation { Country = "en", Text = "translation" }],
                Photos = [new Photo { FilePath = string.Empty }]
            }
        };
        var dbMilestone = new Milestone
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
            context.Milestones.Add(new Milestone
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
                        milestones(first: 5)
                        {
                            totalCount
                            pageInfo {
                                hasNextPage
                                hasPreviousPage
                                startCursor
                                endCursor
                            }
                            edges {
                                node {
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
                            nodes {
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
