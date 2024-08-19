using FluentAssertions;
using FluentAssertions.Execution;
using GraphQL;
using StarWarsProgressBarIssueTracker.App.Queries;
using StarWarsProgressBarIssueTracker.App.Tests.Helpers.GraphQL.Payloads.Labels;
using StarWarsProgressBarIssueTracker.Common.Tests;
using StarWarsProgressBarIssueTracker.Domain.Issues;
using StarWarsProgressBarIssueTracker.Domain.Releases;
using StarWarsProgressBarIssueTracker.Infrastructure.Models;

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
        var request = CreateGetLabelsRequest();

        // Act
        var response = await GraphQLClient.SendQueryAsync<GetLabelsResponse>(request);

        // Assert
        using (new AssertionScope())
        {
            response.Should().NotBeNull();
            response.Errors.Should().BeNullOrEmpty();
            response.Data.Should().NotBeNull();
            response.Data.Labels.Should().BeEmpty();
        }
    }

    [Test]
    public async Task GetLabelsShouldReturnAllLabels()
    {
        // Arrange
        var dbLabel = new DbLabel
        {
            Color = "001122",
            TextColor = "223344",
            Title = "Label 1",
            Description = "Description 1"
        };
        var dbLabel2 = new DbLabel
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
        var request = CreateGetLabelsRequest();

        // Act
        var response = await GraphQLClient.SendQueryAsync<GetLabelsResponse>(request);

        // Assert
        using (new AssertionScope())
        {
            response.Should().NotBeNull();
            response.Errors.Should().BeNullOrEmpty();
            response.Data.Should().NotBeNull();
            var labels = response.Data.Labels.ToList();
            labels.Count.Should().Be(2);

            var label = labels.Single(entity => entity.Id.Equals(dbLabel.Id));
            label.Id.Should().Be(dbLabel.Id);
            label.Title.Should().Be(dbLabel.Title);
            label.Description.Should().Be(dbLabel.Description);
            label.Color.Should().Be(dbLabel.Color);
            label.TextColor.Should().Be(dbLabel.TextColor);
            label.CreatedAt.Should().BeCloseTo(dbLabel.CreatedAt, TimeSpan.FromMilliseconds(300));
            label.LastModifiedAt.Should().Be(dbLabel.LastModifiedAt);

            var label2 = labels.Single(entity => entity.Id.Equals(dbLabel2.Id));
            label2.Id.Should().Be(dbLabel2.Id);
            label2.Title.Should().Be(dbLabel2.Title);
            label2.Description.Should().Be(dbLabel2.Description);
            label2.Color.Should().Be(dbLabel2.Color);
            label2.TextColor.Should().Be(dbLabel2.TextColor);
            label2.CreatedAt.Should().BeCloseTo(dbLabel2.CreatedAt, TimeSpan.FromMilliseconds(300));
            label2.LastModifiedAt.Should().Be(dbLabel2.LastModifiedAt);
        }
    }

    [Test]
    public async Task GetLabelShouldReturnNullIfLabelWithGivenIdDoesNotExist()
    {
        // Arrange
        await SeedDatabaseAsync(context =>
        {
            context.Labels.Add(new DbLabel
            {
                Id = new Guid("5888CDB6-57E2-4774-B6E8-7AABE82E2A5F"),
                Color = "001122",
                TextColor = "223344",
                Title = "Label 1",
                Description = "Description 1"
            });
        });
        const string id = "F1378377-9846-4168-A595-E763CD61CD9F";
        var request = CreateGetLabelRequest(id);

        // Act
        var response = await GraphQLClient.SendQueryAsync<GetLabelResponse>(request);

        // Assert
        using (new AssertionScope())
        {
            response.Should().NotBeNull();
            response.Errors.Should().BeNull();
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
        var request = CreateGetLabelRequest(id);

        // Act
        var response = await GraphQLClient.SendQueryAsync<GetLabelResponse>(request);

        // Assert
        using (new AssertionScope())
        {
            response.Should().NotBeNull();
            response.Errors.Should().BeNull();
            response.Data.Should().NotBeNull();
            response.Data.Label.Should().BeNull();
        }
    }

    [Test]
    public async Task GetLabelShouldReturnLabelWithGivenId()
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
        var dbLabel = new DbLabel
        {
            Id = new Guid(id),
            Color = "112233",
            TextColor = "334455",
            Title = "Label 2",
            Description = "Description 2",
            Issues = [dbIssue]
        };
        await SeedDatabaseAsync(context =>
        {
            context.Issues.Add(dbIssue);
            context.Labels.Add(new DbLabel
            {
                Id = new Guid("5888CDB6-57E2-4774-B6E8-7AABE82E2A5F"),
                Color = "001122",
                TextColor = "223344",
                Title = "Label 1",
                Description = "Description 1"
            });
            context.Labels.Add(dbLabel);
        });
        var request = CreateGetLabelRequest(id);

        // Act
        var response = await GraphQLClient.SendQueryAsync<GetLabelResponse>(request);

        // Assert
        using (new AssertionScope())
        {
            response.Should().NotBeNull();
            response.Errors.Should().BeNull();
            response.Data.Should().NotBeNull();
            var label = response.Data.Label;

            label.Should().NotBeNull();
            label!.Id.Should().Be(dbLabel.Id);
            label.Title.Should().Be(dbLabel.Title);
            label.Description.Should().Be(dbLabel.Description);
            label.Color.Should().Be(dbLabel.Color);
            label.TextColor.Should().Be(dbLabel.TextColor);
            label.Issues.Should().Contain(i => i.Id.Equals(dbIssue.Id));
            label.CreatedAt.Should().BeCloseTo(dbLabel.CreatedAt, TimeSpan.FromMilliseconds(300));
            label.LastModifiedAt.Should().Be(dbLabel.LastModifiedAt);
        }
    }

    private static GraphQLRequest CreateGetLabelsRequest()
    {
        var queryRequest = new GraphQLRequest
        {
            Query = """
                    query labels
                    {
                        labels()
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
            OperationName = "labels"
        };
        return queryRequest;
    }

    private static GraphQLRequest CreateGetLabelRequest(string id)
    {
        var queryRequest = new GraphQLRequest
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
