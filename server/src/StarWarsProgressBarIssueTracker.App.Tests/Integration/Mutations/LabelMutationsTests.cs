using Bogus;
using FluentAssertions;
using FluentAssertions.Execution;
using GraphQL;
using Microsoft.EntityFrameworkCore;
using StarWarsProgressBarIssueTracker.App.Labels;
using StarWarsProgressBarIssueTracker.App.Mutations;
using StarWarsProgressBarIssueTracker.App.Tests.Helpers.GraphQL.Payloads.Labels;
using StarWarsProgressBarIssueTracker.Domain.Issues;
using StarWarsProgressBarIssueTracker.Domain.Labels;
using StarWarsProgressBarIssueTracker.TestHelpers;

namespace StarWarsProgressBarIssueTracker.App.Tests.Integration.Mutations;

[TestFixture(TestOf = typeof(IssueTrackerMutations))]
[Category(TestCategory.Integration)]
public class LabelMutationsTests : IntegrationTestBase
{
    private const string AllowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ß_#%";
    private const string HexCodeColorChars = "0123456789abcdef";

    [TestCaseSource(nameof(AddLabelCases))]
    public async Task AddLabelShouldAddLabel(Label expectedLabel)
    {
        // Arrange
        CheckDbContent(context =>
        {
            context.Labels.Should().BeEmpty();
        });
        GraphQLRequest mutationRequest = CreateAddRequest(expectedLabel);

        DateTime startTime = DateTime.UtcNow;

        // Act
        GraphQLResponse<AddLabelResponse> response = await GraphQLClient.SendMutationAsync<AddLabelResponse>(mutationRequest);

        // Assert
        AssertAddedLabel(response, expectedLabel, startTime);
    }

    [TestCaseSource(nameof(AddLabelCases))]
    public async Task AddLabelShouldAddLabelIfLabelsAreNotEmpty(Label expectedLabel)
    {
        // Arrange
        Label dbLabel = new Label
        {
            Id = new Guid("87653DC5-B029-4BA6-959A-1FBFC48E2C81"),
            Title = "Title",
            Description = "Desc",
            Color = "#001122",
            TextColor = "#334455",
            LastModifiedAt = DateTime.UtcNow.AddDays(1)
        };
        await SeedDatabaseAsync(context =>
        {
            context.Labels.Add(dbLabel);
        });
        CheckDbContent(context =>
        {
            context.Labels.Should().ContainEquivalentOf(dbLabel);
        });
        GraphQLRequest mutationRequest = CreateAddRequest(expectedLabel);

        DateTime startTime = DateTime.UtcNow;

        // Act
        GraphQLResponse<AddLabelResponse> response = await GraphQLClient.SendMutationAsync<AddLabelResponse>(mutationRequest);

        // Assert
        AssertAddedLabel(response, expectedLabel, startTime, dbLabel);
    }

    [TestCaseSource(nameof(InvalidAddLabelCases))]
    public async Task AddLabelShouldNotAddLabel((Label expectedLabel, IEnumerable<string> errors) expectedResult)
    {
        // Arrange
        CheckDbContent(context =>
        {
            context.Labels.Should().BeEmpty();
        });
        GraphQLRequest mutationRequest = CreateAddRequest(expectedResult.expectedLabel);

        // Act
        GraphQLResponse<AddLabelResponse> response = await GraphQLClient.SendMutationAsync<AddLabelResponse>(mutationRequest);

        // Assert
        AssertLabelNotAdded(response, expectedResult.errors);
    }

    [TestCaseSource(nameof(AddLabelCases))]
    public async Task UpdateLabelShouldUpdateLabel(Label expectedLabel)
    {
        // Arrange
        Label dbLabel = new Label
        {
            Id = new Guid("87653DC5-B029-4BA6-959A-1FBFC48E2C81"),
            Title = "Title",
            Description = "Desc",
            Color = "#001122",
            TextColor = "#334455",
            LastModifiedAt = DateTime.UtcNow.AddDays(1)
        };
        await SeedDatabaseAsync(context =>
        {
            context.Labels.Add(dbLabel);
        });
        expectedLabel.Id = dbLabel.Id;
        expectedLabel.CreatedAt = dbLabel.CreatedAt;
        CheckDbContent(context =>
        {
            context.Labels.Should().ContainEquivalentOf(dbLabel);
        });
        GraphQLRequest mutationRequest = CreateUpdateRequest(expectedLabel);

        DateTime startTime = DateTime.UtcNow;

        // Act
        GraphQLResponse<UpdateLabelResponse> response = await GraphQLClient.SendMutationAsync<UpdateLabelResponse>(mutationRequest);

        // Assert
        AssertUpdatedLabel(response, expectedLabel, startTime);
    }

