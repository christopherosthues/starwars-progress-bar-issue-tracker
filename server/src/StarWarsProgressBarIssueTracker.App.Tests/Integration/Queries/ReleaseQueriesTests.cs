using FluentAssertions;
using FluentAssertions.Execution;
using GraphQL;
using StarWarsProgressBarIssueTracker.App.Queries;
using StarWarsProgressBarIssueTracker.App.Tests.Helpers.GraphQL.Payloads;
using StarWarsProgressBarIssueTracker.App.Tests.Helpers.GraphQL.Payloads.Releases;
using StarWarsProgressBarIssueTracker.Domain.Issues;
using StarWarsProgressBarIssueTracker.Domain.Milestones;
using StarWarsProgressBarIssueTracker.Domain.Releases;
using StarWarsProgressBarIssueTracker.Domain.Vehicles;
using StarWarsProgressBarIssueTracker.TestHelpers;

namespace StarWarsProgressBarIssueTracker.App.Tests.Integration.Queries;

[Category(TestCategory.Integration)]
public class ReleaseQueriesTests : IntegrationTestBase
{
    [Test]
    public async Task GetReleasesShouldReturnEmptyListIfNoReleaseExist()
    {
        // Arrange
        CheckDbContentAsync(context =>
        {
            context.Releases.Should().BeEmpty();
        });
        var request = CreateGetReleasesRequest();

        // Act
        var response = await GraphQLClient.SendQueryAsync<GetReleasesResponse>(request);

        // Assert
        using (Assert.Multiple())
        {
            response.Should().NotBeNull();
            response.Errors.Should().BeNullOrEmpty();
            response.Data.Should().NotBeNull();
            response.Data.Releases.TotalCount.Should().Be(0);
            response.Data.Releases.PageInfo.HasNextPage.Should().BeFalse();
            response.Data.Releases.PageInfo.HasPreviousPage.Should().BeFalse();
            response.Data.Releases.PageInfo.StartCursor.Should().BeNull();
            response.Data.Releases.PageInfo.EndCursor.Should().BeNull();
            response.Data.Releases.Edges.Should().BeEmpty();
            response.Data.Releases.Nodes.Should().BeEmpty();
        }
    }

