using FluentAssertions;
using FluentAssertions.Execution;
using GraphQL;
using StarWarsProgressBarIssueTracker.App.Labels;
using StarWarsProgressBarIssueTracker.App.Queries;
using StarWarsProgressBarIssueTracker.App.Tests.Helpers.GraphQL.Payloads;
using StarWarsProgressBarIssueTracker.App.Tests.Helpers.GraphQL.Payloads.Labels;
using StarWarsProgressBarIssueTracker.Domain.Issues;
using StarWarsProgressBarIssueTracker.Domain.Labels;
using StarWarsProgressBarIssueTracker.Domain.Releases;
using StarWarsProgressBarIssueTracker.Domain.Vehicles;
using StarWarsProgressBarIssueTracker.TestHelpers;

namespace StarWarsProgressBarIssueTracker.App.Tests.Integration.Queries;

[TestFixture(TestOf = typeof(IssueTrackerQueries))]
[Category(TestCategory.Integration)]
public class LabelQueriesTests : IntegrationTestBase
{
    [Test]
    public async Task GetLabelsShouldReturnEmptyListIfNoLabelExist()
    {
        // Arrange
        CheckDbContent(context =>
        {
            context.Labels.Should().BeEmpty();
        });
        GraphQLRequest request = CreateGetLabelsRequest();

        // Act
        GraphQLResponse<GetLabelsResponse> response = await GraphQLClient.SendQueryAsync<GetLabelsResponse>(request);

        // Assert
        using (new AssertionScope())
        {
            response.Should().NotBeNull();
            response.Errors.Should().BeNullOrEmpty();
            response.Data.Should().NotBeNull();
            response.Data.Labels.TotalCount.Should().Be(0);
            response.Data.Labels.PageInfo.HasNextPage.Should().BeFalse();
            response.Data.Labels.PageInfo.HasPreviousPage.Should().BeFalse();
            response.Data.Labels.PageInfo.StartCursor.Should().BeNull();
            response.Data.Labels.PageInfo.EndCursor.Should().BeNull();
            response.Data.Labels.Edges.Should().BeEmpty();
            response.Data.Labels.Nodes.Should().BeEmpty();
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
        CheckDbContent(context =>
        {
            context.Labels.Should().ContainEquivalentOf(dbLabel);
            context.Labels.Should().ContainEquivalentOf(dbLabel2);
        });
        GraphQLRequest request = CreateGetLabelsRequest();

        // Act
        GraphQLResponse<GetLabelsResponse> response = await GraphQLClient.SendQueryAsync<GetLabelsResponse>(request);

        // Assert
        using (new AssertionScope())
        {
            response.Should().NotBeNull();
            response.Errors.Should().BeNullOrEmpty();
            response.Data.Should().NotBeNull();

            response.Data.Labels.TotalCount.Should().Be(2);
            response.Data.Labels.PageInfo.HasNextPage.Should().BeFalse();
            response.Data.Labels.PageInfo.HasPreviousPage.Should().BeFalse();
            response.Data.Labels.PageInfo.StartCursor.Should().NotBeNull();
            response.Data.Labels.PageInfo.EndCursor.Should().NotBeNull();
            List<LabelDto> labels = response.Data.Labels.Nodes.ToList();
            labels.Count.Should().Be(2);

            LabelDto label = labels.Single(entity => entity.Id.Equals(dbLabel.Id));
            label.Id.Should().Be(dbLabel.Id);
            label.Title.Should().Be(dbLabel.Title);
            label.Description.Should().Be(dbLabel.Description);
            label.Color.Should().Be(dbLabel.Color);
            label.TextColor.Should().Be(dbLabel.TextColor);
            label.CreatedAt.Should().BeCloseTo(dbLabel.CreatedAt, TimeSpan.FromMilliseconds(300));
            label.LastModifiedAt.Should().Be(dbLabel.LastModifiedAt);

            LabelDto label2 = labels.Single(entity => entity.Id.Equals(dbLabel2.Id));
            label2.Id.Should().Be(dbLabel2.Id);
            label2.Title.Should().Be(dbLabel2.Title);
            label2.Description.Should().Be(dbLabel2.Description);
            label2.Color.Should().Be(dbLabel2.Color);
            label2.TextColor.Should().Be(dbLabel2.TextColor);
            label2.CreatedAt.Should().BeCloseTo(dbLabel2.CreatedAt, TimeSpan.FromMilliseconds(300));
            label2.LastModifiedAt.Should().Be(dbLabel2.LastModifiedAt);

            List<Edge<LabelDto>> edges = response.Data.Labels.Edges.ToList();
            edges.Count.Should().Be(2);

            LabelDto edgeLabel = edges.Single(entity => entity.Node.Id.Equals(dbLabel.Id)).Node;
            edgeLabel.Id.Should().Be(dbLabel.Id);
            edgeLabel.Title.Should().Be(dbLabel.Title);
            edgeLabel.Description.Should().Be(dbLabel.Description);
            edgeLabel.Color.Should().Be(dbLabel.Color);
            edgeLabel.TextColor.Should().Be(dbLabel.TextColor);
            edgeLabel.CreatedAt.Should().BeCloseTo(dbLabel.CreatedAt, TimeSpan.FromMilliseconds(300));
            edgeLabel.LastModifiedAt.Should().Be(dbLabel.LastModifiedAt);

            LabelDto edgeLabel2 = edges.Single(entity => entity.Node.Id.Equals(dbLabel2.Id)).Node;
            edgeLabel2.Id.Should().Be(dbLabel2.Id);
            edgeLabel2.Title.Should().Be(dbLabel2.Title);
            edgeLabel2.Description.Should().Be(dbLabel2.Description);
            edgeLabel2.Color.Should().Be(dbLabel2.Color);
            edgeLabel2.TextColor.Should().Be(dbLabel2.TextColor);
            edgeLabel2.CreatedAt.Should().BeCloseTo(dbLabel2.CreatedAt, TimeSpan.FromMilliseconds(300));
            edgeLabel2.LastModifiedAt.Should().Be(dbLabel2.LastModifiedAt);
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
        GraphQLResponse<GetLabelResponse> response = await GraphQLClient.SendQueryAsync<GetLabelResponse>(request);

        // Assert
        using (new AssertionScope())
        {
            response.Should().NotBeNull();
            response.Errors.Should().BeNull();
            // response.Errors!.First().Extensions!["message"].Should().Be($"No Label found with id '{id}'.",
            //     StringComparer.InvariantCultureIgnoreCase);
            response.Data.Should().NotBeNull();
            response.Data.Label.Should().BeNull();
        }
    }

    [Test]
    public async Task GetLabelShouldReturnNullIfLabelsAreEmpty()
    {
        // Arrange
        CheckDbContent(context =>
        {
            context.Labels.Should().BeEmpty();
        });
        const string id = "F1378377-9846-4168-A595-E763CD61CD9F";
        GraphQLRequest request = CreateGetLabelRequest(id);

        // Act
        GraphQLResponse<GetLabelResponse> response = await GraphQLClient.SendQueryAsync<GetLabelResponse>(request);

        // Assert
        using (new AssertionScope())
        {
            response.Should().NotBeNull();
            response.Errors.Should().BeNull();
            // var firstError = response.Errors!.First();
            // firstError.Extensions.Should().NotBeNull();
            // firstError.Extensions!.GetValueOrDefault("message").Should().Be($"No Label found with id '{id}'.",
            //     StringComparer.InvariantCultureIgnoreCase);
            response.Data.Should().NotBeNull();
            response.Data.Label.Should().BeNull();
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
        GraphQLResponse<GetLabelResponse> response = await GraphQLClient.SendQueryAsync<GetLabelResponse>(request);

        // Assert
        using (new AssertionScope())
        {
            response.Should().NotBeNull();
            response.Errors.Should().BeNull();
            response.Data.Should().NotBeNull();
            LabelDto? label = response.Data.Label;

            label.Should().NotBeNull();
            label!.Id.Should().Be(dbLabel.Id);
            label.Title.Should().Be(dbLabel.Title);
            label.Description.Should().Be(dbLabel.Description);
            label.Color.Should().Be(dbLabel.Color);
            label.TextColor.Should().Be(dbLabel.TextColor);
            label.Issues.Should().Contain(i => i.Id.Equals(issue.Id));
            label.CreatedAt.Should().BeCloseTo(dbLabel.CreatedAt, TimeSpan.FromMilliseconds(300));
            label.LastModifiedAt.Should().Be(dbLabel.LastModifiedAt);
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
