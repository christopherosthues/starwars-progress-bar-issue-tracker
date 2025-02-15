using Bogus;
using GraphQL;
using Microsoft.EntityFrameworkCore;
using StarWarsProgressBarIssueTracker.App.Tests.Helpers;
using StarWarsProgressBarIssueTracker.App.Tests.Helpers.GraphQL.Payloads.Milestones;
using StarWarsProgressBarIssueTracker.Domain.Issues;
using StarWarsProgressBarIssueTracker.Domain.Milestones;
using StarWarsProgressBarIssueTracker.TestHelpers;

namespace StarWarsProgressBarIssueTracker.App.Tests.Integration.Mutations;

[Category(TestCategory.Integration)]
[NotInParallel(NotInParallelTests.MilestoneMutation)]
public class MilestoneMutationsTests : IntegrationTestBase
{
    // TODO: Check DoesNotContain and Contains
    private const string AllowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ß_#%";

    [Test]
    [MethodDataSource(nameof(AddMilestoneCases))]
    public async Task AddMilestoneShouldAddMilestone(Milestone expectedMilestone)
    {
        // Arrange
        await CheckDbContentAsync(async context =>
        {
            await Assert.That(context.Milestones).IsEmpty();
        });
        GraphQLRequest mutationRequest = CreateAddRequest(expectedMilestone);
        expectedMilestone.State = MilestoneState.Open;

        DateTime startTime = DateTime.UtcNow;

        // Act
        GraphQLResponse<AddMilestoneResponse> response = await CreateGraphQLClient().SendMutationAsync<AddMilestoneResponse>(mutationRequest);

        // Assert
        await AssertAddedMilestoneAsync(response, expectedMilestone, startTime);
    }

    [Test]
    [MethodDataSource(nameof(AddMilestoneCases))]
    public async Task AddMilestoneShouldAddMilestoneIfMilestonesAreNotEmpty(Milestone expectedMilestone)
    {
        // Arrange
        Milestone dbMilestone = new Milestone
        {
            Id = new Guid("87653DC5-B029-4BA6-959A-1FBFC48E2C81"),
            Title = "Title",
            Description = "Desc",
            State = MilestoneState.Open,
            LastModifiedAt = DateTime.UtcNow.AddDays(1)
        };
        await SeedDatabaseAsync(context =>
        {
            context.Milestones.Add(dbMilestone);
        });
        await CheckDbContentAsync(async context =>
        {
            await Assert.That(context.Milestones).Contains(dbMilestone);
        });
        GraphQLRequest mutationRequest = CreateAddRequest(expectedMilestone);

        DateTime startTime = DateTime.UtcNow;

        // Act
        GraphQLResponse<AddMilestoneResponse> response = await CreateGraphQLClient().SendMutationAsync<AddMilestoneResponse>(mutationRequest);

        // Assert
        await AssertAddedMilestoneAsync(response, expectedMilestone, startTime, dbMilestone);
    }

    [Test]
    [MethodDataSource(nameof(InvalidAddMilestoneCases))]
    public async Task AddMilestoneShouldNotAddMilestone((Milestone expectedMilestone, IEnumerable<string> errors) expectedResult)
    {
        // Arrange
        await CheckDbContentAsync(async context =>
        {
            await Assert.That(context.Milestones).IsEmpty();
        });
        GraphQLRequest mutationRequest = CreateAddRequest(expectedResult.expectedMilestone);

        // Act
        GraphQLResponse<AddMilestoneResponse> response = await CreateGraphQLClient().SendMutationAsync<AddMilestoneResponse>(mutationRequest);

        // Assert
        await AssertMilestoneNotAddedAsync(response, expectedResult.errors);
    }

