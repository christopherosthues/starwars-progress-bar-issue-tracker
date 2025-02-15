using GraphQL;
using StarWarsProgressBarIssueTracker.App.Labels;
using StarWarsProgressBarIssueTracker.App.Tests.Helpers;
using StarWarsProgressBarIssueTracker.App.Tests.Helpers.GraphQL.Payloads;
using StarWarsProgressBarIssueTracker.App.Tests.Helpers.GraphQL.Payloads.Labels;
using StarWarsProgressBarIssueTracker.Domain.Issues;
using StarWarsProgressBarIssueTracker.Domain.Labels;
using StarWarsProgressBarIssueTracker.Domain.Releases;
using StarWarsProgressBarIssueTracker.Domain.Vehicles;
using StarWarsProgressBarIssueTracker.TestHelpers;

namespace StarWarsProgressBarIssueTracker.App.Tests.Integration.Queries;

[Category(TestCategory.Integration)]
[NotInParallel(NotInParallelTests.LabelRetrieval)]
public class LabelQueriesTests : IntegrationTestBase
{
    // TODO: Check DoesNotContain and Contains
    [Test]
    public async Task GetLabelsShouldReturnEmptyListIfNoLabelExist()
    {
        // Arrange
        await CheckDbContentAsync(async context =>
        {
            await Assert.That(context.Labels).IsEmpty();
        });
        GraphQLRequest request = CreateGetLabelsRequest();

        // Act
        GraphQLResponse<GetLabelsResponse> response = await CreateGraphQLClient().SendQueryAsync<GetLabelsResponse>(request);

        // Assert
        using (Assert.Multiple())
        {
            await Assert.That(response).IsNotNull();
            await Assert.That(response.Errors).IsNull().Or.IsEmpty();
            await Assert.That(response.Data).IsNotNull();
            await Assert.That(response.Data.Labels.TotalCount).IsEqualTo(0);
            await Assert.That(response.Data.Labels.PageInfo.HasNextPage).IsFalse();
            await Assert.That(response.Data.Labels.PageInfo.HasPreviousPage).IsFalse();
            await Assert.That(response.Data.Labels.PageInfo.StartCursor).IsNull();
            await Assert.That(response.Data.Labels.PageInfo.EndCursor).IsNull();
            await Assert.That(response.Data.Labels.Edges).IsEmpty();
            await Assert.That(response.Data.Labels.Nodes).IsEmpty();
        }
    }

