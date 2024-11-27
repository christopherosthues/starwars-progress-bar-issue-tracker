using FluentAssertions;
using FluentAssertions.Execution;
using GraphQL;
using StarWarsProgressBarIssueTracker.App.Queries;
using StarWarsProgressBarIssueTracker.App.Tests.Helpers.GraphQL.Payloads;
using StarWarsProgressBarIssueTracker.App.Tests.Helpers.GraphQL.Payloads.Appearances;
using StarWarsProgressBarIssueTracker.Domain.Vehicles;
using StarWarsProgressBarIssueTracker.TestHelpers;

namespace StarWarsProgressBarIssueTracker.App.Tests.Integration.Queries;

[TestFixture(TestOf = typeof(IssueTrackerQueries))]
[Category(TestCategory.Integration)]
public class AppearanceQueriesTests : IntegrationTestBase
{
    [Test]
    public async Task GetAppearancesShouldReturnEmptyListIfNoAppearanceExist()
    {
        // Arrange
        CheckDbContent(context =>
        {
            context.Appearances.Should().BeEmpty();
        });
        var request = CreateGetAppearancesRequest();

        // Act
        var response = await GraphQLClient.SendQueryAsync<GetAppearancesResponse>(request);

        // Assert
        using (new AssertionScope())
        {
            response.Should().NotBeNull();
            response.Errors.Should().BeNullOrEmpty();
            response.Data.Should().NotBeNull();
            response.Data.Appearances.TotalCount.Should().Be(0);
            response.Data.Appearances.PageInfo.HasNextPage.Should().BeFalse();
            response.Data.Appearances.PageInfo.HasPreviousPage.Should().BeFalse();
            response.Data.Appearances.PageInfo.StartCursor.Should().BeNull();
            response.Data.Appearances.PageInfo.EndCursor.Should().BeNull();
            response.Data.Appearances.Edges.Should().BeEmpty();
            response.Data.Appearances.Nodes.Should().BeEmpty();
        }
    }

    [Test]
    public async Task GetAppearancesShouldReturnAllAppearances()
    {
        // Arrange
        var dbAppearance = new Appearance
        {
            Color = "001122",
            TextColor = "223344",
            Title = "Appearance 1",
            Description = "Description 1"
        };
        var dbAppearance2 = new Appearance
        {
            Color = "112233",
            TextColor = "334455",
            Title = "Appearance 2",
            Description = "Description 2"
        };
        await SeedDatabaseAsync(context =>
        {
            context.Appearances.Add(dbAppearance);
            context.Appearances.Add(dbAppearance2);
        });
        CheckDbContent(context =>
        {
            context.Appearances.Should().ContainEquivalentOf(dbAppearance);
            context.Appearances.Should().ContainEquivalentOf(dbAppearance2);
        });
        var request = CreateGetAppearancesRequest();

        // Act
        var response = await GraphQLClient.SendQueryAsync<GetAppearancesResponse>(request);

        // Assert
        using (new AssertionScope())
        {
            response.Should().NotBeNull();
            response.Errors.Should().BeNullOrEmpty();
            response.Data.Should().NotBeNull();

            response.Data.Appearances.TotalCount.Should().Be(2);
            response.Data.Appearances.PageInfo.HasNextPage.Should().BeFalse();
            response.Data.Appearances.PageInfo.HasPreviousPage.Should().BeFalse();
            response.Data.Appearances.PageInfo.StartCursor.Should().NotBeNull();
            response.Data.Appearances.PageInfo.EndCursor.Should().NotBeNull();
            List<Appearance> appearances = response.Data.Appearances.Nodes.ToList();
            appearances.Count.Should().Be(2);

            var appearance = appearances.Single(entity => entity.Id.Equals(dbAppearance.Id));
            appearance.Id.Should().Be(dbAppearance.Id);
            appearance.Title.Should().Be(dbAppearance.Title);
            appearance.Description.Should().Be(dbAppearance.Description);
            appearance.Color.Should().Be(dbAppearance.Color);
            appearance.TextColor.Should().Be(dbAppearance.TextColor);
            appearance.CreatedAt.Should().BeCloseTo(dbAppearance.CreatedAt, TimeSpan.FromMilliseconds(300));
            appearance.LastModifiedAt.Should().Be(dbAppearance.LastModifiedAt);

            var appearance2 = appearances.Single(entity => entity.Id.Equals(dbAppearance2.Id));
            appearance2.Id.Should().Be(dbAppearance2.Id);
            appearance2.Title.Should().Be(dbAppearance2.Title);
            appearance2.Description.Should().Be(dbAppearance2.Description);
            appearance2.Color.Should().Be(dbAppearance2.Color);
            appearance2.TextColor.Should().Be(dbAppearance2.TextColor);
            appearance2.CreatedAt.Should().BeCloseTo(dbAppearance2.CreatedAt, TimeSpan.FromMilliseconds(300));
            appearance2.LastModifiedAt.Should().Be(dbAppearance2.LastModifiedAt);

            List<Edge<Appearance>> edges = response.Data.Appearances.Edges.ToList();
            edges.Count.Should().Be(2);

            Appearance edgeAppearance = edges.Single(entity => entity.Node.Id.Equals(dbAppearance.Id)).Node;
            edgeAppearance.Id.Should().Be(dbAppearance.Id);
            edgeAppearance.Title.Should().Be(dbAppearance.Title);
            edgeAppearance.Description.Should().Be(dbAppearance.Description);
            edgeAppearance.Color.Should().Be(dbAppearance.Color);
            edgeAppearance.TextColor.Should().Be(dbAppearance.TextColor);
            edgeAppearance.CreatedAt.Should().BeCloseTo(dbAppearance.CreatedAt, TimeSpan.FromMilliseconds(300));
            edgeAppearance.LastModifiedAt.Should().Be(dbAppearance.LastModifiedAt);

            Appearance edgeAppearance2 = edges.Single(entity => entity.Node.Id.Equals(dbAppearance2.Id)).Node;
            edgeAppearance2.Id.Should().Be(dbAppearance2.Id);
            edgeAppearance2.Title.Should().Be(dbAppearance2.Title);
            edgeAppearance2.Description.Should().Be(dbAppearance2.Description);
            edgeAppearance2.Color.Should().Be(dbAppearance2.Color);
            edgeAppearance2.TextColor.Should().Be(dbAppearance2.TextColor);
            edgeAppearance2.CreatedAt.Should().BeCloseTo(dbAppearance2.CreatedAt, TimeSpan.FromMilliseconds(300));
            edgeAppearance2.LastModifiedAt.Should().Be(dbAppearance2.LastModifiedAt);
        }
    }