    [Test]
    [MethodDataSource(nameof(AddMilestoneCases))]
    public async Task UpdateMilestoneShouldUpdateMilestone(Milestone expectedMilestone)
    {
        // Arrange
        Milestone dbMilestone = new Milestone
        {
            Id = new Guid("87653DC5-B029-4BA6-959A-1FBFC48E2C81"),
            Title = "Title",
            Description = "Desc",
            State = MilestoneState.Open,
            LastModifiedAt = DateTime.UtcNow.AddDays(1)
        };
        await SeedDatabaseAsync(context =>
        {
            context.Milestones.Add(dbMilestone);
        });
        expectedMilestone.Id = dbMilestone.Id;
        expectedMilestone.CreatedAt = dbMilestone.CreatedAt;
        await CheckDbContentAsync(async context =>
        {
            await Assert.That(context.Milestones).Contains(dbMilestone);
        });
        GraphQLRequest mutationRequest = CreateUpdateRequest(expectedMilestone);

        DateTime startTime = DateTime.UtcNow;

        // Act
        GraphQLResponse<UpdateMilestoneResponse> response = await CreateGraphQLClient().SendMutationAsync<UpdateMilestoneResponse>(mutationRequest);

        // Assert
        await AssertUpdatedMilestoneAsync(response, expectedMilestone, startTime);
    }

    [Test]
    [MethodDataSource(nameof(AddMilestoneCases))]
    public async Task UpdateMilestoneShouldUpdateMilestoneIfMilestonesAreNotEmpty(Milestone expectedMilestone)
    {
        // Arrange
        Milestone dbMilestone = new Milestone
        {
            Id = new Guid("87653DC5-B029-4BA6-959A-1FBFC48E2C81"),
            Title = "Title",
            Description = "Desc",
            State = MilestoneState.Open,
            LastModifiedAt = DateTime.UtcNow.AddDays(1)
        };
        Milestone dbMilestone2 = new Milestone
        {
            Id = new Guid("0609F93C-CBCC-4650-BA4C-B8D5FF93A877"),
            Title = "Title 2",
            Description = "Desc 2",
            State = MilestoneState.Closed,
            LastModifiedAt = DateTime.UtcNow.AddDays(2)
        };


        await SeedDatabaseAsync(context =>
        {
            context.Milestones.Add(dbMilestone);
            context.Milestones.Add(dbMilestone2);
        });
        expectedMilestone.Id = dbMilestone.Id;
        expectedMilestone.CreatedAt = dbMilestone.CreatedAt;
        await CheckDbContentAsync(async context =>
        {
            using (Assert.Multiple())
            {
                await Assert.That(context.Milestones).Contains(dbMilestone);
                await Assert.That(context.Milestones).Contains(dbMilestone2);
            }
        });
        GraphQLRequest mutationRequest = CreateUpdateRequest(expectedMilestone);

        DateTime startTime = DateTime.UtcNow;

        // Act
        GraphQLResponse<UpdateMilestoneResponse> response = await CreateGraphQLClient().SendMutationAsync<UpdateMilestoneResponse>(mutationRequest);

        // Assert
        await AssertUpdatedMilestoneAsync(response, expectedMilestone, startTime, dbMilestone, dbMilestone2);
    }

    [Test]
    [MethodDataSource(nameof(InvalidUpdateMilestoneCases))]
    public async Task UpdateMilestoneShouldNotUpdateMilestone((Milestone expectedMilestone, IEnumerable<string> errors) expectedResult)
    {
        // Arrange
        await CheckDbContentAsync(async context =>
        {
            await Assert.That(context.Milestones).IsEmpty();
        });
        GraphQLRequest mutationRequest = CreateUpdateRequest(expectedResult.expectedMilestone);

        // Act
        GraphQLResponse<UpdateMilestoneResponse> response = await CreateGraphQLClient().SendMutationAsync<UpdateMilestoneResponse>(mutationRequest);

        // Assert
        await AssertMilestoneNotUpdatedAsync(response, expectedResult.errors);
    }