    [TestCaseSource(nameof(AddLabelCases))]
    public async Task UpdateLabelShouldUpdateLabelWithIssues(Label expectedLabel)
    {
        // Arrange
        Label dbLabel = new Label
        {
            Id = new Guid("87653DC5-B029-4BA6-959A-1FBFC48E2C81"),
            Title = "Title",
            Description = "Desc",
            Color = "#001122",
            TextColor = "#334455",
            LastModifiedAt = DateTime.UtcNow.AddDays(1)
        };
        Issue dbIssue = new Issue { Title = "IssueTitle" };
        dbLabel.Issues.Add(dbIssue);
        await SeedDatabaseAsync(context =>
        {
            context.Issues.Add(dbIssue);
            context.Labels.Add(dbLabel);
        });
        expectedLabel.Id = dbLabel.Id;
        expectedLabel.CreatedAt = dbLabel.CreatedAt;
        CheckDbContent(context =>
        {
            List<Label> labels = context.Labels.Include(label => label.Issues).ToList();
            labels.Should().ContainEquivalentOf(dbLabel, config => config.Excluding(label => label.Issues));
            labels.First().Issues.Should()
                .ContainEquivalentOf(dbIssue, config => config.Excluding(issue => issue.Labels));
        });
        GraphQLRequest mutationRequest = CreateUpdateRequest(expectedLabel);

        DateTime startTime = DateTime.UtcNow;

        // Act
        GraphQLResponse<UpdateLabelResponse> response = await GraphQLClient.SendMutationAsync<UpdateLabelResponse>(mutationRequest);

        // Assert
        AssertUpdatedLabel(response, expectedLabel, startTime, emptyIssues: false);
        CheckDbContent(context =>
        {
            Label resultLabel = context.Labels.Include(label => label.Issues).First(label => label.Id == dbLabel.Id);
            resultLabel.Issues.Should().NotBeEmpty();
            resultLabel.Issues.Should().ContainEquivalentOf(dbIssue, config => config.Excluding(issue => issue.Labels));
        });
    }

    [TestCaseSource(nameof(AddLabelCases))]
    public async Task UpdateLabelShouldUpdateLabelIfLabelsAreNotEmpty(Label expectedLabel)
    {
        // Arrange
        Label dbLabel = new Label
        {
            Id = new Guid("87653DC5-B029-4BA6-959A-1FBFC48E2C81"),
            Title = "Title",
            Description = "Desc",
            Color = "#001122",
            TextColor = "#334455",
            LastModifiedAt = DateTime.UtcNow.AddDays(1)
        };
        Label dbLabel2 = new Label
        {
            Id = new Guid("0609F93C-CBCC-4650-BA4C-B8D5FF93A877"),
            Title = "Title 2",
            Description = "Desc 2",
            Color = "#221100",
            TextColor = "#554433",
            LastModifiedAt = DateTime.UtcNow.AddDays(2)
        };


        await SeedDatabaseAsync(context =>
        {
            context.Labels.Add(dbLabel);
            context.Labels.Add(dbLabel2);
        });
        expectedLabel.Id = dbLabel.Id;
        expectedLabel.CreatedAt = dbLabel.CreatedAt;
        CheckDbContent(context =>
        {
            context.Labels.Should().ContainEquivalentOf(dbLabel);
            context.Labels.Should().ContainEquivalentOf(dbLabel2);
        });
        GraphQLRequest mutationRequest = CreateUpdateRequest(expectedLabel);

        DateTime startTime = DateTime.UtcNow;

        // Act
        GraphQLResponse<UpdateLabelResponse> response = await GraphQLClient.SendMutationAsync<UpdateLabelResponse>(mutationRequest);

        // Assert
        AssertUpdatedLabel(response, expectedLabel, startTime, dbLabel, dbLabel2);
    }