    [Test]
    public async Task GetAppearanceShouldReturnNullIfAppearanceWithGivenIdDoesNotExist()
    {
        // Arrange
        await SeedDatabaseAsync(context =>
        {
            context.Appearances.Add(new Appearance
            {
                Id = new Guid("5888CDB6-57E2-4774-B6E8-7AABE82E2A5F"),
                Color = "001122",
                TextColor = "223344",
                Title = "Appearance 1",
                Description = "Description 1"
            });
        });
        const string id = "F1378377-9846-4168-A595-E763CD61CD9F";
        var request = CreateGetAppearanceRequest(id);

        // Act
        var response = await GraphQLClient.SendQueryAsync<GetAppearanceResponse>(request);

        // Assert
        using (new AssertionScope())
        {
            response.Should().NotBeNull();
            response.Errors.Should().BeNull();
            // response.Errors!.First().Extensions!["message"].Should().Be($"No Appearance found with id '{id}'.",
            //     StringComparer.InvariantCultureIgnoreCase);
            response.Data.Should().NotBeNull();
            response.Data.Appearance.Should().BeNull();
        }
    }

    [Test]
    public async Task GetAppearanceShouldReturnNullIfAppearancesAreEmpty()
    {
        // Arrange
        CheckDbContent(context =>
        {
            context.Appearances.Should().BeEmpty();
        });
        const string id = "F1378377-9846-4168-A595-E763CD61CD9F";
        var request = CreateGetAppearanceRequest(id);

        // Act
        var response = await GraphQLClient.SendQueryAsync<GetAppearanceResponse>(request);

        // Assert
        using (new AssertionScope())
        {
            response.Should().NotBeNull();
            response.Errors.Should().BeNull();
            // response.Errors!.First().Extensions!["message"].Should().Be($"No Appearance found with id '{id}'.",
            //     StringComparer.InvariantCultureIgnoreCase);
            response.Data.Should().NotBeNull();
            response.Data.Appearance.Should().BeNull();
        }
    }

    [Test]
    public async Task GetAppearanceShouldReturnAppearanceWithGivenId()
    {
        // Arrange
        const string id = "F1378377-9846-4168-A595-E763CD61CD9F";
        var dbAppearance = new Appearance
        {
            Id = new Guid(id),
            Color = "112233",
            TextColor = "334455",
            Title = "Appearance 2",
            Description = "Description 2"
        };
        await SeedDatabaseAsync(context =>
        {
            context.Appearances.Add(new Appearance
            {
                Id = new Guid("5888CDB6-57E2-4774-B6E8-7AABE82E2A5F"),
                Color = "001122",
                TextColor = "223344",
                Title = "Appearance 1",
                Description = "Description 1"
            });
            context.Appearances.Add(dbAppearance);
        });
        var request = CreateGetAppearanceRequest(id);

        // Act
        var response = await GraphQLClient.SendQueryAsync<GetAppearanceResponse>(request);

        // Assert
        using (new AssertionScope())
        {
            response.Should().NotBeNull();
            response.Errors.Should().BeNull();
            response.Data.Should().NotBeNull();
            var appearance = response.Data.Appearance;

            appearance.Should().NotBeNull();
            appearance!.Id.Should().Be(dbAppearance.Id);
            appearance.Title.Should().Be(dbAppearance.Title);
            appearance.Description.Should().Be(dbAppearance.Description);
            appearance.Color.Should().Be(dbAppearance.Color);
            appearance.TextColor.Should().Be(dbAppearance.TextColor);
            appearance.CreatedAt.Should().BeCloseTo(dbAppearance.CreatedAt, TimeSpan.FromMilliseconds(300));
            appearance.LastModifiedAt.Should().Be(dbAppearance.LastModifiedAt);
        }
    }

    private static GraphQLRequest CreateGetAppearancesRequest()
    {
        var queryRequest = new GraphQLRequest
        {
            Query = """
                    query appearances
                    {
                        appearances(first: 5)
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
                                    color
                                    textColor
                                    createdAt
                                    lastModifiedAt
                                }
                            }
                            nodes {
                                id
                                title
                                description
                                color
                                textColor
                                createdAt
                                lastModifiedAt
                            }
                        }
                    }
                    """,
            OperationName = "appearances"
        };
        return queryRequest;
    }

    private static GraphQLRequest CreateGetAppearanceRequest(string id)
    {
        var queryRequest = new GraphQLRequest
        {
            Query = $$"""
                    query appearance
                    {
                        appearance(id: "{{id}}")
                        {
                            id
                            title
                            description
                            color
                            textColor
                            createdAt
                            lastModifiedAt
                        }
                    }
                    """,
            OperationName = "appearance"
        };
        return queryRequest;
    }
}
