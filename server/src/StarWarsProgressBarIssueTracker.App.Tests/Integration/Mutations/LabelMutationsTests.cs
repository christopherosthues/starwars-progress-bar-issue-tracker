using Bogus;
using GraphQL;
using Microsoft.EntityFrameworkCore;
using StarWarsProgressBarIssueTracker.App.Labels;
using StarWarsProgressBarIssueTracker.App.Tests.Helpers;
using StarWarsProgressBarIssueTracker.App.Tests.Helpers.GraphQL.Payloads.Labels;
using StarWarsProgressBarIssueTracker.Domain.Issues;
using StarWarsProgressBarIssueTracker.Domain.Labels;
using StarWarsProgressBarIssueTracker.TestHelpers;

namespace StarWarsProgressBarIssueTracker.App.Tests.Integration.Mutations;

[Category(TestCategory.Integration)]
[NotInParallel(NotInParallelTests.LabelMutation)]
public class LabelMutationsTests : IntegrationTestBase
{
    private const string AllowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ß_#%";
    private const string HexCodeColorChars = "0123456789abcdef";

    [Test]
    [MethodDataSource(nameof(AddLabelCases))]
    public async Task AddLabelShouldAddLabel(Label expectedLabel)
    {
        // Arrange
        await CheckDbContentAsync(async context =>
        {
            await Assert.That(context.Labels).IsEmpty();
        });
        GraphQLRequest mutationRequest = CreateAddRequest(expectedLabel);

        DateTime startTime = DateTime.UtcNow;

        // Act
        GraphQLResponse<AddLabelResponse> response = await CreateGraphQLClient().SendMutationAsync<AddLabelResponse>(mutationRequest);

        // Assert
        await AssertAddedLabelAsync(response, expectedLabel, startTime);
    }

    [Test]
    [MethodDataSource(nameof(AddLabelCases))]
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
        await CheckDbContentAsync(async context =>
        {
            await Assert.That(context.Labels).Contains(dbLabel);
        });
        GraphQLRequest mutationRequest = CreateAddRequest(expectedLabel);

        DateTime startTime = DateTime.UtcNow;

        // Act
        GraphQLResponse<AddLabelResponse> response = await CreateGraphQLClient().SendMutationAsync<AddLabelResponse>(mutationRequest);