    [TestCaseSource(nameof(InvalidAddLabelCases))]
    public async Task UpdateLabelShouldNotUpdateLabel((Label expectedLabel, IEnumerable<string> errors) expectedResult)
    {
        // Arrange
        CheckDbContent(context =>
        {
            context.Labels.Should().BeEmpty();
        });
        GraphQLRequest mutationRequest = CreateUpdateRequest(expectedResult.expectedLabel);

        // Act
        GraphQLResponse<UpdateLabelResponse> response = await GraphQLClient.SendMutationAsync<UpdateLabelResponse>(mutationRequest);

        // Assert
        AssertLabelNotUpdated(response, expectedResult.errors);
    }

    [Test]
    public async Task UpdateLabelShouldNotUpdateLabelIfLabelDoesNotExist()
    {
        // Arrange
        Label label = CreateLabel();
        CheckDbContent(context =>
        {
            context.Labels.Should().BeEmpty();
        });
        GraphQLRequest mutationRequest = CreateUpdateRequest(label);

        // Act
        GraphQLResponse<UpdateLabelResponse> response = await GraphQLClient.SendMutationAsync<UpdateLabelResponse>(mutationRequest);

        // Assert
        AssertLabelNotUpdated(response, new List<string> { $"No {nameof(Label)} found with id '{label.Id}'." });
    }

    [Test]
    public async Task DeleteLabelShouldDeleteLabel()
    {
        // Arrange
        Label label = CreateLabel();
        Label dbLabel = new Label
        {
            Id = label.Id,
            Title = label.Title,
            Description = label.Description,
            Color = label.Color,
            TextColor = label.TextColor,
            LastModifiedAt = DateTime.UtcNow.AddDays(1)
        };
        await SeedDatabaseAsync(context =>
        {
            context.Labels.Add(dbLabel);
        });
        label.CreatedAt = dbLabel.CreatedAt;
        label.LastModifiedAt = dbLabel.LastModifiedAt;
        CheckDbContent(context =>
        {
            context.Labels.Should().ContainEquivalentOf(dbLabel);
        });
        GraphQLRequest mutationRequest = CreateDeleteRequest(label);

        // Act
        GraphQLResponse<DeleteLabelResponse> response = await GraphQLClient.SendMutationAsync<DeleteLabelResponse>(mutationRequest);

        // Assert
        AssertDeletedLabel(response, label);
    }

    [Test]
    public async Task DeleteLabelShouldDeleteLabelAndReferenceToIssues()
    {
        // Arrange
        Label label = CreateLabel();
        Label dbLabel = new Label
        {
            Id = label.Id,
            Title = label.Title,
            Description = label.Description,
            Color = label.Color,
            TextColor = label.TextColor,
            LastModifiedAt = DateTime.UtcNow.AddDays(1)
        };
        Label dbLabel2 = new Label
        {
            Id = new Guid("B961A621-9848-429A-8B44-B1AF1F0182CE"),
            Color = "#778899",
            TextColor = "#665544",
            Title = "Title 2"
        };
        Issue dbIssue2 = new Issue
        {
            Id = new Guid("74AE8DD4-7669-4428-8E81-FB8A24A217A3"),
            Title = "Issue2",
            Labels =
            [
                dbLabel,
                dbLabel2
            ]
        };
        dbLabel.Issues.Add(dbIssue2);
        dbLabel2.Issues.Add(dbIssue2);
        Issue dbIssue = new Issue
        {
            Id = new Guid("87A2F9BF-CAB7-41D3-84F9-155135FA41D7"),
            Title = "Issue",
            Labels = [dbLabel]
        };
        await SeedDatabaseAsync(context =>
        {
            context.Labels.Add(dbLabel);
            context.Labels.Add(dbLabel2);
            context.Issues.Add(dbIssue);
            context.Issues.Add(dbIssue2);
        });
        label.CreatedAt = dbLabel.CreatedAt;
        label.LastModifiedAt = dbLabel.LastModifiedAt;
        CheckDbContent(context =>
        {
            context.Labels.Should().ContainEquivalentOf(dbLabel, config => config.Excluding(l => l.Issues));
        });
        GraphQLRequest mutationRequest = CreateDeleteRequest(label);

        // Act
        GraphQLResponse<DeleteLabelResponse> response = await GraphQLClient.SendMutationAsync<DeleteLabelResponse>(mutationRequest);

        // Assert
        AssertDeletedLabel(response, label);
        CheckDbContent(context =>
        {
            List<Issue> dbIssues = context.Issues.Include(entity => entity.Labels).ToList();
            dbIssues.Should().Contain(i => i.Id.Equals(dbIssue.Id));
            dbIssues.Should().Contain(i => i.Id.Equals(dbIssue2.Id));
            foreach (Issue entity in dbIssues)
            {
                dbIssue.Labels.Should().NotContain(l => l.Id.Equals(entity.Id));
            }

            dbIssues.First(entity => entity.Id.Equals(dbIssue2.Id)).Labels.Should().ContainEquivalentOf(dbLabel2, config => config.Excluding(l => l.Issues));
        });
    }

