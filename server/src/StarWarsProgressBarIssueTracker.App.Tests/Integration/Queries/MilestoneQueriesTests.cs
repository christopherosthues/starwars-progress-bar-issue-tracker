using GraphQL;
using StarWarsProgressBarIssueTracker.App.Milestones;
using StarWarsProgressBarIssueTracker.App.Tests.Helpers;
using StarWarsProgressBarIssueTracker.App.Tests.Helpers.GraphQL.Payloads;
using StarWarsProgressBarIssueTracker.App.Tests.Helpers.GraphQL.Payloads.Milestones;
using StarWarsProgressBarIssueTracker.Domain.Issues;
using StarWarsProgressBarIssueTracker.Domain.Milestones;
using StarWarsProgressBarIssueTracker.Domain.Releases;
using StarWarsProgressBarIssueTracker.Domain.Vehicles;
using StarWarsProgressBarIssueTracker.TestHelpers;

namespace StarWarsProgressBarIssueTracker.App.Tests.Integration.Queries;

[Category(TestCategory.Integration)]
[NotInParallel(NotInParallelTests.MilestoneRetrieval)]
public class MilestoneQueriesTests : IntegrationTestBase
{
    // TODO: Check DoesNotContain and Contains
    [Test]
    public async Task GetMilestonesShouldReturnEmptyListIfNoReleaseExist()
    {
        // Arrange
        await CheckDbContentAsync(async context =>
        {
            await Assert.That(context.Milestones).IsEmpty();
        });
        GraphQLRequest request = CreateGetMilestonesRequest();

        // Act
        GraphQLResponse<GetMilestonesResponse> response = await CreateGraphQLClient().SendQueryAsync<GetMilestonesResponse>(request);

        // Assert
        using (Assert.Multiple())
        {
            await Assert.That(response).IsNotNull();
            await Assert.That(response.Errors).IsNull().Or.IsEmpty();
            await Assert.That(response.Data).IsNotNull();
            await Assert.That(response.Data.Milestones.TotalCount).IsEqualTo(0);
            await Assert.That(response.Data.Milestones.PageInfo.HasNextPage).IsFalse();
            await Assert.That(response.Data.Milestones.PageInfo.HasPreviousPage).IsFalse();
            await Assert.That(response.Data.Milestones.PageInfo.StartCursor).IsNull();
            await Assert.That(response.Data.Milestones.PageInfo.EndCursor).IsNull();
            await Assert.That(response.Data.Milestones.Edges).IsEmpty();
            await Assert.That(response.Data.Milestones.Nodes).IsEmpty();
        }
    }