    [Test]
    public async Task UpdateMilestoneShouldNotUpdateMilestoneIfMilestoneDoesNotExist()
    {
        // Arrange
        Milestone milestone = CreateMilestone();
        await CheckDbContentAsync(async context =>
        {
            await Assert.That(context.Milestones).IsEmpty();
        });
        GraphQLRequest mutationRequest = CreateUpdateRequest(milestone);

        // Act
        GraphQLResponse<UpdateMilestoneResponse> response = await CreateGraphQLClient().SendMutationAsync<UpdateMilestoneResponse>(mutationRequest);

        // Assert
        await AssertMilestoneNotUpdatedAsync(response, new List<string> { $"No {nameof(Milestone)} found with id '{milestone.Id}'." });
    }

    [Test]
    public async Task DeleteMilestoneShouldDeleteMilestone()
    {
        // Arrange
        Milestone milestone = CreateMilestone();
        Milestone dbMilestone = new Milestone
        {
            Id = milestone.Id,
            Title = milestone.Title,
            Description = milestone.Description,
            State = milestone.State,
            LastModifiedAt = DateTime.UtcNow.AddDays(1)
        };
        await SeedDatabaseAsync(context =>
        {
            context.Milestones.Add(dbMilestone);
        });
        milestone.CreatedAt = dbMilestone.CreatedAt;
        milestone.LastModifiedAt = dbMilestone.LastModifiedAt;
        await CheckDbContentAsync(async context =>
        {
            await Assert.That(context.Milestones).Contains(dbMilestone);
        });
        GraphQLRequest mutationRequest = CreateDeleteRequest(milestone);

        // Act
        GraphQLResponse<DeleteMilestoneResponse> response = await CreateGraphQLClient().SendMutationAsync<DeleteMilestoneResponse>(mutationRequest);

        // Assert
        await AssertDeletedMilestoneAsync(response, milestone);
    }

    [Test]
    public async Task DeleteMilestoneShouldDeleteMilestoneAndReferenceToIssues()
    {
        // Arrange
        Milestone milestone = CreateMilestone();
        Milestone dbMilestone = new Milestone
        {
            Id = milestone.Id,
            Title = milestone.Title,
            Description = milestone.Description,
            State = milestone.State,
            LastModifiedAt = DateTime.UtcNow.AddDays(1)
        };
        Issue dbIssue = new Issue
        {
            Id = new Guid("87A2F9BF-CAB7-41D3-84F9-155135FA41D7"),
            Title = "IssueTitle",
            Milestone = dbMilestone
        };
        dbMilestone.Issues.Add(dbIssue);
        Milestone dbMilestone2 = new Milestone
        {
            Id = new Guid("B961A621-9848-429A-8B44-B1AF1F0182CE"),
            State = MilestoneState.Closed,
            Title = "Title 2"
        };
        Issue dbIssue2 = new Issue
        {
            Id = new Guid("74AE8DD4-7669-4428-8E81-FB8A24A217A3"),
            Title = "IssueTitle",
            Milestone = dbMilestone2
        };
        dbMilestone2.Issues.Add(dbIssue2);
        await SeedDatabaseAsync(context =>
        {
            context.Milestones.Add(dbMilestone);
            context.Milestones.Add(dbMilestone2);
            context.Issues.Add(dbIssue);
            context.Issues.Add(dbIssue2);
        });
        milestone.CreatedAt = dbMilestone.CreatedAt;
        milestone.LastModifiedAt = dbMilestone.LastModifiedAt;
        await CheckDbContentAsync(async context =>
        {
            await Assert.That(context.Milestones).Contains(dbMilestone);
        });
        GraphQLRequest mutationRequest = CreateDeleteRequest(milestone);

        // Act
        GraphQLResponse<DeleteMilestoneResponse> response = await CreateGraphQLClient().SendMutationAsync<DeleteMilestoneResponse>(mutationRequest);

        // Assert
        await AssertDeletedMilestoneAsync(response, milestone);
        await CheckDbContentAsync(async context =>
        {
            using (Assert.Multiple())
            {
                List<Issue> dbIssues = context.Issues.Include(dbEntity => dbEntity.Milestone).ToList();
                await Assert.That(dbIssues).Contains(i => i.Id.Equals(dbIssue.Id));
                await Assert.That(dbIssues).Contains(i => i.Id.Equals(dbIssue2.Id));

                Issue changedDbIssue = context.Issues.Include(dbEntity => dbEntity.Milestone).Single(dbEntity => dbEntity.Id.Equals(dbIssue.Id));
                await Assert.That(changedDbIssue.Milestone).IsNull();

                Issue unchangedDbIssue = context.Issues.Include(dbEntity => dbEntity.Milestone).Single(dbEntity => dbEntity.Id.Equals(dbIssue2.Id));
                await Assert.That(unchangedDbIssue.Milestone).IsNotNull();
                await Assert.That(unchangedDbIssue.Milestone!.Id).IsEqualTo(dbMilestone2.Id);
            }
        });
    }