    [Test]
    public async Task DeleteLabelShouldDeleteLabelIfLabelsIsNotEmpty()
    {
        // Arrange
        Label label = CreateLabel();
        Label dbLabel = new Label
        {
            Id = label.Id,
            Title = label.Title,
            Description = label.Description,
            Color = label.Color,
            TextColor = label.TextColor,
            LastModifiedAt = DateTime.UtcNow.AddDays(1)
        };
        Label dbLabel2 = new Label
        {
            Id = new Guid("0609F93C-CBCC-4650-BA4C-B8D5FF93A877"),
            Title = "Title 2",
            Description = "Desc 2",
            Color = "#221100",
            TextColor = "#554433",
            LastModifiedAt = DateTime.UtcNow.AddDays(2)
        };


        await SeedDatabaseAsync(context =>
        {
            context.Labels.Add(dbLabel);
            context.Labels.Add(dbLabel2);
        });
        label.CreatedAt = dbLabel.CreatedAt;
        label.LastModifiedAt = dbLabel.LastModifiedAt;
        CheckDbContent(context =>
        {
            context.Labels.Should().ContainEquivalentOf(dbLabel);
            context.Labels.Should().ContainEquivalentOf(dbLabel2);
        });
        GraphQLRequest mutationRequest = CreateDeleteRequest(label);

        // Act
        GraphQLResponse<DeleteLabelResponse> response = await GraphQLClient.SendMutationAsync<DeleteLabelResponse>(mutationRequest);

        // Assert
        AssertDeletedLabel(response, label, dbLabel2);
    }

    [Test]
    public async Task DeleteLabelShouldNotDeleteLabelIfLabelDoesNotExist()
    {
        // Arrange
        Label label = CreateLabel();
        CheckDbContent(context =>
        {
            context.Labels.Should().BeEmpty();
        });
        GraphQLRequest mutationRequest = CreateDeleteRequest(label);

        // Act
        GraphQLResponse<DeleteLabelResponse> response = await GraphQLClient.SendMutationAsync<DeleteLabelResponse>(mutationRequest);

        // Assert
        AssertLabelNotDeleted(response, new List<string> { $"No {nameof(Label)} found with id '{label.Id}'." });
    }

    private static GraphQLRequest CreateAddRequest(Label expectedLabel)
    {
        string descriptionParameter = expectedLabel.Description != null
            ? $"""
               , description: "{expectedLabel.Description}"
               """
            : string.Empty;
        GraphQLRequest mutationRequest = new GraphQLRequest
        {
            Query = $$"""
                      mutation addLabel
                      {
                          addLabel(input: {title: "{{expectedLabel.Title}}", color: "{{expectedLabel.Color}}", textColor: "{{expectedLabel.TextColor}}"{{descriptionParameter}}})
                          {
                              label
                              {
                                  id
                                  title
                                  description
                                  color
                                  textColor
                                  createdAt
                                  lastModifiedAt
                              },
                              errors
                              {
                                  ... on Error
                                  {
                                      message
                                  }
                              }
                          }
                      }
                      """,
            OperationName = "addLabel"
        };
        return mutationRequest;
    }