    [Test]
    public async Task GetReleasesShouldReturnAllReleases()
    {
        // Arrange
        var dbRelease = new Release
        {
            Title = "Release 1",
            State = ReleaseState.Planned
        };

        var dbIssue = new Issue
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
        var dbRelease2 = new Release
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
        CheckDbContentAsync(context =>
        {
            var dbReleases = context.Releases.ToList();
            dbReleases.Any(release => release.Id.Equals(dbRelease.Id)).Should().BeTrue();
            dbReleases.Any(release => release.Id.Equals(dbRelease2.Id)).Should().BeTrue();
            var dbIssues = context.Issues.ToList();
            dbIssues.Any(issue => issue.Id.Equals(dbIssue.Id)).Should().BeTrue();
        });
        var request = CreateGetReleasesRequest();

        // Act
        var response = await GraphQLClient.SendQueryAsync<GetReleasesResponse>(request);

        // Assert
        using (Assert.Multiple())
        {
            response.Should().NotBeNull();
            response.Errors.Should().BeNullOrEmpty();
            response.Data.Should().NotBeNull();

            response.Data.Releases.TotalCount.Should().Be(2);
            response.Data.Releases.PageInfo.HasNextPage.Should().BeFalse();
            response.Data.Releases.PageInfo.HasPreviousPage.Should().BeFalse();
            response.Data.Releases.PageInfo.StartCursor.Should().NotBeNull();
            response.Data.Releases.PageInfo.EndCursor.Should().NotBeNull();
            List<Release> releases = response.Data.Releases.Nodes.ToList();
            releases.Count.Should().Be(2);

            Release release = releases.Single(entity => entity.Id.Equals(dbRelease.Id));
            release.Id.Should().Be(dbRelease.Id);
            release.Title.Should().Be(dbRelease.Title);
            release.Notes.Should().BeNull();
            release.State.Should().Be(dbRelease.State);
            release.Date.Should().BeNull();
            release.CreatedAt.Should().BeCloseTo(dbRelease.CreatedAt, TimeSpan.FromMilliseconds(300));
            release.LastModifiedAt.Should().Be(dbRelease.LastModifiedAt);
            release.Issues.Should().BeEmpty();

            Release release2 = releases.Single(entity => entity.Id.Equals(dbRelease2.Id));
            release2.Id.Should().Be(dbRelease2.Id);
            release2.Title.Should().Be(dbRelease2.Title);
            release2.Notes.Should().Be(dbRelease2.Notes);
            release2.State.Should().Be(dbRelease2.State);
            release2.Date.Should().BeCloseTo(dbRelease2.Date!.Value, TimeSpan.FromMilliseconds(300));
            release2.CreatedAt.Should().BeCloseTo(dbRelease2.CreatedAt, TimeSpan.FromMilliseconds(300));
            release2.LastModifiedAt.Should().BeCloseTo(dbRelease2.LastModifiedAt!.Value, TimeSpan.FromMilliseconds(300));
            release2.Issues.Should().NotBeEmpty();
            release2.Issues.Count.Should().Be(1);
            Issue issue = release2.Issues.First();
            issue.Id.Should().Be(dbIssue.Id);
            issue.Milestone.Should().NotBeNull();
            issue.Milestone!.Id.Should().Be(dbIssue.Milestone.Id);
            issue.Vehicle.Should().NotBeNull();
            issue.Vehicle!.Id.Should().Be(dbIssue.Vehicle.Id);
            issue.Vehicle!.Translations.Should().BeEmpty();
            issue.Vehicle!.Photos.Should().BeEmpty();

            List<Edge<Release>> edges = response.Data.Releases.Edges.ToList();
            edges.Count.Should().Be(2);

            Release edgeRelease = edges.Single(entity => entity.Node.Id.Equals(dbRelease.Id)).Node;
            edgeRelease.Id.Should().Be(dbRelease.Id);
            edgeRelease.Title.Should().Be(dbRelease.Title);
            edgeRelease.Notes.Should().BeNull();
            edgeRelease.State.Should().Be(dbRelease.State);
            edgeRelease.Date.Should().BeNull();
            edgeRelease.CreatedAt.Should().BeCloseTo(dbRelease.CreatedAt, TimeSpan.FromMilliseconds(300));
            edgeRelease.LastModifiedAt.Should().Be(dbRelease.LastModifiedAt);
            edgeRelease.Issues.Should().BeEmpty();

            Release edgeRelease2 = edges.Single(entity => entity.Node.Id.Equals(dbRelease2.Id)).Node;
            edgeRelease2.Id.Should().Be(dbRelease2.Id);
            edgeRelease2.Title.Should().Be(dbRelease2.Title);
            edgeRelease2.Notes.Should().Be(dbRelease2.Notes);
            edgeRelease2.State.Should().Be(dbRelease2.State);
            edgeRelease2.Date.Should().BeCloseTo(dbRelease2.Date!.Value, TimeSpan.FromMilliseconds(300));
            edgeRelease2.CreatedAt.Should().BeCloseTo(dbRelease2.CreatedAt, TimeSpan.FromMilliseconds(300));
            edgeRelease2.LastModifiedAt.Should().BeCloseTo(dbRelease2.LastModifiedAt!.Value, TimeSpan.FromMilliseconds(300));
            edgeRelease2.Issues.Should().NotBeEmpty();
            edgeRelease2.Issues.Count.Should().Be(1);
            Issue edgeIssue = edgeRelease2.Issues.First();
            edgeIssue.Id.Should().Be(dbIssue.Id);
            edgeIssue.Milestone.Should().NotBeNull();
            edgeIssue.Milestone!.Id.Should().Be(dbIssue.Milestone.Id);
            edgeIssue.Vehicle.Should().NotBeNull();
            edgeIssue.Vehicle!.Id.Should().Be(dbIssue.Vehicle.Id);
            edgeIssue.Vehicle!.Translations.Should().BeEmpty();
            edgeIssue.Vehicle!.Photos.Should().BeEmpty();
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
        var request = CreateGetReleaseRequest(id);

        // Act
        var response = await GraphQLClient.SendQueryAsync<GetReleaseResponse>(request);

        // Assert
        using (Assert.Multiple())
        {
            response.Should().NotBeNull();
            response.Errors.Should().BeNull();
            // response.Errors!.First().Extensions!["message"].Should().Be($"No Label found with id '{id}'.",
            //     StringComparer.InvariantCultureIgnoreCase);
            response.Data.Should().NotBeNull();
            response.Data.Release.Should().BeNull();
        }
    }

    [Test]
    public async Task GetReleaseShouldReturnNullIfReleasesAreEmpty()
    {
        // Arrange
        CheckDbContentAsync(context =>
        {
            context.Releases.Should().BeEmpty();
        });
        const string id = "F1378377-9846-4168-A595-E763CD61CD9F";
        var request = CreateGetReleaseRequest(id);

        // Act
        var response = await GraphQLClient.SendQueryAsync<GetReleaseResponse>(request);

        // Assert
        using (Assert.Multiple())
        {
            response.Should().NotBeNull();
            response.Errors.Should().BeNull();
            // response.Errors!.First().Extensions!["message"].Should().Be($"No Label found with id '{id}'.",
            //     StringComparer.InvariantCultureIgnoreCase);
            response.Data.Should().NotBeNull();
            response.Data.Release.Should().BeNull();
        }
    }

    [Test]
    public async Task GetReleaseShouldReturnReleaseWithGivenId()
    {
        // Arrange
        const string id = "F1378377-9846-4168-A595-E763CD61CD9F";
        var dbIssue = new Issue
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
        var dbRelease = new Release
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
        var request = CreateGetReleaseRequest(id);

        // Act
        var response = await GraphQLClient.SendQueryAsync<GetReleaseResponse>(request);

        // Assert
        using (Assert.Multiple())
        {
            response.Should().NotBeNull();
            response.Errors.Should().BeNull();
            response.Data.Should().NotBeNull();
            var release = response.Data.Release;

            release.Should().NotBeNull();
            release!.Id.Should().Be(dbRelease.Id);
            release.Title.Should().Be(dbRelease.Title);
            release.Notes.Should().Be(dbRelease.Notes);
            release.State.Should().Be(dbRelease.State);
            release.Date.Should().BeCloseTo(dbRelease.Date!.Value, TimeSpan.FromMilliseconds(300));
            release.CreatedAt.Should().BeCloseTo(dbRelease.CreatedAt, TimeSpan.FromMilliseconds(300));
            release.LastModifiedAt.Should().BeCloseTo(dbRelease.LastModifiedAt!.Value, TimeSpan.FromMilliseconds(300));
            release.Issues.Should().NotBeEmpty();
            release.Issues.Count.Should().Be(1);
            var issue = release.Issues.First();
            issue.Id.Should().Be(dbIssue.Id);
            issue.Milestone.Should().NotBeNull();
            issue.Milestone!.Id.Should().Be(dbIssue.Milestone.Id);
            issue.Vehicle.Should().NotBeNull();
            issue.Vehicle!.Id.Should().Be(dbIssue.Vehicle.Id);
            issue.Vehicle!.Translations.Should().BeEmpty();
            issue.Vehicle!.Photos.Should().BeEmpty();
        }
    }

    private static GraphQLRequest CreateGetReleasesRequest()
    {
        var queryRequest = new GraphQLRequest
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
        var queryRequest = new GraphQLRequest
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