    [Test]
    public async Task DeleteMilestoneShouldDeleteMilestoneIfMilestonesIsNotEmpty()
    {
        // Arrange
        Milestone milestone = CreateMilestone();
        Milestone dbMilestone = new Milestone
        {
            Id = milestone.Id,
            Title = milestone.Title,
            Description = milestone.Description,
            State = milestone.State,
            LastModifiedAt = DateTime.UtcNow.AddDays(1)
        };
        Milestone dbMilestone2 = new Milestone
        {
            Id = new Guid("0609F93C-CBCC-4650-BA4C-B8D5FF93A877"),
            Title = "Title 2",
            Description = "Desc 2",
            State = MilestoneState.Closed,
            LastModifiedAt = DateTime.UtcNow.AddDays(2)
        };


        await SeedDatabaseAsync(context =>
        {
            context.Milestones.Add(dbMilestone);
            context.Milestones.Add(dbMilestone2);
        });
        milestone.CreatedAt = dbMilestone.CreatedAt;
        milestone.LastModifiedAt = dbMilestone.LastModifiedAt;
        await CheckDbContentAsync(async context =>
        {
            using (Assert.Multiple())
            {
                await Assert.That(context.Milestones).Contains(dbMilestone);
                await Assert.That(context.Milestones).Contains(dbMilestone2);
            }
        });
        GraphQLRequest mutationRequest = CreateDeleteRequest(milestone);

        // Act
        GraphQLResponse<DeleteMilestoneResponse> response = await CreateGraphQLClient().SendMutationAsync<DeleteMilestoneResponse>(mutationRequest);

        // Assert
        await AssertDeletedMilestoneAsync(response, milestone, dbMilestone2);
    }

    [Test]
    public async Task DeleteMilestoneShouldNotDeleteMilestoneIfMilestoneDoesNotExist()
    {
        // Arrange
        Milestone milestone = CreateMilestone();
        await CheckDbContentAsync(async context =>
        {
            await Assert.That(context.Milestones).IsEmpty();
        });
        GraphQLRequest mutationRequest = CreateDeleteRequest(milestone);

        // Act
        GraphQLResponse<DeleteMilestoneResponse> response = await CreateGraphQLClient().SendMutationAsync<DeleteMilestoneResponse>(mutationRequest);

        // Assert
        await AssertMilestoneNotDeletedAsync(response, new List<string> { $"No {nameof(Milestone)} found with id '{milestone.Id}'." });
    }