    private void AssertAddedLabel(GraphQLResponse<AddLabelResponse> response, Label expectedLabel,
        DateTime startTime, Label? dbLabel = null)
    {
        DateTime endTime = DateTime.UtcNow;
        LabelDto? addedLabel;
        using (new AssertionScope())
        {
            response.Should().NotBeNull();
            response.Errors.Should().BeNullOrEmpty();
            addedLabel = response.Data.AddLabel.Label;
            addedLabel.Id.Should().NotBeEmpty();
            addedLabel.Title.Should().Be(expectedLabel.Title);
            addedLabel.Description.Should().Be(expectedLabel.Description);
            addedLabel.Color.Should().Be(expectedLabel.Color);
            addedLabel.TextColor.Should().Be(expectedLabel.TextColor);
            addedLabel.CreatedAt.Should().BeCloseTo(startTime, TimeSpan.FromSeconds(1), "Start time").And
                .BeCloseTo(endTime, TimeSpan.FromSeconds(1), "End time");
            addedLabel.LastModifiedAt.Should().BeNull();
        }

        CheckDbContent(context =>
        {
            using (new AssertionScope())
            {
                if (dbLabel is not null)
                {
                    context.Labels.Any(dbLabel1 => dbLabel1.Id.Equals(dbLabel.Id)).Should().BeTrue();
                }
                Label addedDbLabel = context.Labels.First(dbLabel1 => dbLabel1.Id.Equals(addedLabel.Id));
                addedDbLabel.Should().NotBeNull();
                addedDbLabel.Id.Should().NotBeEmpty().And.Be(addedLabel.Id);
                addedDbLabel.Title.Should().Be(expectedLabel.Title);
                addedDbLabel.Description.Should().Be(expectedLabel.Description);
                addedDbLabel.Color.Should().Be(expectedLabel.Color);
                addedDbLabel.TextColor.Should().Be(expectedLabel.TextColor);
                addedDbLabel.CreatedAt.Should().BeCloseTo(startTime, TimeSpan.FromSeconds(1), "Start time").And
                    .BeCloseTo(endTime, TimeSpan.FromSeconds(1), "End time");
                addedDbLabel.LastModifiedAt.Should().BeNull();
            }
        });
    }

    private void AssertLabelNotAdded(GraphQLResponse<AddLabelResponse> response, IEnumerable<string> errors)
    {
        using (new AssertionScope())
        {
            response.Should().NotBeNull();
            response.Data.AddLabel.Errors.Should().NotBeNullOrEmpty();
            response.Data.AddLabel.Label.Should().BeNull();

            IEnumerable<string> resultErrors = response.Data.AddLabel.Errors.Select(error => error.Message);
            resultErrors.Should().BeEquivalentTo(errors);
        }

        CheckDbContent(context =>
        {
            context.Labels.Should().BeEmpty();
        });
    }

    private static GraphQLRequest CreateUpdateRequest(Label expectedLabel)
    {
        string descriptionParameter = expectedLabel.Description != null
            ? $"""
               , description: "{expectedLabel.Description}"
               """
            : string.Empty;
        GraphQLRequest mutationRequest = new GraphQLRequest
        {
            Query = $$"""
                      mutation updateLabel
                      {
                          updateLabel(input: {id: "{{expectedLabel.Id}}", title: "{{expectedLabel.Title}}", color: "{{expectedLabel.Color}}", textColor: "{{expectedLabel.TextColor}}"{{descriptionParameter}}})
                          {
                              label
                              {
                                  id
                                  title
                                  description
                                  color
                                  textColor
                                  createdAt
                                  lastModifiedAt
                              },
                              errors
                              {
                                  ... on Error
                                  {
                                      message
                                  }
                              }
                          }
                      }
                      """,
            OperationName = "updateLabel"
        };
        return mutationRequest;
    }