    [Test]
    public async Task GetLabelsShouldReturnAllLabels()
    {
        // Arrange
        Label dbLabel = new Label
        {
            Color = "001122",
            TextColor = "223344",
            Title = "Label 1",
            Description = "Description 1"
        };
        Label dbLabel2 = new Label
        {
            Color = "112233",
            TextColor = "334455",
            Title = "Label 2",
            Description = "Description 2"
        };
        await SeedDatabaseAsync(context =>
        {
            context.Labels.Add(dbLabel);
            context.Labels.Add(dbLabel2);
        });
        await CheckDbContentAsync(async context =>
        {
            using (Assert.Multiple())
            {
                await Assert.That(context.Labels).Contains(dbLabel);
                await Assert.That(context.Labels).Contains(dbLabel2);
            }
        });
        GraphQLRequest request = CreateGetLabelsRequest();

        // Act
        GraphQLResponse<GetLabelsResponse> response = await CreateGraphQLClient().SendQueryAsync<GetLabelsResponse>(request);

        // Assert
        using (Assert.Multiple())
        {
            await Assert.That(response).IsNotNull();
            await Assert.That(response.Errors).IsNull().Or.IsEmpty();
            await Assert.That(response.Data).IsNotNull();

            await Assert.That(response.Data.Labels.TotalCount).IsEqualTo(2);
            await Assert.That(response.Data.Labels.PageInfo.HasNextPage).IsFalse();
            await Assert.That(response.Data.Labels.PageInfo.HasPreviousPage).IsFalse();
            await Assert.That(response.Data.Labels.PageInfo.StartCursor).IsNotNull();
            await Assert.That(response.Data.Labels.PageInfo.EndCursor).IsNotNull();
            List<LabelDto> labels = response.Data.Labels.Nodes.ToList();
            await Assert.That(labels.Count).IsEqualTo(2);

            LabelDto label = labels.Single(entity => entity.Id.Equals(dbLabel.Id));
            await Assert.That(label.Id).IsEqualTo(dbLabel.Id);
            await Assert.That(label.Title).IsEqualTo(dbLabel.Title);
            await Assert.That(label.Description).IsEqualTo(dbLabel.Description);
            await Assert.That(label.Color).IsEqualTo(dbLabel.Color);
            await Assert.That(label.TextColor).IsEqualTo(dbLabel.TextColor);
            await Assert.That(label.CreatedAt).IsEquivalentTo(dbLabel.CreatedAt);
            await Assert.That(label.LastModifiedAt).IsEquivalentTo(dbLabel.LastModifiedAt);

            LabelDto label2 = labels.Single(entity => entity.Id.Equals(dbLabel2.Id));
            await Assert.That(label2.Id).IsEqualTo(dbLabel2.Id);
            await Assert.That(label2.Title).IsEqualTo(dbLabel2.Title);
            await Assert.That(label2.Description).IsEqualTo(dbLabel2.Description);
            await Assert.That(label2.Color).IsEqualTo(dbLabel2.Color);
            await Assert.That(label2.TextColor).IsEqualTo(dbLabel2.TextColor);
            await Assert.That(label2.CreatedAt).IsEquivalentTo(dbLabel2.CreatedAt);
            await Assert.That(label2.LastModifiedAt).IsEquivalentTo(dbLabel2.LastModifiedAt);

            List<Edge<LabelDto>> edges = response.Data.Labels.Edges.ToList();
            await Assert.That(edges.Count).IsEqualTo(2);

            LabelDto edgeLabel = edges.Single(entity => entity.Node.Id.Equals(dbLabel.Id)).Node;
            await Assert.That(edgeLabel.Id).IsEqualTo(dbLabel.Id);
            await Assert.That(edgeLabel.Title).IsEqualTo(dbLabel.Title);
            await Assert.That(edgeLabel.Description).IsEqualTo(dbLabel.Description);
            await Assert.That(edgeLabel.Color).IsEqualTo(dbLabel.Color);
            await Assert.That(edgeLabel.TextColor).IsEqualTo(dbLabel.TextColor);
            await Assert.That(edgeLabel.CreatedAt).IsEquivalentTo(dbLabel.CreatedAt);
            await Assert.That(edgeLabel.LastModifiedAt).IsEquivalentTo(dbLabel.LastModifiedAt);

            LabelDto edgeLabel2 = edges.Single(entity => entity.Node.Id.Equals(dbLabel2.Id)).Node;
            await Assert.That(edgeLabel2.Id).IsEqualTo(dbLabel2.Id);
            await Assert.That(edgeLabel2.Title).IsEqualTo(dbLabel2.Title);
            await Assert.That(edgeLabel2.Description).IsEqualTo(dbLabel2.Description);
            await Assert.That(edgeLabel2.Color).IsEqualTo(dbLabel2.Color);
            await Assert.That(edgeLabel2.TextColor).IsEqualTo(dbLabel2.TextColor);
            await Assert.That(edgeLabel2.CreatedAt).IsEquivalentTo(dbLabel2.CreatedAt);
            await Assert.That(edgeLabel2.LastModifiedAt).IsEquivalentTo(dbLabel2.LastModifiedAt);
        }
    }

    [Test]
    public async Task GetLabelShouldReturnNullIfLabelWithGivenIdDoesNotExist()
    {
        // Arrange
        await SeedDatabaseAsync(context =>
        {
            context.Labels.Add(new Label
            {
                Id = new Guid("5888CDB6-57E2-4774-B6E8-7AABE82E2A5F"),
                Color = "001122",
                TextColor = "223344",
                Title = "Label 1",
                Description = "Description 1"
            });
        });
        const string id = "F1378377-9846-4168-A595-E763CD61CD9F";
        GraphQLRequest request = CreateGetLabelRequest(id);

        // Act
        GraphQLResponse<GetLabelResponse> response = await CreateGraphQLClient().SendQueryAsync<GetLabelResponse>(request);

        // Assert
        using (Assert.Multiple())
        {
            await Assert.That(response).IsNotNull();
            await Assert.That(response.Errors).IsNull();
            // response.Errors!.First().Extensions!["message"].Should().Be($"No Label found with id '{id}'.", StringComparer.InvariantCultureIgnoreCase);
            await Assert.That(response.Data).IsNotNull();
            await Assert.That(response.Data.Label).IsNull();
        }
    }

