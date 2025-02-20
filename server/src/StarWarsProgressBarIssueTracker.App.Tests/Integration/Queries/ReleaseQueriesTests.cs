using GraphQL;
using StarWarsProgressBarIssueTracker.App.Releases;
using StarWarsProgressBarIssueTracker.App.Tests.Helpers;
using StarWarsProgressBarIssueTracker.App.Tests.Helpers.GraphQL.Payloads;
using StarWarsProgressBarIssueTracker.App.Tests.Helpers.GraphQL.Payloads.Releases;
using StarWarsProgressBarIssueTracker.Domain.Issues;
using StarWarsProgressBarIssueTracker.Domain.Milestones;
using StarWarsProgressBarIssueTracker.Domain.Releases;
using StarWarsProgressBarIssueTracker.Domain.Vehicles;
using StarWarsProgressBarIssueTracker.TestHelpers;

namespace StarWarsProgressBarIssueTracker.App.Tests.Integration.Queries;

[Category(TestCategory.Integration)]
[NotInParallel(NotInParallelTests.ReleaseRetrieval)]
public class ReleaseQueriesTests : IntegrationTestBase
{
    // TODO: Check DoesNotContain and Contains
    [Test]
    public async Task GetReleasesShouldReturnEmptyListIfNoReleaseExist()
    {
        // Arrange
        await CheckDbContentAsync(async context =>
        {
            await Assert.That(context.Releases).IsEmpty();
        });
        GraphQLRequest request = CreateGetReleasesRequest();

        // Act
        GraphQLResponse<GetReleasesResponse> response = await CreateGraphQLClient().SendQueryAsync<GetReleasesResponse>(request);

        // Assert
        using (Assert.Multiple())
        {
            await Assert.That(response).IsNotNull();
            await Assert.That(response.Errors).IsNull().Or.IsEmpty();
            await Assert.That(response.Data).IsNotNull();
            await Assert.That(response.Data.Releases.TotalCount).IsEqualTo(0);
            await Assert.That(response.Data.Releases.PageInfo.HasNextPage).IsFalse();
            await Assert.That(response.Data.Releases.PageInfo.HasPreviousPage).IsFalse();
            await Assert.That(response.Data.Releases.PageInfo.StartCursor).IsNull();
            await Assert.That(response.Data.Releases.PageInfo.EndCursor).IsNull();
            await Assert.That(response.Data.Releases.Edges).IsEmpty();
            await Assert.That(response.Data.Releases.Nodes).IsEmpty();
        }
    }