        // Assert
        await AssertAddedLabelAsync(response, expectedLabel, startTime, dbLabel);
    }

    [Test]
    [MethodDataSource(nameof(InvalidAddLabelCases))]
    public async Task AddLabelShouldNotAddLabel((Label expectedLabel, IEnumerable<string> errors) expectedResult)
    {
        // Arrange
        await CheckDbContentAsync(async context =>
        {
            await Assert.That(context.Labels).IsEmpty();
        });
        GraphQLRequest mutationRequest = CreateAddRequest(expectedResult.expectedLabel);

        // Act
        GraphQLResponse<AddLabelResponse> response = await CreateGraphQLClient().SendMutationAsync<AddLabelResponse>(mutationRequest);

        // Assert
        await AssertLabelNotAddedAsync(response, expectedResult.errors);
    }

    [Test]
    [MethodDataSource(nameof(AddLabelCases))]
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
        await CheckDbContentAsync(async context =>
        {
            await Assert.That(context.Labels).Contains(dbLabel);
        });
        GraphQLRequest mutationRequest = CreateUpdateRequest(expectedLabel);

        DateTime startTime = DateTime.UtcNow;

        // Act
        GraphQLResponse<UpdateLabelResponse> response = await CreateGraphQLClient().SendMutationAsync<UpdateLabelResponse>(mutationRequest);

        // Assert
        await AssertUpdatedLabelAsync(response, expectedLabel, startTime);
    }

    [Test]
    [MethodDataSource(nameof(AddLabelCases))]
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
        await CheckDbContentAsync(async context =>
        {
            List<Label> labels = context.Labels.Include(label => label.Issues).ToList();
            await Assert.That(labels).Contains(dbLabel);
            await Assert.That(labels.First().Issues).Contains(dbIssue);
        });
        GraphQLRequest mutationRequest = CreateUpdateRequest(expectedLabel);

        DateTime startTime = DateTime.UtcNow;

        // Act
        GraphQLResponse<UpdateLabelResponse> response = await CreateGraphQLClient().SendMutationAsync<UpdateLabelResponse>(mutationRequest);

        // Assert
        await AssertUpdatedLabelAsync(response, expectedLabel, startTime, emptyIssues: false);
        await CheckDbContentAsync(async context =>
        {
            Label resultLabel = context.Labels.Include(label => label.Issues).First(label => label.Id == dbLabel.Id);
            await Assert.That(resultLabel.Issues).IsNotEmpty();
            await Assert.That(resultLabel.Issues).Contains(dbIssue);
        });
    }

    [Test]
    [MethodDataSource(nameof(AddLabelCases))]
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
        await CheckDbContentAsync(async context =>
        {
            await Assert.That(context.Labels).Contains(dbLabel);
            await Assert.That(context.Labels).Contains(dbLabel2);
        });
        GraphQLRequest mutationRequest = CreateUpdateRequest(expectedLabel);

        DateTime startTime = DateTime.UtcNow;

        // Act
        GraphQLResponse<UpdateLabelResponse> response = await CreateGraphQLClient().SendMutationAsync<UpdateLabelResponse>(mutationRequest);

        // Assert
        await AssertUpdatedLabelAsync(response, expectedLabel, startTime, dbLabel, dbLabel2);
    }

    [Test]
    [MethodDataSource(nameof(InvalidAddLabelCases))]
    public async Task UpdateLabelShouldNotUpdateLabel((Label expectedLabel, IEnumerable<string> errors) expectedResult)
    {
        // Arrange
        await CheckDbContentAsync(async context =>
        {
            await Assert.That(context.Labels).IsEmpty();
        });
        GraphQLRequest mutationRequest = CreateUpdateRequest(expectedResult.expectedLabel);

        // Act
        GraphQLResponse<UpdateLabelResponse> response = await CreateGraphQLClient().SendMutationAsync<UpdateLabelResponse>(mutationRequest);

        // Assert
        await AssertLabelNotUpdatedAsync(response, expectedResult.errors);
    }

    [Test]
    public async Task UpdateLabelShouldNotUpdateLabelIfLabelDoesNotExist()
    {
        // Arrange
        Label label = CreateLabel();
        await CheckDbContentAsync(async context =>
        {
            await Assert.That(context.Labels).IsEmpty();
        });
        GraphQLRequest mutationRequest = CreateUpdateRequest(label);

        // Act
        GraphQLResponse<UpdateLabelResponse> response = await CreateGraphQLClient().SendMutationAsync<UpdateLabelResponse>(mutationRequest);

        // Assert
        await AssertLabelNotUpdatedAsync(response, new List<string> { $"No {nameof(Label)} found with id '{label.Id}'." });
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
        await CheckDbContentAsync(async context =>
        {
            await Assert.That(context.Labels).Contains(dbLabel);
        });
        GraphQLRequest mutationRequest = CreateDeleteRequest(label);

        // Act
        GraphQLResponse<DeleteLabelResponse> response = await CreateGraphQLClient().SendMutationAsync<DeleteLabelResponse>(mutationRequest);

        // Assert
        await AssertDeletedLabelAsync(response, label);
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
        await CheckDbContentAsync(async context =>
        {
            await Assert.That(context.Labels).Contains(dbLabel);
        });
        GraphQLRequest mutationRequest = CreateDeleteRequest(label);

        // Act
        GraphQLResponse<DeleteLabelResponse> response = await CreateGraphQLClient().SendMutationAsync<DeleteLabelResponse>(mutationRequest);

        // Assert
        await AssertDeletedLabelAsync(response, label);
        await CheckDbContentAsync(async context =>
        {
            List<Issue> dbIssues = context.Issues.Include(entity => entity.Labels).ToList();
            using (Assert.Multiple())
            {
                await Assert.That(dbIssues).Contains(i => i.Id.Equals(dbIssue.Id));
                await Assert.That(dbIssues).Contains(i => i.Id.Equals(dbIssue2.Id));
                foreach (Issue entity in dbIssues)
                {
                    await Assert.That(dbIssue.Labels.ToList()).DoesNotContain(l => l.Id.Equals(entity.Id));
                }
                await Assert.That(dbIssues.First(entity => entity.Id.Equals(dbIssue2.Id)).Labels).Contains(dbLabel2);
            }
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
        await CheckDbContentAsync(async context =>
        {
            using (Assert.Multiple())
            {
                await Assert.That(context.Labels).Contains(dbLabel);
                await Assert.That(context.Labels).Contains(dbLabel2);
            }
        });
        GraphQLRequest mutationRequest = CreateDeleteRequest(label);

        // Act
        GraphQLResponse<DeleteLabelResponse> response = await CreateGraphQLClient().SendMutationAsync<DeleteLabelResponse>(mutationRequest);

        // Assert
        await AssertDeletedLabelAsync(response, label, dbLabel2);
    }

    [Test]
    public async Task DeleteLabelShouldNotDeleteLabelIfLabelDoesNotExist()
    {
        // Arrange
        Label label = CreateLabel();
        await CheckDbContentAsync(async context =>
        {
            await Assert.That(context.Labels).IsEmpty();
        });
        GraphQLRequest mutationRequest = CreateDeleteRequest(label);

        // Act
        GraphQLResponse<DeleteLabelResponse> response = await CreateGraphQLClient().SendMutationAsync<DeleteLabelResponse>(mutationRequest);

        // Assert
        await AssertLabelNotDeletedAsync(response, new List<string> { $"No {nameof(Label)} found with id '{label.Id}'." });
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

    private async Task AssertAddedLabelAsync(GraphQLResponse<AddLabelResponse> response, Label expectedLabel,
        DateTime startTime, Label? dbLabel = null)
    {
        DateTime endTime = DateTime.UtcNow;
        LabelDto? addedLabel;
        using (Assert.Multiple())
        {
            await Assert.That(response).IsNotNull();
            await Assert.That(response.Errors).IsNotNull().And.IsNotEmpty();
            addedLabel = response.Data.AddLabel.Label;
            await Assert.That(addedLabel.Id).IsNotDefault();
            await Assert.That(addedLabel.Title).IsEqualTo(expectedLabel.Title);
            await Assert.That(addedLabel.Description).IsEqualTo(expectedLabel.Description);
            await Assert.That(addedLabel.Color).IsEqualTo(expectedLabel.Color);
            await Assert.That(addedLabel.TextColor).IsEqualTo(expectedLabel.TextColor);
            await Assert.That(addedLabel.CreatedAt).IsGreaterThanOrEqualTo(startTime).And.IsLessThanOrEqualTo(endTime);
            await Assert.That(addedLabel.LastModifiedAt).IsNull();
        }

        await CheckDbContentAsync(async context =>
        {
            using (Assert.Multiple())
            {
                if (dbLabel is not null)
                {
                    await Assert.That(context.Labels.ToList()).Contains(dbLabel1 => dbLabel1.Id.Equals(dbLabel.Id));
                }
                Label addedDbLabel = context.Labels.First(dbLabel1 => dbLabel1.Id.Equals(addedLabel.Id));
                await Assert.That(addedLabel).IsNotNull();
                await Assert.That(addedLabel.Id).IsNotDefault().And.IsEqualTo(addedDbLabel.Id);
                await Assert.That(addedLabel.Title).IsEqualTo(expectedLabel.Title);
                await Assert.That(addedLabel.Description).IsEqualTo(expectedLabel.Description);
                await Assert.That(addedLabel.Color).IsEqualTo(expectedLabel.Color);
                await Assert.That(addedLabel.TextColor).IsEqualTo(expectedLabel.TextColor);
                await Assert.That(addedLabel.CreatedAt).IsGreaterThanOrEqualTo(startTime).And.IsLessThanOrEqualTo(endTime);
                await Assert.That(addedLabel.LastModifiedAt).IsNull();
            }
        });
    }

    private async Task AssertLabelNotAddedAsync(GraphQLResponse<AddLabelResponse> response, IEnumerable<string> errors)
    {
        using (Assert.Multiple())
        {
            await Assert.That(response).IsNotNull();
            await Assert.That(response.Data.AddLabel.Errors).IsNotNull().And.IsNotEmpty();
            await Assert.That(response.Data.AddLabel.Label).IsNotNull();

            IEnumerable<string> resultErrors = response.Data.AddLabel.Errors.Select(error => error.Message);
            await Assert.That(resultErrors).IsEquivalentTo(errors);
        }

        await CheckDbContentAsync(async context =>
        {
            await Assert.That(context.Labels).IsEmpty();
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

    private async Task AssertUpdatedLabelAsync(GraphQLResponse<UpdateLabelResponse> response, Label expectedLabel,
        DateTime startTime, Label? dbLabel = null, Label? notUpdatedLabel = null, bool emptyIssues = true)
    {
        DateTime endTime = DateTime.UtcNow;
        LabelDto? updatedLabel;
        using (Assert.Multiple())
        {
            await Assert.That(response).IsNotNull();
            await Assert.That(response.Errors).IsNotNull().And.IsNotEmpty();
            updatedLabel = response.Data.UpdateLabel.Label;
            await Assert.That(updatedLabel.Id).IsEqualTo(expectedLabel.Id);
            await Assert.That(updatedLabel.Title).IsEqualTo(expectedLabel.Title);
            await Assert.That(updatedLabel.Description).IsEqualTo(expectedLabel.Description);
            await Assert.That(updatedLabel.Color).IsEqualTo(expectedLabel.Color);
            await Assert.That(updatedLabel.TextColor).IsEqualTo(expectedLabel.TextColor);
            await Assert.That(updatedLabel.CreatedAt).IsEqualTo(expectedLabel.CreatedAt);
            await Assert.That(updatedLabel.LastModifiedAt!.Value).IsGreaterThanOrEqualTo(startTime).And.IsLessThanOrEqualTo(endTime);
        }

        await CheckDbContentAsync(async context =>
        {
            using (Assert.Multiple())
            {
                if (dbLabel is not null)
                {
                    await Assert.That(context.Labels.ToList()).Contains(dbLabel1 => dbLabel1.Id.Equals(dbLabel.Id));
                }
                Label updatedDbLabel = context.Labels.Include(label => label.Issues)
                    .First(dbLabel1 => dbLabel1.Id.Equals(updatedLabel.Id));
                await Assert.That(updatedDbLabel).IsNotNull();
                await Assert.That(updatedDbLabel.Id).IsEqualTo(updatedLabel.Id);
                await Assert.That(updatedDbLabel.Title).IsEqualTo(updatedLabel.Title);
                await Assert.That(updatedDbLabel.Description).IsEqualTo(updatedLabel.Description);
                await Assert.That(updatedDbLabel.Color).IsEqualTo(updatedLabel.Color);
                await Assert.That(updatedDbLabel.TextColor).IsEqualTo(updatedLabel.TextColor);
                await Assert.That(updatedDbLabel.CreatedAt).IsEqualTo(updatedLabel.CreatedAt);
                await Assert.That(updatedLabel.LastModifiedAt!.Value).IsGreaterThanOrEqualTo(startTime).And.IsLessThanOrEqualTo(endTime);
                if (emptyIssues)
                {
                    await Assert.That(context.Issues).IsEmpty();
                }
                else
                {
                    await Assert.That(context.Issues).IsNotEmpty();
                }

                if (notUpdatedLabel is not null)
                {
                    Label? secondLabel =
                        context.Labels.Include(label => label.Issues)
                            .FirstOrDefault(label => label.Id.Equals(notUpdatedLabel.Id));

                    await Assert.That(secondLabel).IsNotNull();
                    await Assert.That(secondLabel!.Id).IsEqualTo(notUpdatedLabel.Id);
                    await Assert.That(secondLabel.Title).IsEqualTo(notUpdatedLabel.Title);
                    await Assert.That(secondLabel.Description).IsEqualTo(notUpdatedLabel.Description);
                    await Assert.That(secondLabel.Color).IsEqualTo(notUpdatedLabel.Color);
                    await Assert.That(secondLabel.TextColor).IsEqualTo(notUpdatedLabel.TextColor);
                    await Assert.That(secondLabel.CreatedAt).IsEqualTo(notUpdatedLabel.CreatedAt);
                    await Assert.That(secondLabel.LastModifiedAt).IsEqualTo(notUpdatedLabel.LastModifiedAt);

                    if (emptyIssues)
                    {
                        await Assert.That(secondLabel.Issues).IsEmpty();
                    }
                    else
                    {
                        await Assert.That(secondLabel.Issues).IsNotEmpty();
                    }
                }
            }
        });
    }

    private async Task AssertLabelNotUpdatedAsync(GraphQLResponse<UpdateLabelResponse> response, IEnumerable<string> errors)
    {
        using (Assert.Multiple())
        {
            await Assert.That(response).IsNotNull();
            await Assert.That(response.Data.UpdateLabel.Errors).IsNotNull().And.IsNotEmpty();
            await Assert.That(response.Data.UpdateLabel.Label).IsNotNull();

            IEnumerable<string> resultErrors = response.Data.UpdateLabel.Errors.Select(error => error.Message);
            await Assert.That(resultErrors).IsEquivalentTo(errors);
        }

        await CheckDbContentAsync(async context =>
        {
            await Assert.That(context.Labels).IsNotEmpty();
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

    private async Task AssertDeletedLabelAsync(GraphQLResponse<DeleteLabelResponse> response, Label expectedLabel, Label? dbLabel = null)
    {
        using (Assert.Multiple())
        {
            await Assert.That(response).IsNotNull();
            await Assert.That(response.Errors).IsNotNull().And.IsNotEmpty();
            LabelDto deletedLabel = response.Data.DeleteLabel.Label;
            await Assert.That(deletedLabel.Id).IsNotDefault();
            await Assert.That(deletedLabel.Title).IsEqualTo(expectedLabel.Title);
            await Assert.That(deletedLabel.Description).IsEqualTo(expectedLabel.Description);
            await Assert.That(deletedLabel.Color).IsEqualTo(expectedLabel.Color);
            await Assert.That(deletedLabel.TextColor).IsEqualTo(expectedLabel.TextColor);
            await Assert.That(deletedLabel.CreatedAt).IsEqualTo(expectedLabel.CreatedAt);
            await Assert.That(deletedLabel.LastModifiedAt).IsEqualTo(expectedLabel.LastModifiedAt);
        }

        await CheckDbContentAsync(async context =>
        {
            using (Assert.Multiple())
            {
                await Assert.That(context.Labels.ToList()).DoesNotContain(dbLabel1 => dbLabel1.Id.Equals(expectedLabel.Id));

                if (dbLabel is not null)
                {
                    await Assert.That(context.Labels.ToList()).Contains(dbLabel1 => dbLabel1.Id.Equals(dbLabel.Id));
                }
            }
        });
    }

    private async Task AssertLabelNotDeletedAsync(GraphQLResponse<DeleteLabelResponse> response, IEnumerable<string> errors)
    {
        using (Assert.Multiple())
        {
            await Assert.That(response).IsNotNull();
            await Assert.That(response.Data.DeleteLabel.Errors).IsNotNull().And.IsNotEmpty();
            await Assert.That(response.Data.DeleteLabel.Label).IsNotNull();

            IEnumerable<string> resultErrors = response.Data.DeleteLabel.Errors.Select(error => error.Message);
            await Assert.That(resultErrors).IsEquivalentTo(errors);
        }

        await CheckDbContentAsync(async context =>
        {
            await Assert.That(context.Labels).IsEmpty();
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

    public static IEnumerable<Func<Label>> AddLabelCases()
    {
        Faker<Label>? faker = new Faker<Label>()
            .RuleFor(label => label.Title, f => f.Random.String2(1, 50, AllowedChars))
            .RuleFor(label => label.Description, f => f.Random.String2(0, 255, AllowedChars).OrNull(f, 0.3f))
            .RuleFor(label => label.Color, f => "#" + f.Random.String2(6, 6, HexCodeColorChars))
            .RuleFor(label => label.TextColor, f => "#" + f.Random.String2(6, 6, HexCodeColorChars));
        IEnumerable<Label> labels = faker.Generate(20);
        return labels.Select<Label, Func<Label>>(label => () => label);
    }

    public static IEnumerable<Func<(Label, IEnumerable<string>)>> InvalidAddLabelCases()
    {
        yield return () => (new Label { Title = null!, Description = null, Color = "#001122", TextColor = "#334455" }, new List<string> { $"The value for {nameof(Label.Title)} is not set.", $"The value '' for {nameof(Label.Title)} is too short. The length of {nameof(Label.Title)} has to be between 1 and 50." });
        yield return () => (new Label { Title = "", Description = null, Color = "#001122", TextColor = "#334455" }, new List<string> { $"The value for {nameof(Label.Title)} is not set.", $"The value '' for {nameof(Label.Title)} is too short. The length of {nameof(Label.Title)} has to be between 1 and 50." });
        yield return () => (new Label { Title = "  \t ", Description = null, Color = "#001122", TextColor = "#334455" }, new List<string> { $"The value for {nameof(Label.Title)} is not set." });
        yield return () => (new Label { Title = new string('a', 51), Description = null, Color = "#001122", TextColor = "#334455" }, new List<string> { $"The value 'aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa' for {nameof(Label.Title)} is long short. The length of {nameof(Label.Title)} has to be between 1 and 50." });
        yield return () => (new Label { Title = "Valid", Description = new string('a', 256), Color = "#001122", TextColor = "#334455" }, new List<string> { $"The value 'aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa' for {nameof(Label.Description)} is long short. The length of {nameof(Label.Description)} has to be less than 256." });
        yield return () => (new Label { Title = "Valid", Description = null, Color = null!, TextColor = "#334455" }, new List<string> { $"The value for {nameof(Label.Color)} is not set.", $"The value '' for field {nameof(Label.Color)} has a wrong format. Only colors in RGB hex format with 6 digits are allowed." });
        yield return () => (new Label { Title = "Valid", Description = null, Color = "01122", TextColor = "#334455" }, new List<string> { $"The value '01122' for field {nameof(Label.Color)} has a wrong format. Only colors in RGB hex format with 6 digits are allowed." });
        yield return () => (new Label { Title = "Valid", Description = null, Color = "001122", TextColor = "#334455" }, new List<string> { $"The value '001122' for field {nameof(Label.Color)} has a wrong format. Only colors in RGB hex format with 6 digits are allowed." });
        yield return () => (new Label { Title = "Valid", Description = null, Color = "#01122", TextColor = "#334455" }, new List<string> { $"The value '#01122' for field {nameof(Label.Color)} has a wrong format. Only colors in RGB hex format with 6 digits are allowed." });
        yield return () => (new Label { Title = "Valid", Description = null, Color = "", TextColor = "#334455" }, new List<string> { $"The value for {nameof(Label.Color)} is not set.", $"The value '' for field {nameof(Label.Color)} has a wrong format. Only colors in RGB hex format with 6 digits are allowed." });
        yield return () => (new Label { Title = "Valid", Description = null, Color = " ", TextColor = "#334455" }, new List<string> { $"The value for {nameof(Label.Color)} is not set.", $"The value ' ' for field {nameof(Label.Color)} has a wrong format. Only colors in RGB hex format with 6 digits are allowed." });
        yield return () => (new Label { Title = "Valid", Description = null, Color = "g", TextColor = "#334455" }, new List<string> { $"The value 'g' for field {nameof(Label.Color)} has a wrong format. Only colors in RGB hex format with 6 digits are allowed." });
        yield return () => (new Label { Title = "Valid", Description = null, TextColor = null!, Color = "#334455" }, new List<string> { $"The value for {nameof(Label.TextColor)} is not set.", $"The value '' for field {nameof(Label.TextColor)} has a wrong format. Only colors in RGB hex format with 6 digits are allowed." });
        yield return () => (new Label { Title = "Valid", Description = null, TextColor = "01122", Color = "#334455" }, new List<string> { $"The value '01122' for field {nameof(Label.TextColor)} has a wrong format. Only colors in RGB hex format with 6 digits are allowed." });
        yield return () => (new Label { Title = "Valid", Description = null, TextColor = "001122", Color = "#334455" }, new List<string> { $"The value '001122' for field {nameof(Label.TextColor)} has a wrong format. Only colors in RGB hex format with 6 digits are allowed." });
        yield return () => (new Label { Title = "Valid", Description = null, TextColor = "#01122", Color = "#334455" }, new List<string> { $"The value '#01122' for field {nameof(Label.TextColor)} has a wrong format. Only colors in RGB hex format with 6 digits are allowed." });
        yield return () => (new Label { Title = "Valid", Description = null, TextColor = "", Color = "#334455" }, new List<string> { $"The value for {nameof(Label.TextColor)} is not set.", $"The value '' for field {nameof(Label.TextColor)} has a wrong format. Only colors in RGB hex format with 6 digits are allowed." });
        yield return () => (new Label { Title = "Valid", Description = null, TextColor = " ", Color = "#334455" }, new List<string> { $"The value for {nameof(Label.TextColor)} is not set.", $"The value ' ' for field {nameof(Label.TextColor)} has a wrong format. Only colors in RGB hex format with 6 digits are allowed." });
        yield return () => (new Label { Title = "Valid", Description = null, TextColor = "g", Color = "#334455" }, new List<string> { $"The value 'g' for field {nameof(Label.TextColor)} has a wrong format. Only colors in RGB hex format with 6 digits are allowed." });
    }
}