    [Test]
    public async Task GetLabelShouldReturnNullIfLabelsAreEmpty()
    {
        // Arrange
        await CheckDbContentAsync(async context =>
        {
            await Assert.That(context.Labels).IsEmpty();
        });
        const string id = "F1378377-9846-4168-A595-E763CD61CD9F";
        GraphQLRequest request = CreateGetLabelRequest(id);

        // Act
        GraphQLResponse<GetLabelResponse> response = await CreateGraphQLClient().SendQueryAsync<GetLabelResponse>(request);

        // Assert
        using (Assert.Multiple())
        {
            await Assert.That(response).IsNotNull();
            await Assert.That(response.Errors).IsNull();
            // var firstError = response.Errors!.First();
            // firstError.Extensions.Should().NotBeNull();
            // firstError.Extensions!.GetValueOrDefault("message").Should().Be($"No Label found with id '{id}'.",
            //     StringComparer.InvariantCultureIgnoreCase);
            await Assert.That(response.Data).IsNotNull();
            await Assert.That(response.Data.Label).IsNull();
        }
    }

    [Test]
    public async Task GetLabelShouldReturnLabelWithGivenId()
    {
        // Arrange
        const string id = "F1378377-9846-4168-A595-E763CD61CD9F";
        Issue issue = new Issue
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
        Label dbLabel = new Label
        {
            Id = new Guid(id),
            Color = "112233",
            TextColor = "334455",
            Title = "Label 2",
            Description = "Description 2",
            Issues = [issue]
        };
        await SeedDatabaseAsync(context =>
        {
            context.Issues.Add(issue);
            context.Labels.Add(new Label
            {
                Id = new Guid("5888CDB6-57E2-4774-B6E8-7AABE82E2A5F"),
                Color = "001122",
                TextColor = "223344",
                Title = "Label 1",
                Description = "Description 1"
            });
            context.Labels.Add(dbLabel);
        });
        GraphQLRequest request = CreateGetLabelRequest(id);

        // Act
        GraphQLResponse<GetLabelResponse> response = await CreateGraphQLClient().SendQueryAsync<GetLabelResponse>(request);

        // Assert
        using (Assert.Multiple())
        {
            await Assert.That(response).IsNotNull();
            await Assert.That(response.Errors).IsNull();
            await Assert.That(response.Data).IsNotNull();
            LabelDto? label = response.Data.Label;
            await Assert.That(label).IsNotNull();
            await Assert.That(label!.Id).IsEqualTo(dbLabel.Id);
            await Assert.That(label.Title).IsEqualTo(dbLabel.Title);
            await Assert.That(label.Description).IsEqualTo(dbLabel.Description);
            await Assert.That(label.Color).IsEqualTo(dbLabel.Color);
            await Assert.That(label.TextColor).IsEqualTo(dbLabel.TextColor);
            await Assert.That(label.Issues.ToList()).Contains(i => i.Id.Equals(issue.Id));
            await Assert.That(label.CreatedAt).IsEquivalentTo(dbLabel.CreatedAt);
            await Assert.That(label.LastModifiedAt).IsEquivalentTo(dbLabel.LastModifiedAt);
        }
    }

    private static GraphQLRequest CreateGetLabelsRequest()
    {
        GraphQLRequest queryRequest = new GraphQLRequest
        {
            Query = """
                    query labels
                    {
                        labels(first: 5)
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
                                    issues {
                                        id
                                        title
                                    }
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
                                issues {
                                    id
                                    title
                                }
                            }
                        }
                    }
                    """,
            OperationName = "labels"
        };
        return queryRequest;
    }

    private static GraphQLRequest CreateGetLabelRequest(string id)
    {
        GraphQLRequest queryRequest = new GraphQLRequest
        {
            Query = $$"""
                    query label
                    {
                        label(id: "{{id}}")
                        {
                            id
                            title
                            description
                            color
                            textColor
                            createdAt
                            lastModifiedAt
                            issues {
                                id
                                title
                            }
                        }
                    }
                    """,
            OperationName = "label"
        };
        return queryRequest;
    }
}