    private void AssertUpdatedLabel(GraphQLResponse<UpdateLabelResponse> response, Label expectedLabel,
        DateTime startTime, Label? dbLabel = null, Label? notUpdatedLabel = null, bool emptyIssues = true)
    {
        DateTime endTime = DateTime.UtcNow;
        LabelDto? updatedLabel;
        using (new AssertionScope())
        {
            response.Should().NotBeNull();
            response.Errors.Should().BeNullOrEmpty();
            updatedLabel = response.Data.UpdateLabel.Label;
            updatedLabel.Id.Should().Be(expectedLabel.Id);
            updatedLabel.Title.Should().Be(expectedLabel.Title);
            updatedLabel.Description.Should().Be(expectedLabel.Description);
            updatedLabel.Color.Should().Be(expectedLabel.Color);
            updatedLabel.TextColor.Should().Be(expectedLabel.TextColor);
            updatedLabel.CreatedAt.Should().BeCloseTo(expectedLabel.CreatedAt, TimeSpan.FromSeconds(1));
            updatedLabel.LastModifiedAt.Should().BeCloseTo(startTime, TimeSpan.FromSeconds(1), "Start time").And
                .BeCloseTo(endTime, TimeSpan.FromSeconds(1), "End time");
        }

        CheckDbContent(context =>
        {
            using (new AssertionScope())
            {
                if (dbLabel is not null)
                {
                    context.Labels.Any(dbLabel1 => dbLabel1.Id.Equals(dbLabel.Id)).Should().BeTrue();
                }
                Label updatedDbLabel = context.Labels.Include(label => label.Issues)
                    .First(dbLabel1 => dbLabel1.Id.Equals(updatedLabel.Id));
                updatedDbLabel.Should().NotBeNull();
                updatedDbLabel.Id.Should().NotBeEmpty().And.Be(updatedLabel.Id);
                updatedDbLabel.Title.Should().Be(expectedLabel.Title);
                updatedDbLabel.Description.Should().Be(expectedLabel.Description);
                updatedDbLabel.Color.Should().Be(expectedLabel.Color);
                updatedDbLabel.TextColor.Should().Be(expectedLabel.TextColor);
                updatedDbLabel.CreatedAt.Should().BeCloseTo(expectedLabel.CreatedAt, TimeSpan.FromSeconds(1));
                updatedDbLabel.LastModifiedAt.Should().BeCloseTo(startTime, TimeSpan.FromSeconds(1), "Start time").And
                    .BeCloseTo(endTime, TimeSpan.FromSeconds(1), "End time");
                if (emptyIssues)
                {
                    updatedDbLabel.Issues.Should().BeEmpty();
                }
                else
                {
                    updatedDbLabel.Issues.Should().NotBeEmpty();
                }

                if (notUpdatedLabel is not null)
                {
                    Label? secondLabel =
                        context.Labels.Include(label => label.Issues)
                            .FirstOrDefault(label => label.Id.Equals(notUpdatedLabel.Id));
                    secondLabel.Should().NotBeNull();
                    secondLabel!.Id.Should().NotBeEmpty().And.Be(notUpdatedLabel.Id);
                    secondLabel.Title.Should().Be(notUpdatedLabel.Title);
                    secondLabel.Description.Should().Be(notUpdatedLabel.Description);
                    secondLabel.Color.Should().Be(notUpdatedLabel.Color);
                    secondLabel.TextColor.Should().Be(notUpdatedLabel.TextColor);
                    secondLabel.CreatedAt.Should().BeCloseTo(notUpdatedLabel.CreatedAt, TimeSpan.FromSeconds(1));
                    secondLabel.LastModifiedAt.Should().BeCloseTo(notUpdatedLabel.LastModifiedAt!.Value, TimeSpan.FromSeconds(1));
                    if (emptyIssues)
                    {
                        secondLabel.Issues.Should().BeEmpty();
                    }
                    else
                    {
                        secondLabel.Issues.Should().NotBeEmpty();
                    }
                }
            }
        });
    }

    private void AssertLabelNotUpdated(GraphQLResponse<UpdateLabelResponse> response, IEnumerable<string> errors)
    {
        using (new AssertionScope())
        {
            response.Should().NotBeNull();
            response.Data.UpdateLabel.Errors.Should().NotBeNullOrEmpty();
            response.Data.UpdateLabel.Label.Should().BeNull();

            IEnumerable<string> resultErrors = response.Data.UpdateLabel.Errors.Select(error => error.Message);
            resultErrors.Should().BeEquivalentTo(errors);
        }

        CheckDbContent(context =>
        {
            context.Labels.Should().BeEmpty();
        });
    }

    private static GraphQLRequest CreateDeleteRequest(Label expectedLabel)
    {
        GraphQLRequest mutationRequest = new GraphQLRequest
        {
            Query = $$"""
                      mutation deleteLabel
                      {
                          deleteLabel(input: {id: "{{expectedLabel.Id}}"})
                          {
                              label
                              {
                                  id
                                  title
                                  description
                                  color
                                  textColor
                                  createdAt
                                  lastModifiedAt
                              },
                              errors
                              {
                                  ... on Error
                                  {
                                      message
                                  }
                              }
                          }
                      }
                      """,
            OperationName = "deleteLabel"
        };
        return mutationRequest;
    }