    [Test]
    public async Task GetMilestonesShouldReturnAllMilestones()
    {
        // Arrange
        Milestone dbMilestone = new Milestone
        {
            Title = "Milestone 1",
            State = MilestoneState.Open
        };

        Issue dbIssue = new Issue
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
        Milestone dbMilestone2 = new Milestone
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
        await CheckDbContentAsync(async context =>
        {
            List<Milestone> dbMilestones = context.Milestones.ToList();
            List<Issue> dbIssues = context.Issues.ToList();
            using (Assert.Multiple())
            {
                await Assert.That(dbMilestones).Contains(milestone => milestone.Id.Equals(dbMilestone.Id));
                await Assert.That(dbMilestones).Contains(milestone => milestone.Id.Equals(dbMilestone2.Id));
                await Assert.That(dbIssues).Contains(issue => issue.Id.Equals(dbIssue.Id));
            }
        });
        GraphQLRequest request = CreateGetMilestonesRequest();

        // Act
        GraphQLResponse<GetMilestonesResponse> response = await CreateGraphQLClient().SendQueryAsync<GetMilestonesResponse>(request);

        // Assert
        using (Assert.Multiple())
        {
            await Assert.That(response).IsNotNull();
            await Assert.That(response.Errors).IsNull().Or.IsEmpty();
            await Assert.That(response.Data).IsNotNull();
            await Assert.That(response.Data.Milestones.TotalCount).IsEqualTo(2);
            await Assert.That(response.Data.Milestones.PageInfo.HasNextPage).IsFalse();
            await Assert.That(response.Data.Milestones.PageInfo.HasPreviousPage).IsFalse();
            await Assert.That(response.Data.Milestones.PageInfo.StartCursor).IsNotNull();
            await Assert.That(response.Data.Milestones.PageInfo.EndCursor).IsNotNull();

            List<MilestoneDto> milestones = response.Data.Milestones.Nodes.ToList();
            await Assert.That(milestones.Count).IsEqualTo(2);

            MilestoneDto milestone = milestones.Single(entity => entity.Id.Equals(dbMilestone.Id));
            await Assert.That(milestone.Id).IsEqualTo(dbMilestone.Id);
            await Assert.That(milestone.Title).IsEqualTo(dbMilestone.Title);
            await Assert.That(milestone.Description).IsNull();
            await Assert.That(milestone.State).IsEqualTo(dbMilestone.State);
            await Assert.That(milestone.CreatedAt).IsEquivalentTo(dbMilestone.CreatedAt);
            await Assert.That(milestone.LastModifiedAt).IsEquivalentTo(dbMilestone.LastModifiedAt);
            await Assert.That(milestone.Issues).IsEmpty();

            MilestoneDto milestone2 = milestones.Single(entity => entity.Id.Equals(dbMilestone2.Id));
            await Assert.That(milestone2.Id).IsEqualTo(dbMilestone2.Id);
            await Assert.That(milestone2.Title).IsEqualTo(dbMilestone2.Title);
            await Assert.That(milestone2.Description).IsEqualTo(dbMilestone2.Description);
            await Assert.That(milestone2.State).IsEqualTo(dbMilestone2.State);
            await Assert.That(milestone2.CreatedAt).IsEquivalentTo(dbMilestone2.CreatedAt);
            await Assert.That(milestone2.LastModifiedAt).IsEquivalentTo(dbMilestone2.LastModifiedAt);
            await Assert.That(milestone2.Issues).IsNotEmpty();
            await Assert.That(milestone2.Issues.Count).IsEqualTo(1);
            MilestoneIssueDto issue = milestone2.Issues.First();
            await Assert.That(issue.Id).IsEqualTo(dbIssue.Id);
            await Assert.That(issue.Release).IsNotNull();
            await Assert.That(issue.Release!.Id).IsEqualTo(dbIssue.Release.Id);
            await Assert.That(issue.Vehicle).IsNotNull();
            await Assert.That(issue.Vehicle!.Id).IsEqualTo(dbIssue.Vehicle.Id);
            await Assert.That(issue.Vehicle!.Translations).IsEmpty();
            await Assert.That(issue.Vehicle!.Photos).IsEmpty();

            List<Edge<MilestoneDto>> edges = response.Data.Milestones.Edges.ToList();
            await Assert.That(edges.Count).IsEqualTo(2);

            MilestoneDto edgeMilestone = edges.Single(entity => entity.Node.Id.Equals(dbMilestone.Id)).Node;
            await Assert.That(edgeMilestone.Id).IsEqualTo(dbMilestone.Id);
            await Assert.That(edgeMilestone.Title).IsEqualTo(dbMilestone.Title);
            await Assert.That(edgeMilestone.Description).IsNull();
            await Assert.That(edgeMilestone.State).IsEqualTo(dbMilestone.State);
            await Assert.That(edgeMilestone.CreatedAt).IsEquivalentTo(dbMilestone.CreatedAt);
            await Assert.That(edgeMilestone.LastModifiedAt).IsEquivalentTo(dbMilestone.LastModifiedAt);
            await Assert.That(edgeMilestone.Issues).IsEmpty();

            MilestoneDto edgeMilestone2 = edges.Single(entity => entity.Node.Id.Equals(dbMilestone2.Id)).Node;
            await Assert.That(edgeMilestone2.Id).IsEqualTo(dbMilestone2.Id);
            await Assert.That(edgeMilestone2.Title).IsEqualTo(dbMilestone2.Title);
            await Assert.That(edgeMilestone2.Description).IsEqualTo(dbMilestone2.Description);
            await Assert.That(edgeMilestone2.State).IsEqualTo(dbMilestone2.State);
            await Assert.That(edgeMilestone2.CreatedAt).IsEquivalentTo(dbMilestone2.CreatedAt);
            await Assert.That(edgeMilestone2.LastModifiedAt).IsEquivalentTo(dbMilestone2.LastModifiedAt);
            await Assert.That(edgeMilestone2.Issues).IsNotEmpty();
            await Assert.That(edgeMilestone2.Issues.Count).IsEqualTo(1);
            MilestoneIssueDto edgeIssue = edgeMilestone2.Issues.First();
            await Assert.That(edgeIssue.Id).IsEqualTo(dbIssue.Id);
            await Assert.That(edgeIssue.Release).IsNotNull();
            await Assert.That(edgeIssue.Release!.Id).IsEqualTo(dbIssue.Release.Id);
            await Assert.That(edgeIssue.Vehicle).IsNotNull();
            await Assert.That(edgeIssue.Vehicle!.Id).IsEqualTo(dbIssue.Vehicle.Id);
            await Assert.That(edgeIssue.Vehicle!.Translations).IsEmpty();
            await Assert.That(edgeIssue.Vehicle!.Photos).IsEmpty();
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
        GraphQLRequest request = CreateGetMilestoneRequest(id);

        // Act
        GraphQLResponse<GetMilestoneResponse> response = await CreateGraphQLClient().SendQueryAsync<GetMilestoneResponse>(request);

        // Assert
        using (Assert.Multiple())
        {
            await Assert.That(response).IsNotNull();
            await Assert.That(response.Errors).IsNull();
            // response.Errors!.First().Extensions!["message"].Should().Be($"No Label found with id '{id}'.", StringComparer.InvariantCultureIgnoreCase);
            await Assert.That(response.Data).IsNotNull();
            await Assert.That(response.Data.Milestone).IsNull();
        }
    }

    [Test]
    public async Task GetMilestoneShouldReturnNullIfMilestonesAreEmpty()
    {
        // Arrange
        await CheckDbContentAsync(async context =>
        {
            await Assert.That(context.Milestones).IsEmpty();
        });
        const string id = "F1378377-9846-4168-A595-E763CD61CD9F";
        GraphQLRequest request = CreateGetMilestoneRequest(id);

        // Act
        GraphQLResponse<GetMilestoneResponse> response = await CreateGraphQLClient().SendQueryAsync<GetMilestoneResponse>(request);

        // Assert
        using (Assert.Multiple())
        {
            await Assert.That(response).IsNotNull();
            await Assert.That(response.Errors).IsNull();
            // response.Errors!.First().Extensions!["message"].Should().Be($"No Label found with id '{id}'.", StringComparer.InvariantCultureIgnoreCase);
            await Assert.That(response.Data).IsNotNull();
            await Assert.That(response.Data.Milestone).IsNull();
        }
    }

    [Test]
    public async Task GetMilestoneShouldReturnReleaseWithGivenId()
    {
        // Arrange
        const string id = "F1378377-9846-4168-A595-E763CD61CD9F";
        Issue dbIssue = new Issue
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
        Milestone dbMilestone = new Milestone
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
        GraphQLRequest request = CreateGetMilestoneRequest(id);

        // Act
        GraphQLResponse<GetMilestoneResponse> response = await CreateGraphQLClient().SendQueryAsync<GetMilestoneResponse>(request);

        // Assert
        using (Assert.Multiple())
        {
            await Assert.That(response).IsNotNull();
            await Assert.That(response.Errors).IsNull();
            await Assert.That(response.Data).IsNotNull();
            MilestoneDto? milestone = response.Data.Milestone;

            await Assert.That(milestone).IsNotNull();
            await Assert.That(milestone!.Id).IsEqualTo(dbMilestone.Id);
            await Assert.That(milestone.Title).IsEqualTo(dbMilestone.Title);
            await Assert.That(milestone.Description).IsEqualTo(dbMilestone.Description);
            await Assert.That(milestone.State).IsEqualTo(dbMilestone.State);
            await Assert.That(milestone.CreatedAt).IsEquivalentTo(dbMilestone.CreatedAt);
            await Assert.That(milestone.LastModifiedAt).IsEquivalentTo(dbMilestone.LastModifiedAt);
            await Assert.That(milestone.Issues).IsNotEmpty();
            await Assert.That(milestone.Issues.Count).IsEqualTo(1);
            MilestoneIssueDto issue = milestone.Issues.First();
            await Assert.That(issue.Id).IsEqualTo(dbIssue.Id);
            await Assert.That(issue.Release).IsNotNull();
            await Assert.That(issue.Release!.Id).IsEqualTo(dbIssue.Release.Id);
            await Assert.That(issue.Vehicle).IsNotNull();
            await Assert.That(issue.Vehicle!.Id).IsEqualTo(dbIssue.Vehicle.Id);
            await Assert.That(issue.Vehicle!.Translations).IsEmpty();
            await Assert.That(issue.Vehicle!.Photos).IsEmpty();
        }
    }

    private static GraphQLRequest CreateGetMilestonesRequest()
    {
        GraphQLRequest queryRequest = new GraphQLRequest
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
        GraphQLRequest queryRequest = new GraphQLRequest
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