    [Test]
    public async Task GetReleasesShouldReturnAllReleases()
    {
        // Arrange
        Release dbRelease = new Release
        {
            Title = "Release 1",
            State = ReleaseState.Planned
        };

        Issue dbIssue = new Issue
        {
            Title = "issue title",
            State = IssueState.Closed,
            Milestone = new Milestone { Title = "milestone title", State = MilestoneState.Open },
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
        Release dbRelease2 = new Release
        {
            Title = "Release 2",
            Notes = "Notes 2",
            State = ReleaseState.Released,
            Date = DateTime.UtcNow,
            LastModifiedAt = DateTime.UtcNow.AddDays(-1),
            Issues =
            [
                dbIssue
            ]
        };
        dbIssue.Release = dbRelease2;
        await SeedDatabaseAsync(context =>
        {
            context.Releases.Add(dbRelease);
            context.Releases.Add(dbRelease2);
        });
        await CheckDbContentAsync(async context =>
        {
            List<Release> dbReleases = context.Releases.ToList();
            List<Issue> dbIssues = context.Issues.ToList();
            using (Assert.Multiple())
            {
                await Assert.That(dbReleases).Contains(release => release.Id.Equals(dbRelease.Id));
                await Assert.That(dbReleases).Contains(release => release.Id.Equals(dbRelease2.Id));
                await Assert.That(dbIssues).Contains(issue => issue.Id.Equals(dbIssue.Id));
            }
        });
        GraphQLRequest request = CreateGetReleasesRequest();

        // Act
        GraphQLResponse<GetReleasesResponse> response = await CreateGraphQLClient().SendQueryAsync<GetReleasesResponse>(request);

        // Assert
        using (Assert.Multiple())
        {
            await Assert.That(response).IsNotNull();
            await Assert.That(response.Errors).IsNull().Or.IsEmpty();
            await Assert.That(response.Data).IsNotNull();

            await Assert.That(response.Data.Releases.TotalCount).IsEqualTo(2);
            await Assert.That(response.Data.Releases.PageInfo.HasNextPage).IsFalse();
            await Assert.That(response.Data.Releases.PageInfo.HasPreviousPage).IsFalse();
            await Assert.That(response.Data.Releases.PageInfo.StartCursor).IsNotNull();
            await Assert.That(response.Data.Releases.PageInfo.EndCursor).IsNotNull();

            List<ReleaseDto> releases = response.Data.Releases.Nodes.ToList();
            await Assert.That(releases.Count).IsEqualTo(2);

            ReleaseDto release = releases.Single(entity => entity.Id.Equals(dbRelease.Id));
            await Assert.That(release.Id).IsEqualTo(dbRelease.Id);
            await Assert.That(release.Title).IsEqualTo(dbRelease.Title);
            await Assert.That(release.Notes).IsNull();
            await Assert.That(release.State).IsEqualTo(dbRelease.State);
            await Assert.That(release.Date).IsNull();
            await Assert.That(release.CreatedAt).IsEquivalentTo(dbRelease.CreatedAt);
            await Assert.That(release.LastModifiedAt).IsEquivalentTo(dbRelease.LastModifiedAt);
            await Assert.That(release.Issues).IsEmpty();

            ReleaseDto release2 = releases.Single(entity => entity.Id.Equals(dbRelease2.Id));
            await Assert.That(release2.Id).IsEqualTo(dbRelease2.Id);
            await Assert.That(release2.Title).IsEqualTo(dbRelease2.Title);
            await Assert.That(release2.Notes).IsEqualTo(dbRelease2.Notes);
            await Assert.That(release2.State).IsEqualTo(dbRelease2.State);
            await Assert.That(release2.Date).IsEquivalentTo(dbRelease2.Date);
            await Assert.That(release2.CreatedAt).IsEquivalentTo(dbRelease2.CreatedAt);
            await Assert.That(release2.LastModifiedAt).IsEquivalentTo(dbRelease2.LastModifiedAt);
            await Assert.That(release2.Issues).IsNotEmpty();
            await Assert.That(release2.Issues.Count).IsEqualTo(1);
            ReleaseIssueDto issue = release2.Issues.First();
            await Assert.That(issue.Id).IsEqualTo(dbIssue.Id);
            await Assert.That(issue.Milestone).IsNotNull();
            await Assert.That(issue.Milestone!.Id).IsEqualTo(dbIssue.Milestone.Id);
            await Assert.That(issue.Vehicle).IsNotNull();
            await Assert.That(issue.Vehicle!.Id).IsEqualTo(dbIssue.Vehicle.Id);
            await Assert.That(issue.Vehicle!.Translations).IsEmpty();
            await Assert.That(issue.Vehicle!.Photos).IsEmpty();

            List<Edge<ReleaseDto>> edges = response.Data.Releases.Edges.ToList();
            await Assert.That(edges.Count).IsEqualTo(2);

            ReleaseDto edgeRelease = edges.Single(entity => entity.Node.Id.Equals(dbRelease.Id)).Node;
            await Assert.That(edgeRelease.Id).IsEqualTo(dbRelease.Id);
            await Assert.That(edgeRelease.Title).IsEqualTo(dbRelease.Title);
            await Assert.That(edgeRelease.Notes).IsNull();
            await Assert.That(edgeRelease.State).IsEqualTo(dbRelease.State);
            await Assert.That(edgeRelease.Date).IsNull();
            await Assert.That(edgeRelease.CreatedAt).IsEquivalentTo(dbRelease.CreatedAt);
            await Assert.That(edgeRelease.LastModifiedAt).IsEquivalentTo(dbRelease.LastModifiedAt);
            await Assert.That(edgeRelease.Issues).IsEmpty();

            ReleaseDto edgeRelease2 = edges.Single(entity => entity.Node.Id.Equals(dbRelease2.Id)).Node;
            await Assert.That(edgeRelease2.Id).IsEqualTo(dbRelease2.Id);
            await Assert.That(edgeRelease2.Title).IsEqualTo(dbRelease2.Title);
            await Assert.That(edgeRelease2.Notes).IsEqualTo(dbRelease2.Notes);
            await Assert.That(edgeRelease2.State).IsEqualTo(dbRelease2.State);
            await Assert.That(edgeRelease2.Date).IsEquivalentTo(dbRelease2.Date);
            await Assert.That(edgeRelease2.CreatedAt).IsEquivalentTo(dbRelease2.CreatedAt);
            await Assert.That(edgeRelease2.LastModifiedAt).IsEquivalentTo(dbRelease2.LastModifiedAt);
            await Assert.That(edgeRelease2.Issues).IsNotEmpty();
            await Assert.That(edgeRelease2.Issues.Count).IsEqualTo(1);
            ReleaseIssueDto edgeIssue = edgeRelease2.Issues.First();
            await Assert.That(edgeIssue.Id).IsEqualTo(dbIssue.Id);
            await Assert.That(edgeIssue.Milestone).IsNotNull();
            await Assert.That(edgeIssue.Milestone!.Id).IsEqualTo(dbIssue.Milestone.Id);
            await Assert.That(edgeIssue.Vehicle).IsNotNull();
            await Assert.That(edgeIssue.Vehicle!.Id).IsEqualTo(dbIssue.Vehicle.Id);
            await Assert.That(edgeIssue.Vehicle!.Translations).IsEmpty();
            await Assert.That(edgeIssue.Vehicle!.Photos).IsEmpty();
        }
    }

    [Test]
    public async Task GetReleaseShouldReturnNullIfReleaseWithGivenIdDoesNotExist()
    {
        // Arrange
        await SeedDatabaseAsync(context =>
        {
            context.Releases.Add(new Release
            {
                Id = new Guid("5888CDB6-57E2-4774-B6E8-7AABE82E2A5F"),
                Title = "Release 1",
                Notes = "Notes 1",
                State = ReleaseState.Released,
                Date = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow.AddDays(-1)
            });
        });
        const string id = "F1378377-9846-4168-A595-E763CD61CD9F";
        GraphQLRequest request = CreateGetReleaseRequest(id);

        // Act
        GraphQLResponse<GetReleaseResponse> response = await CreateGraphQLClient().SendQueryAsync<GetReleaseResponse>(request);

        // Assert
        using (Assert.Multiple())
        {
            await Assert.That(response).IsNotNull();
            await Assert.That(response.Errors).IsNull();
            // response.Errors!.First().Extensions!["message"].Should().Be($"No Label found with id '{id}'.", StringComparer.InvariantCultureIgnoreCase);
            await Assert.That(response.Data).IsNotNull();
            await Assert.That(response.Data.Release).IsNull();
        }
    }

    [Test]
    public async Task GetReleaseShouldReturnNullIfReleasesAreEmpty()
    {
        // Arrange
        await CheckDbContentAsync(async context =>
        {
            await Assert.That(context.Releases).IsEmpty();
        });
        const string id = "F1378377-9846-4168-A595-E763CD61CD9F";
        GraphQLRequest request = CreateGetReleaseRequest(id);

        // Act
        GraphQLResponse<GetReleaseResponse> response = await CreateGraphQLClient().SendQueryAsync<GetReleaseResponse>(request);

        // Assert
        using (Assert.Multiple())
        {
            await Assert.That(response).IsNotNull();
            await Assert.That(response.Errors).IsNull();
            // response.Errors!.First().Extensions!["message"].Should().Be($"No Label found with id '{id}'.", StringComparer.InvariantCultureIgnoreCase);
            await Assert.That(response.Data).IsNotNull();
            await Assert.That(response.Data.Release).IsNull();
        }
    }

    [Test]
    public async Task GetReleaseShouldReturnReleaseWithGivenId()
    {
        // Arrange
        const string id = "F1378377-9846-4168-A595-E763CD61CD9F";
        Issue dbIssue = new Issue
        {
            Title = "issue title",
            State = IssueState.Closed,
            Milestone = new Milestone { Title = "milestone title", State = MilestoneState.Open },
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
        Release dbRelease = new Release
        {
            Id = new Guid(id),
            Title = "Release 2",
            Notes = "Notes 2",
            State = ReleaseState.Released,
            Date = DateTime.UtcNow,
            LastModifiedAt = DateTime.UtcNow.AddDays(-1),
            Issues = [dbIssue]
        };
        await SeedDatabaseAsync(context =>
        {
            context.Releases.Add(new Release
            {
                Id = new Guid("5888CDB6-57E2-4774-B6E8-7AABE82E2A5F"),
                Title = "Release 1",
                State = ReleaseState.Planned
            });
            context.Releases.Add(dbRelease);
        });
        GraphQLRequest request = CreateGetReleaseRequest(id);

        // Act
        GraphQLResponse<GetReleaseResponse> response = await CreateGraphQLClient().SendQueryAsync<GetReleaseResponse>(request);

        // Assert
        using (Assert.Multiple())
        {
            await Assert.That(response).IsNotNull();
            await Assert.That(response.Errors).IsNull();
            await Assert.That(response.Data).IsNotNull();
            ReleaseDto? release = response.Data.Release;

            await Assert.That(release).IsNotNull();
            await Assert.That(release!.Id).IsEqualTo(dbRelease.Id);
            await Assert.That(release.Title).IsEqualTo(dbRelease.Title);
            await Assert.That(release.Notes).IsEqualTo(dbRelease.Notes);
            await Assert.That(release.State).IsEqualTo(dbRelease.State);
            await Assert.That(release.Date).IsEquivalentTo(dbRelease.Date);
            await Assert.That(release.CreatedAt).IsEquivalentTo(dbRelease.CreatedAt);
            await Assert.That(release.LastModifiedAt).IsEquivalentTo(dbRelease.LastModifiedAt);
            await Assert.That(release.Issues).IsNotEmpty();
            await Assert.That(release.Issues.Count).IsEqualTo(1);
            ReleaseIssueDto issue = release.Issues.First();
            await Assert.That(issue.Id).IsEqualTo(dbIssue.Id);
            await Assert.That(issue.Milestone).IsNotNull();
            await Assert.That(issue.Milestone!.Id).IsEqualTo(dbIssue.Milestone.Id);
            await Assert.That(issue.Vehicle).IsNotNull();
            await Assert.That(issue.Vehicle!.Id).IsEqualTo(dbIssue.Vehicle.Id);
            await Assert.That(issue.Vehicle!.Translations).IsEmpty();
            await Assert.That(issue.Vehicle!.Photos).IsEmpty();
        }
    }

    private static GraphQLRequest CreateGetReleasesRequest()
    {
        GraphQLRequest queryRequest = new GraphQLRequest
        {
            Query = """
                    query releases
                    {
                        releases(first: 5)
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
                                    notes
                                    state
                                    date
                                    createdAt
                                    lastModifiedAt
                                    issues
                                    {
                                        id
                                        title
                                        milestone
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
                                notes
                                state
                                date
                                createdAt
                                lastModifiedAt
                                issues
                                {
                                    id
                                    title
                                    milestone
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
            OperationName = "releases"
        };
        return queryRequest;
    }

    private static GraphQLRequest CreateGetReleaseRequest(string id)
    {
        GraphQLRequest queryRequest = new GraphQLRequest
        {
            Query = $$"""
                    query release
                    {
                        release(id: "{{id}}")
                        {
                            id
                            title
                            notes
                            state
                            date
                            createdAt
                            lastModifiedAt
                            issues
                            {
                                id
                                title
                                milestone
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
            OperationName = "release"
        };
        return queryRequest;
    }
}