    private void AssertDeletedLabel(GraphQLResponse<DeleteLabelResponse> response, Label expectedLabel, Label? dbLabel = null)
    {
        using (new AssertionScope())
        {
            response.Should().NotBeNull();
            response.Errors.Should().BeNullOrEmpty();
            LabelDto deletedLabel = response.Data.DeleteLabel.Label;
            deletedLabel.Id.Should().NotBeEmpty();
            deletedLabel.Title.Should().Be(expectedLabel.Title);
            deletedLabel.Description.Should().Be(expectedLabel.Description);
            deletedLabel.Color.Should().Be(expectedLabel.Color);
            deletedLabel.TextColor.Should().Be(expectedLabel.TextColor);
            deletedLabel.CreatedAt.Should().BeCloseTo(expectedLabel.CreatedAt, TimeSpan.FromSeconds(1));
            deletedLabel.LastModifiedAt.Should().BeCloseTo(expectedLabel.LastModifiedAt!.Value, TimeSpan.FromSeconds(1));
        }

        CheckDbContent(context =>
        {
            using (new AssertionScope())
            {
                context.Labels.Any(dbLabel1 => dbLabel1.Id.Equals(expectedLabel.Id)).Should().BeFalse();

                if (dbLabel is not null)
                {
                    context.Labels.Any(dbLabel1 => dbLabel1.Id.Equals(dbLabel.Id)).Should().BeTrue();
                }
            }
        });
    }

    private void AssertLabelNotDeleted(GraphQLResponse<DeleteLabelResponse> response, IEnumerable<string> errors)
    {
        using (new AssertionScope())
        {
            response.Should().NotBeNull();
            response.Data.DeleteLabel.Errors.Should().NotBeNullOrEmpty();
            response.Data.DeleteLabel.Label.Should().BeNull();

            IEnumerable<string> resultErrors = response.Data.DeleteLabel.Errors.Select(error => error.Message);
            resultErrors.Should().BeEquivalentTo(errors);
        }

        CheckDbContent(context =>
        {
            context.Labels.Should().BeEmpty();
        });
    }

    private static Label CreateLabel()
    {
        Faker<Label>? faker = new Faker<Label>()
            .RuleFor(label => label.Id, f => f.Random.Guid())
            .RuleFor(label => label.Title, f => f.Random.String2(1, 50, AllowedChars))
            .RuleFor(label => label.Description, f => f.Random.String2(0, 255, AllowedChars).OrNull(f, 0.3f))
            .RuleFor(label => label.Color, f => "#" + f.Random.String2(6, 6, HexCodeColorChars))
            .RuleFor(label => label.TextColor, f => "#" + f.Random.String2(6, 6, HexCodeColorChars));
        return faker.Generate();
    }

    private static IEnumerable<Label> AddLabelCases()
    {
        Faker<Label>? faker = new Faker<Label>()
            .RuleFor(label => label.Title, f => f.Random.String2(1, 50, AllowedChars))
            .RuleFor(label => label.Description, f => f.Random.String2(0, 255, AllowedChars).OrNull(f, 0.3f))
            .RuleFor(label => label.Color, f => "#" + f.Random.String2(6, 6, HexCodeColorChars))
            .RuleFor(label => label.TextColor, f => "#" + f.Random.String2(6, 6, HexCodeColorChars));
        return faker.Generate(20);
    }

