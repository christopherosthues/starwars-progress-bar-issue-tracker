using GraphQL;
using StarWarsProgressBarIssueTracker.App.Tests.Helpers.GraphQL.Payloads;
using StarWarsProgressBarIssueTracker.App.Tests.Helpers.GraphQL.Payloads.Appearances;
using StarWarsProgressBarIssueTracker.Domain.Vehicles;
using StarWarsProgressBarIssueTracker.TestHelpers;

namespace StarWarsProgressBarIssueTracker.App.Tests.Integration.Queries;

[Category(TestCategory.Integration)]
public class AppearanceQueriesTests : IntegrationTestBase
{
    [Test]
    public async Task GetAppearancesShouldReturnEmptyListIfNoAppearanceExist()
    {
        // Arrange
        await CheckDbContentAsync(async context =>
        {
            await Assert.That(context.Appearances).IsEmpty();
        });
        var request = CreateGetAppearancesRequest();

        // Act
        var response = await GraphQLClient.SendQueryAsync<GetAppearancesResponse>(request);

        // Assert
        using (Assert.Multiple())
        {
            await Assert.That(response).IsNotNull();
            await Assert.That(response.Errors).IsNull().Or.IsEmpty();
            await Assert.That(response.Data).IsNotNull();
            await Assert.That(response.Data.Appearances.TotalCount).IsEqualTo(0);
            await Assert.That(response.Data.Appearances.PageInfo.HasNextPage).IsFalse();
            await Assert.That(response.Data.Appearances.PageInfo.HasPreviousPage).IsFalse();
            await Assert.That(response.Data.Appearances.PageInfo.StartCursor).IsNull();
            await Assert.That(response.Data.Appearances.PageInfo.EndCursor).IsNull();
            await Assert.That(response.Data.Appearances.Edges).IsEmpty();
            await Assert.That(response.Data.Appearances.Nodes).IsEmpty();
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
        await CheckDbContentAsync(async context =>
        {
            using (Assert.Multiple())
            {
                await Assert.That(context.Appearances).Contains(dbAppearance);
                await Assert.That(context.Appearances).Contains(dbAppearance2);
            }
        });
        var request = CreateGetAppearancesRequest();

        // Act
        var response = await GraphQLClient.SendQueryAsync<GetAppearancesResponse>(request);

        // Assert
        using (Assert.Multiple())
        {
            await Assert.That(response).IsNotNull();
            await Assert.That(response.Errors).IsNull().Or.IsEmpty();
            await Assert.That(response.Data).IsNotNull();

            await Assert.That(response.Data.Appearances.TotalCount).IsEqualTo(2);
            await Assert.That(response.Data.Appearances.PageInfo.HasNextPage).IsFalse();
            await Assert.That(response.Data.Appearances.PageInfo.HasPreviousPage).IsFalse();
            await Assert.That(response.Data.Appearances.PageInfo.StartCursor).IsNotNull();
            await Assert.That(response.Data.Appearances.PageInfo.EndCursor).IsNotNull();
            List<Appearance> appearances = response.Data.Appearances.Nodes.ToList();
            await Assert.That(appearances.Count).IsEqualTo(2);

            var appearance = appearances.Single(entity => entity.Id.Equals(dbAppearance.Id));
            await Assert.That(appearance.Id).IsEqualTo(dbAppearance.Id);
            await Assert.That(appearance.Title).IsEqualTo(dbAppearance.Title);
            await Assert.That(appearance.Description).IsEqualTo(dbAppearance.Description);
            await Assert.That(appearance.Color).IsEqualTo(dbAppearance.Color);
            await Assert.That(appearance.TextColor).IsEqualTo(dbAppearance.TextColor);
            await Assert.That(appearance.CreatedAt).IsEqualTo(dbAppearance.CreatedAt);
            await Assert.That(appearance.LastModifiedAt).IsEqualTo(dbAppearance.LastModifiedAt);

            var appearance2 = appearances.Single(entity => entity.Id.Equals(dbAppearance2.Id));
            await Assert.That(appearance2.Id).IsEqualTo(dbAppearance2.Id);
            await Assert.That(appearance2.Title).IsEqualTo(dbAppearance2.Title);
            await Assert.That(appearance2.Description).IsEqualTo(dbAppearance2.Description);
            await Assert.That(appearance2.Color).IsEqualTo(dbAppearance2.Color);
            await Assert.That(appearance2.TextColor).IsEqualTo(dbAppearance2.TextColor);
            await Assert.That(appearance2.CreatedAt).IsEqualTo(dbAppearance2.CreatedAt);
            await Assert.That(appearance2.LastModifiedAt).IsEqualTo(dbAppearance2.LastModifiedAt);

            List<Edge<Appearance>> edges = response.Data.Appearances.Edges.ToList();
            await Assert.That(edges.Count).IsEqualTo(2);

            Appearance edgeAppearance = edges.Single(entity => entity.Node.Id.Equals(dbAppearance.Id)).Node;
            await Assert.That(edgeAppearance.Id).IsEqualTo(dbAppearance.Id);
            await Assert.That(edgeAppearance.Title).IsEqualTo(dbAppearance.Title);
            await Assert.That(edgeAppearance.Description).IsEqualTo(dbAppearance.Description);
            await Assert.That(edgeAppearance.Color).IsEqualTo(dbAppearance.Color);
            await Assert.That(edgeAppearance.TextColor).IsEqualTo(dbAppearance.TextColor);
            await Assert.That(edgeAppearance.CreatedAt).IsEqualTo(dbAppearance.CreatedAt);
            await Assert.That(edgeAppearance.LastModifiedAt).IsEqualTo(dbAppearance.LastModifiedAt);

            Appearance edgeAppearance2 = edges.Single(entity => entity.Node.Id.Equals(dbAppearance2.Id)).Node;
            await Assert.That(edgeAppearance2.Id).IsEqualTo(dbAppearance2.Id);
            await Assert.That(edgeAppearance2.Title).IsEqualTo(dbAppearance2.Title);
            await Assert.That(edgeAppearance2.Description).IsEqualTo(dbAppearance2.Description);
            await Assert.That(edgeAppearance2.Color).IsEqualTo(dbAppearance2.Color);
            await Assert.That(edgeAppearance2.TextColor).IsEqualTo(dbAppearance2.TextColor);
            await Assert.That(edgeAppearance2.CreatedAt).IsEqualTo(dbAppearance2.CreatedAt);
            await Assert.That(edgeAppearance2.LastModifiedAt).IsEqualTo(dbAppearance2.LastModifiedAt);
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
        using (Assert.Multiple())
        {
            await Assert.That(response).IsNotNull();
            await Assert.That(response.Errors).IsNull();
            // response.Errors!.First().Extensions!["message"].Should().Be($"No Appearance found with id '{id}'.",
            //     StringComparer.InvariantCultureIgnoreCase);
            await Assert.That(response.Data).IsNotNull();
            await Assert.That(response.Data.Appearance).IsNull();
        }
    }

    [Test]
    public async Task GetAppearanceShouldReturnNullIfAppearancesAreEmpty()
    {
        // Arrange
        await CheckDbContentAsync(async context =>
        {
            await Assert.That(context.Appearances).IsEmpty();
        });
        const string id = "F1378377-9846-4168-A595-E763CD61CD9F";
        var request = CreateGetAppearanceRequest(id);

        // Act
        var response = await GraphQLClient.SendQueryAsync<GetAppearanceResponse>(request);

        // Assert
        using (Assert.Multiple())
        {
            await Assert.That(response).IsNotNull();
            await Assert.That(response.Errors).IsNull();
            // response.Errors!.First().Extensions!["message"].Should().Be($"No Appearance found with id '{id}'.",
            //     StringComparer.InvariantCultureIgnoreCase);
            await Assert.That(response.Data).IsNotNull();
            await Assert.That(response.Data.Appearance).IsNull();
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
        using (Assert.Multiple())
        {
            await Assert.That(response).IsNotNull();
            await Assert.That(response.Errors).IsNull();
            await Assert.That(response.Data).IsNotNull();
            var appearance = response.Data.Appearance;

            await Assert.That(appearance).IsNotNull();
            await Assert.That(appearance!.Id).IsEqualTo(dbAppearance.Id);
            await Assert.That(appearance.Title).IsEqualTo(dbAppearance.Title);
            await Assert.That(appearance.Description).IsEqualTo(dbAppearance.Description);
            await Assert.That(appearance.Color).IsEqualTo(dbAppearance.Color);
            await Assert.That(appearance.TextColor).IsEqualTo(dbAppearance.TextColor);
            await Assert.That(appearance.CreatedAt).IsEqualTo(dbAppearance.CreatedAt);
            await Assert.That(appearance.LastModifiedAt).IsEqualTo(dbAppearance.LastModifiedAt);
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