    private static GraphQLRequest CreateAddRequest(Milestone expectedMilestone)
    {
        string descriptionParameter = expectedMilestone.Description != null
            ? $"""
               , description: "{expectedMilestone.Description}"
               """
            : string.Empty;
        GraphQLRequest mutationRequest = new GraphQLRequest
        {
            Query = $$"""
                      mutation addMilestone
                      {
                          addMilestone(input: {title: "{{expectedMilestone.Title}}"{{descriptionParameter}}})
                          {
                              milestone
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
                                  }
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
            OperationName = "addMilestone"
        };
        return mutationRequest;
    }

    private async Task AssertAddedMilestoneAsync(GraphQLResponse<AddMilestoneResponse> response, Milestone expectedMilestone,
        DateTime startTime, Milestone? dbMilestone = null)
    {
        DateTime endTime = DateTime.UtcNow;
        Milestone? addedMilestone;
        using (Assert.Multiple())
        {
            await Assert.That(response).IsNotNull();
            await Assert.That(response.Errors).IsNull().Or.IsEmpty();
            addedMilestone = response.Data.AddMilestone.Milestone;
            await Assert.That(addedMilestone.Id).IsNotDefault();
            await Assert.That(addedMilestone.Title).IsEqualTo(expectedMilestone.Title);
            await Assert.That(addedMilestone.Description).IsEqualTo(expectedMilestone.Description);
            await Assert.That(addedMilestone.State).IsEqualTo(expectedMilestone.State);
            await Assert.That(addedMilestone.Issues).IsEquivalentTo(expectedMilestone.Issues);
            await Assert.That(addedMilestone.CreatedAt).IsBetween(startTime, endTime).WithInclusiveBounds();
            await Assert.That(addedMilestone.LastModifiedAt).IsNull();
        }

        await CheckDbContentAsync(async context =>
        {
            using (Assert.Multiple())
            {
                if (dbMilestone is not null)
                {
                    await Assert.That(context.Milestones.ToList())
                        .Contains(dbMilestone1 => dbMilestone1.Id.Equals(dbMilestone.Id));
                }
                Milestone addedDbMilestone = context.Milestones.Include(dbMilestone2 => dbMilestone2.Issues)
                    .First(dbMilestone1 => dbMilestone1.Id.Equals(addedMilestone.Id));
                await Assert.That(addedDbMilestone).IsNotNull();
                await Assert.That(addedDbMilestone.Id).IsNotDefault().And.IsEqualTo(addedMilestone.Id);
                await Assert.That(addedDbMilestone.Title).IsEqualTo(expectedMilestone.Title);
                await Assert.That(addedDbMilestone.Description).IsEqualTo(expectedMilestone.Description);
                await Assert.That(addedDbMilestone.State).IsEqualTo(expectedMilestone.State);
                await Assert.That(addedDbMilestone.Issues).IsEquivalentTo(expectedMilestone.Issues);
                await Assert.That(addedDbMilestone.CreatedAt).IsBetween(startTime, endTime).WithInclusiveBounds();
                await Assert.That(addedDbMilestone.LastModifiedAt).IsNull();
            }
        });
    }

    private async Task AssertMilestoneNotAddedAsync(GraphQLResponse<AddMilestoneResponse> response, IEnumerable<string> errors)
    {
        using (Assert.Multiple())
        {
           await Assert.That(response).IsNotNull();
           await Assert.That(response.Data.AddMilestone.Errors).IsNotNull().And.IsNotEmpty();
           await Assert.That(response.Data.AddMilestone.Milestone).IsNull();

            IEnumerable<string> resultErrors = response.Data.AddMilestone.Errors.Select(error => error.Message);
            await Assert.That(resultErrors).IsEquivalentTo(errors);
        }

        await CheckDbContentAsync(async context =>
        {
            await Assert.That(context.Milestones).IsEmpty();
        });
    }

    private static GraphQLRequest CreateUpdateRequest(Milestone expectedMilestone)
    {
        string descriptionParameter = expectedMilestone.Description != null
            ? $"""
               , description: "{expectedMilestone.Description}"
               """
            : string.Empty;
        GraphQLRequest mutationRequest = new GraphQLRequest
        {
            Query = $$"""
                      mutation updateMilestone
                      {
                          updateMilestone(input: {id: "{{expectedMilestone.Id}}", title: "{{expectedMilestone.Title}}", state: {{expectedMilestone.State.ToString().ToUpper()}}{{descriptionParameter}}})
                          {
                              milestone
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
                                  }
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
            OperationName = "updateMilestone"
        };
        return mutationRequest;
    }

    private async Task AssertUpdatedMilestoneAsync(GraphQLResponse<UpdateMilestoneResponse> response, Milestone expectedMilestone,
        DateTime startTime, Milestone? dbMilestone = null, Milestone? notUpdatedMilestone = null, bool emptyIssues = true)
    {
        DateTime endTime = DateTime.UtcNow;
        Milestone? updatedMilestone;
        using (Assert.Multiple())
        {
            await Assert.That(response).IsNotNull();
            await Assert.That(response.Errors).IsNull().Or.IsEmpty();
            updatedMilestone = response.Data.UpdateMilestone.Milestone;
            await Assert.That(updatedMilestone.Id).IsEqualTo(expectedMilestone.Id);
            await Assert.That(updatedMilestone.Title).IsEqualTo(expectedMilestone.Title);
            await Assert.That(updatedMilestone.Description).IsEqualTo(expectedMilestone.Description);
            await Assert.That(updatedMilestone.State).IsEqualTo(expectedMilestone.State);
            await Assert.That(updatedMilestone.CreatedAt).IsEqualTo(expectedMilestone.CreatedAt);
            await Assert.That(updatedMilestone.LastModifiedAt!.Value).IsBetween(startTime, endTime).WithInclusiveBounds();
            if (emptyIssues)
            {
                await Assert.That(updatedMilestone.Issues).IsEmpty();
            }
            else
            {
                await Assert.That(updatedMilestone.Issues).IsNotEmpty();
            }
        }

        await CheckDbContentAsync(async context =>
        {
            using (Assert.Multiple())
            {
                if (dbMilestone is not null)
                {
                    await Assert.That(context.Milestones.ToList())
                        .Contains(dbMilestone1 => dbMilestone1.Id.Equals(dbMilestone.Id));
                }
                Milestone updatedDbMilestone = context.Milestones.Include(dbMilestone2 => dbMilestone2.Issues)
                    .First(dbMilestone1 => dbMilestone1.Id.Equals(updatedMilestone.Id));
                await Assert.That(updatedDbMilestone).IsNotNull();
                await Assert.That(updatedDbMilestone.Id).IsNotDefault().And.IsEqualTo(updatedMilestone.Id);
                await Assert.That(updatedDbMilestone.Title).IsEqualTo(expectedMilestone.Title);
                await Assert.That(updatedDbMilestone.Description).IsEqualTo(expectedMilestone.Description);
                await Assert.That(updatedDbMilestone.State).IsEqualTo(expectedMilestone.State);
                await Assert.That(updatedDbMilestone.CreatedAt).IsEqualTo(expectedMilestone.CreatedAt);
                await Assert.That(updatedDbMilestone.LastModifiedAt!.Value).IsBetween(startTime, endTime).WithInclusiveBounds();
                if (emptyIssues)
                {
                    await Assert.That(updatedDbMilestone.Issues).IsEmpty();
                }
                else
                {
                    await Assert.That(updatedDbMilestone.Issues).IsNotEmpty();
                }

                if (notUpdatedMilestone is not null)
                {
                    Milestone? secondMilestone =
                        context.Milestones.Include(dbMilestone2 => dbMilestone2.Issues)
                            .FirstOrDefault(milestone => milestone.Id.Equals(notUpdatedMilestone.Id));
                    await Assert.That(secondMilestone).IsNotNull();
                    await Assert.That(secondMilestone!.Id).IsNotDefault().And.IsEqualTo(notUpdatedMilestone.Id);
                    await Assert.That(secondMilestone.Title).IsEqualTo(notUpdatedMilestone.Title);
                    await Assert.That(secondMilestone.Description).IsEqualTo(notUpdatedMilestone.Description);
                    await Assert.That(secondMilestone.State).IsEqualTo(notUpdatedMilestone.State);
                    await Assert.That(secondMilestone.CreatedAt).IsEqualTo(notUpdatedMilestone.CreatedAt);
                    await Assert.That(secondMilestone.LastModifiedAt).IsEqualTo(notUpdatedMilestone.LastModifiedAt!.Value);
                    if (emptyIssues)
                    {
                        await Assert.That(secondMilestone.Issues).IsEmpty();
                    }
                    else
                    {
                        await Assert.That(secondMilestone.Issues).IsNotEmpty();
                    }
                }
            }
        });
    }

    private async Task AssertMilestoneNotUpdatedAsync(GraphQLResponse<UpdateMilestoneResponse> response, IEnumerable<string> errors)
    {
        using (Assert.Multiple())
        {
            await Assert.That(response).IsNotNull();
            await Assert.That(response.Data.UpdateMilestone.Errors).IsNotNull().And.IsNotEmpty();
            await Assert.That(response.Data.UpdateMilestone.Milestone).IsNull();

            IEnumerable<string> resultErrors = response.Data.UpdateMilestone.Errors.Select(error => error.Message);
            await Assert.That(resultErrors).IsEquivalentTo(errors);
        }

        await CheckDbContentAsync(async context =>
        {
            await Assert.That(context.Milestones).IsEmpty();
        });
    }

    private static GraphQLRequest CreateDeleteRequest(Milestone expectedMilestone)
    {
        GraphQLRequest mutationRequest = new GraphQLRequest
        {
            Query = $$"""
                      mutation deleteMilestone
                      {
                          deleteMilestone(input: {id: "{{expectedMilestone.Id}}"})
                          {
                              milestone
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
                                  }
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
            OperationName = "deleteMilestone"
        };
        return mutationRequest;
    }

    private async Task AssertDeletedMilestoneAsync(GraphQLResponse<DeleteMilestoneResponse> response, Milestone expectedMilestone, Milestone? dbMilestone = null)
    {
        using (Assert.Multiple())
        {
            await Assert.That(response).IsNotNull();
            await Assert.That(response.Errors).IsNull().Or.IsEmpty();
            Milestone deletedMilestone = response.Data.DeleteMilestone.Milestone;
            await Assert.That(deletedMilestone.Id).IsNotDefault();
            await Assert.That(deletedMilestone.Title).IsEqualTo(expectedMilestone.Title);
            await Assert.That(deletedMilestone.Description).IsEqualTo(expectedMilestone.Description);
            await Assert.That(deletedMilestone.State).IsEqualTo(expectedMilestone.State);
            await Assert.That(deletedMilestone.CreatedAt).IsEqualTo(expectedMilestone.CreatedAt);
            await Assert.That(deletedMilestone.LastModifiedAt).IsEqualTo(expectedMilestone.LastModifiedAt!.Value);
        }

        await CheckDbContentAsync(async context =>
        {
            using (Assert.Multiple())
            {
                await Assert.That(context.Milestones.ToList())
                    .DoesNotContain(dbMilestone1 => dbMilestone1.Id.Equals(expectedMilestone.Id));

                if (dbMilestone is not null)
                {
                    await Assert.That(context.Milestones.ToList())
                        .Contains(dbMilestone1 => dbMilestone1.Id.Equals(dbMilestone.Id));
                }
            }
        });
    }

    private async Task AssertMilestoneNotDeletedAsync(GraphQLResponse<DeleteMilestoneResponse> response, IEnumerable<string> errors)
    {
        using (Assert.Multiple())
        {
            await Assert.That(response).IsNotNull();
            await Assert.That(response.Data.DeleteMilestone.Errors).IsNotNull().And.IsNotEmpty();
            await Assert.That(response.Data.DeleteMilestone.Milestone).IsNull();

            IEnumerable<string> resultErrors = response.Data.DeleteMilestone.Errors.Select(error => error.Message);
            await Assert.That(resultErrors).IsEquivalentTo(errors);
        }

        await CheckDbContentAsync(async context =>
        {
            await Assert.That(context.Milestones).IsEmpty();
        });
    }

    private static Milestone CreateMilestone()
    {
        Faker<Milestone>? faker = new Faker<Milestone>()
            .RuleFor(milestone => milestone.Id, f => f.Random.Guid())
            .RuleFor(milestone => milestone.Title, f => f.Random.String2(1, 50, AllowedChars))
            .RuleFor(milestone => milestone.Description, f => f.Random.String2(0, 255, AllowedChars).OrNull(f, 0.3f))
            .RuleFor(milestone => milestone.State, _ => MilestoneState.Open);
        return faker.Generate();
    }

    public static IEnumerable<Func<Milestone>> AddMilestoneCases()
    {
        Faker<Milestone>? faker = new Faker<Milestone>()
            .RuleFor(milestone => milestone.Title, f => f.Random.String2(1, 50, AllowedChars))
            .RuleFor(milestone => milestone.Description, f => f.Random.String2(0, 255, AllowedChars).OrNull(f, 0.3f))
            .RuleFor(milestone => milestone.State, _ => MilestoneState.Open);
        List<Milestone>? milestones = faker.Generate(20);
        return milestones.Select<Milestone, Func<Milestone>>(milestone => () => milestone);
    }

    public static IEnumerable<Func<(Milestone, IEnumerable<string>)>> InvalidAddMilestoneCases()
    {
        yield return () => (new Milestone { Title = null!, Description = null, State = MilestoneState.Open }, new List<string> { $"The value for {nameof(Milestone.Title)} is not set.", $"The value '' for {nameof(Milestone.Title)} is too short. The length of {nameof(Milestone.Title)} has to be between 1 and 50." });
        yield return () => (new Milestone { Title = "", Description = null, State = MilestoneState.Open }, new List<string> { $"The value for {nameof(Milestone.Title)} is not set.", $"The value '' for {nameof(Milestone.Title)} is too short. The length of {nameof(Milestone.Title)} has to be between 1 and 50." });
        yield return () => (new Milestone { Title = "  \t ", Description = null, State = MilestoneState.Open }, new List<string> { $"The value for {nameof(Milestone.Title)} is not set." });
        yield return () => (new Milestone { Title = new string('a', 51), Description = null, State = MilestoneState.Open }, new List<string> { $"The value 'aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa' for {nameof(Milestone.Title)} is long short. The length of {nameof(Milestone.Title)} has to be between 1 and 50." });
        yield return () => (new Milestone { Title = "Valid", Description = new string('a', 256), State = MilestoneState.Open }, new List<string> { $"The value 'aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa' for {nameof(Milestone.Description)} is long short. The length of {nameof(Milestone.Description)} has to be less than 256." });
    }

    public static IEnumerable<Func<(Milestone, IEnumerable<string>)>> InvalidUpdateMilestoneCases()
    {
        yield return () => (new Milestone { Title = null!, Description = null, State = MilestoneState.Open }, new List<string> { $"The value for {nameof(Milestone.Title)} is not set.", $"The value '' for {nameof(Milestone.Title)} is too short. The length of {nameof(Milestone.Title)} has to be between 1 and 50." });
        yield return () => (new Milestone { Title = "", Description = null, State = MilestoneState.Open }, new List<string> { $"The value for {nameof(Milestone.Title)} is not set.", $"The value '' for {nameof(Milestone.Title)} is too short. The length of {nameof(Milestone.Title)} has to be between 1 and 50." });
        yield return () => (new Milestone { Title = "  \t ", Description = null, State = MilestoneState.Open }, new List<string> { $"The value for {nameof(Milestone.Title)} is not set." });
        yield return () => (new Milestone { Title = new string('a', 51), Description = null, State = MilestoneState.Open }, new List<string> { $"The value 'aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa' for {nameof(Milestone.Title)} is long short. The length of {nameof(Milestone.Title)} has to be between 1 and 50." });
        yield return () => (new Milestone { Title = "Valid", Description = new string('a', 256), State = MilestoneState.Open }, new List<string> { $"The value 'aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa' for {nameof(Milestone.Description)} is long short. The length of {nameof(Milestone.Description)} has to be less than 256." });
        yield return () => (new Milestone { Title = "Valid", Description = null, State = MilestoneState.Unknown }, new List<string> { $"The value for {nameof(Milestone.State)} is not set." });
    }
}