    private static IEnumerable<(Label, IEnumerable<string>)> InvalidAddLabelCases()
    {
        yield return (new Label { Title = null!, Description = null, Color = "#001122", TextColor = "#334455" }, new List<string> { $"The value for {nameof(Label.Title)} is not set.", $"The value '' for {nameof(Label.Title)} is too short. The length of {nameof(Label.Title)} has to be between 1 and 50." });
        yield return (new Label { Title = "", Description = null, Color = "#001122", TextColor = "#334455" }, new List<string> { $"The value for {nameof(Label.Title)} is not set.", $"The value '' for {nameof(Label.Title)} is too short. The length of {nameof(Label.Title)} has to be between 1 and 50." });
        yield return (new Label { Title = "  \t ", Description = null, Color = "#001122", TextColor = "#334455" }, new List<string> { $"The value for {nameof(Label.Title)} is not set." });
        yield return (new Label { Title = new string('a', 51), Description = null, Color = "#001122", TextColor = "#334455" }, new List<string> { $"The value 'aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa' for {nameof(Label.Title)} is long short. The length of {nameof(Label.Title)} has to be between 1 and 50." });
        yield return (new Label { Title = "Valid", Description = new string('a', 256), Color = "#001122", TextColor = "#334455" }, new List<string> { $"The value 'aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa' for {nameof(Label.Description)} is long short. The length of {nameof(Label.Description)} has to be less than 256." });
        yield return (new Label { Title = "Valid", Description = null, Color = null!, TextColor = "#334455" }, new List<string> { $"The value for {nameof(Label.Color)} is not set.", $"The value '' for field {nameof(Label.Color)} has a wrong format. Only colors in RGB hex format with 6 digits are allowed." });
        yield return (new Label { Title = "Valid", Description = null, Color = "01122", TextColor = "#334455" }, new List<string> { $"The value '01122' for field {nameof(Label.Color)} has a wrong format. Only colors in RGB hex format with 6 digits are allowed." });
        yield return (new Label { Title = "Valid", Description = null, Color = "001122", TextColor = "#334455" }, new List<string> { $"The value '001122' for field {nameof(Label.Color)} has a wrong format. Only colors in RGB hex format with 6 digits are allowed." });
        yield return (new Label { Title = "Valid", Description = null, Color = "#01122", TextColor = "#334455" }, new List<string> { $"The value '#01122' for field {nameof(Label.Color)} has a wrong format. Only colors in RGB hex format with 6 digits are allowed." });
        yield return (new Label { Title = "Valid", Description = null, Color = "", TextColor = "#334455" }, new List<string> { $"The value for {nameof(Label.Color)} is not set.", $"The value '' for field {nameof(Label.Color)} has a wrong format. Only colors in RGB hex format with 6 digits are allowed." });
        yield return (new Label { Title = "Valid", Description = null, Color = " ", TextColor = "#334455" }, new List<string> { $"The value for {nameof(Label.Color)} is not set.", $"The value ' ' for field {nameof(Label.Color)} has a wrong format. Only colors in RGB hex format with 6 digits are allowed." });
        yield return (new Label { Title = "Valid", Description = null, Color = "g", TextColor = "#334455" }, new List<string> { $"The value 'g' for field {nameof(Label.Color)} has a wrong format. Only colors in RGB hex format with 6 digits are allowed." });
        yield return (new Label { Title = "Valid", Description = null, TextColor = null!, Color = "#334455" }, new List<string> { $"The value for {nameof(Label.TextColor)} is not set.", $"The value '' for field {nameof(Label.TextColor)} has a wrong format. Only colors in RGB hex format with 6 digits are allowed." });
        yield return (new Label { Title = "Valid", Description = null, TextColor = "01122", Color = "#334455" }, new List<string> { $"The value '01122' for field {nameof(Label.TextColor)} has a wrong format. Only colors in RGB hex format with 6 digits are allowed." });
        yield return (new Label { Title = "Valid", Description = null, TextColor = "001122", Color = "#334455" }, new List<string> { $"The value '001122' for field {nameof(Label.TextColor)} has a wrong format. Only colors in RGB hex format with 6 digits are allowed." });
        yield return (new Label { Title = "Valid", Description = null, TextColor = "#01122", Color = "#334455" }, new List<string> { $"The value '#01122' for field {nameof(Label.TextColor)} has a wrong format. Only colors in RGB hex format with 6 digits are allowed." });
        yield return (new Label { Title = "Valid", Description = null, TextColor = "", Color = "#334455" }, new List<string> { $"The value for {nameof(Label.TextColor)} is not set.", $"The value '' for field {nameof(Label.TextColor)} has a wrong format. Only colors in RGB hex format with 6 digits are allowed." });
        yield return (new Label { Title = "Valid", Description = null, TextColor = " ", Color = "#334455" }, new List<string> { $"The value for {nameof(Label.TextColor)} is not set.", $"The value ' ' for field {nameof(Label.TextColor)} has a wrong format. Only colors in RGB hex format with 6 digits are allowed." });
        yield return (new Label { Title = "Valid", Description = null, TextColor = "g", Color = "#334455" }, new List<string> { $"The value 'g' for field {nameof(Label.TextColor)} has a wrong format. Only colors in RGB hex format with 6 digits are allowed." });
    }
}
