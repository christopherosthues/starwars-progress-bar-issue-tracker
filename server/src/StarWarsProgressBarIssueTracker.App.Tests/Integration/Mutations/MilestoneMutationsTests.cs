using Bogus;
using FluentAssertions;
using FluentAssertions.Execution;
using GraphQL;
using Microsoft.EntityFrameworkCore;
using StarWarsProgressBarIssueTracker.App.Mutations;
using StarWarsProgressBarIssueTracker.App.Tests.Helpers.GraphQL.Payloads.Milestones;
using StarWarsProgressBarIssueTracker.Domain.Issues;
using StarWarsProgressBarIssueTracker.Domain.Milestones;
using StarWarsProgressBarIssueTracker.TestHelpers;

namespace StarWarsProgressBarIssueTracker.App.Tests.Integration.Mutations;

[TestFixture(TestOf = typeof(IssueTrackerMutations))]
[Category(TestCategory.Integration)]
public class MilestoneMutationsTests : IntegrationTestBase
{
    private const string AllowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ß_#%";

    [TestCaseSource(nameof(AddMilestoneCases))]
    public async Task AddMilestoneShouldAddMilestone(Milestone expectedMilestone)
    {
        // Arrange
        CheckDbContentAsync(context =>
        {
            context.Milestones.Should().BeEmpty();
        });
        var mutationRequest = CreateAddRequest(expectedMilestone);
        expectedMilestone.State = MilestoneState.Open;

        var startTime = DateTime.UtcNow;

        // Act
        var response = await GraphQLClient.SendMutationAsync<AddMilestoneResponse>(mutationRequest);

        // Assert
        AssertAddedMilestone(response, expectedMilestone, startTime);
    }

    [TestCaseSource(nameof(AddMilestoneCases))]
    public async Task AddMilestoneShouldAddMilestoneIfMilestonesAreNotEmpty(Milestone expectedMilestone)
    {
        // Arrange
        var dbMilestone = new Milestone
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
        CheckDbContentAsync(context =>
        {
            context.Milestones.Should().ContainEquivalentOf(dbMilestone);
        });
        var mutationRequest = CreateAddRequest(expectedMilestone);

        var startTime = DateTime.UtcNow;

        // Act
        var response = await GraphQLClient.SendMutationAsync<AddMilestoneResponse>(mutationRequest);

        // Assert
        AssertAddedMilestone(response, expectedMilestone, startTime, dbMilestone);
    }

    [TestCaseSource(nameof(InvalidAddMilestoneCases))]
    public async Task AddMilestoneShouldNotAddMilestone((Milestone expectedMilestone, IEnumerable<string> errors) expectedResult)
    {
        // Arrange
        CheckDbContentAsync(context =>
        {
            context.Milestones.Should().BeEmpty();
        });
        var mutationRequest = CreateAddRequest(expectedResult.expectedMilestone);

        // Act
        var response = await GraphQLClient.SendMutationAsync<AddMilestoneResponse>(mutationRequest);

        // Assert
        AssertMilestoneNotAdded(response, expectedResult.errors);
    }

    [TestCaseSource(nameof(AddMilestoneCases))]
    public async Task UpdateMilestoneShouldUpdateMilestone(Milestone expectedMilestone)
    {
        // Arrange
        var dbMilestone = new Milestone
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
        CheckDbContentAsync(context =>
        {
            context.Milestones.Should().ContainEquivalentOf(dbMilestone);
        });
        var mutationRequest = CreateUpdateRequest(expectedMilestone);

        var startTime = DateTime.UtcNow;

        // Act
        var response = await GraphQLClient.SendMutationAsync<UpdateMilestoneResponse>(mutationRequest);

        // Assert
        AssertUpdatedMilestone(response, expectedMilestone, startTime);
    }

    [TestCaseSource(nameof(AddMilestoneCases))]
    public async Task UpdateMilestoneShouldUpdateMilestoneIfMilestonesAreNotEmpty(Milestone expectedMilestone)
    {
        // Arrange
        var dbMilestone = new Milestone
        {
            Id = new Guid("87653DC5-B029-4BA6-959A-1FBFC48E2C81"),
            Title = "Title",
            Description = "Desc",
            State = MilestoneState.Open,
            LastModifiedAt = DateTime.UtcNow.AddDays(1)
        };
        var dbMilestone2 = new Milestone
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
        CheckDbContentAsync(context =>
        {
            context.Milestones.Should().ContainEquivalentOf(dbMilestone);
            context.Milestones.Should().ContainEquivalentOf(dbMilestone2);
        });
        var mutationRequest = CreateUpdateRequest(expectedMilestone);

        var startTime = DateTime.UtcNow;

        // Act
        var response = await GraphQLClient.SendMutationAsync<UpdateMilestoneResponse>(mutationRequest);

        // Assert
        AssertUpdatedMilestone(response, expectedMilestone, startTime, dbMilestone, dbMilestone2);
    }

    [TestCaseSource(nameof(InvalidUpdateMilestoneCases))]
    public async Task UpdateMilestoneShouldNotUpdateMilestone((Milestone expectedMilestone, IEnumerable<string> errors) expectedResult)
    {
        // Arrange
        CheckDbContentAsync(context =>
        {
            context.Milestones.Should().BeEmpty();
        });
        var mutationRequest = CreateUpdateRequest(expectedResult.expectedMilestone);

        // Act
        var response = await GraphQLClient.SendMutationAsync<UpdateMilestoneResponse>(mutationRequest);

        // Assert
        AssertMilestoneNotUpdated(response, expectedResult.errors);
    }

    [Test]
    public async Task UpdateMilestoneShouldNotUpdateMilestoneIfMilestoneDoesNotExist()
    {
        // Arrange
        var milestone = CreateMilestone();
        CheckDbContentAsync(context =>
        {
            context.Milestones.Should().BeEmpty();
        });
        var mutationRequest = CreateUpdateRequest(milestone);

        // Act
        var response = await GraphQLClient.SendMutationAsync<UpdateMilestoneResponse>(mutationRequest);

        // Assert
        AssertMilestoneNotUpdated(response, new List<string> { $"No {nameof(Milestone)} found with id '{milestone.Id}'." });
    }

    [Test]
    public async Task DeleteMilestoneShouldDeleteMilestone()
    {
        // Arrange
        var milestone = CreateMilestone();
        var dbMilestone = new Milestone
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
        CheckDbContentAsync(context =>
        {
            context.Milestones.Should().ContainEquivalentOf(dbMilestone);
        });
        var mutationRequest = CreateDeleteRequest(milestone);

        // Act
        var response = await GraphQLClient.SendMutationAsync<DeleteMilestoneResponse>(mutationRequest);

        // Assert
        AssertDeletedMilestone(response, milestone);
    }

    [Test]
    public async Task DeleteMilestoneShouldDeleteMilestoneAndReferenceToIssues()
    {
        // Arrange
        var milestone = CreateMilestone();
        var dbMilestone = new Milestone
        {
            Id = milestone.Id,
            Title = milestone.Title,
            Description = milestone.Description,
            State = milestone.State,
            LastModifiedAt = DateTime.UtcNow.AddDays(1)
        };
        var dbIssue = new Issue
        {
            Id = new Guid("87A2F9BF-CAB7-41D3-84F9-155135FA41D7"),
            Title = "IssueTitle",
            Milestone = dbMilestone
        };
        dbMilestone.Issues.Add(dbIssue);
        var dbMilestone2 = new Milestone
        {
            Id = new Guid("B961A621-9848-429A-8B44-B1AF1F0182CE"),
            State = MilestoneState.Closed,
            Title = "Title 2"
        };
        var dbIssue2 = new Issue
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
        CheckDbContentAsync(context =>
        {
            context.Milestones.Should().ContainEquivalentOf(dbMilestone, options => options.Excluding(entity => entity.Issues));
        });
        var mutationRequest = CreateDeleteRequest(milestone);

        // Act
        var response = await GraphQLClient.SendMutationAsync<DeleteMilestoneResponse>(mutationRequest);

        // Assert
        AssertDeletedMilestone(response, milestone);
        CheckDbContentAsync(context =>
        {
            var dbIssues = context.Issues.Include(dbEntity => dbEntity.Milestone).ToList();
            dbIssues.Should().Contain(i => i.Id.Equals(dbIssue.Id));
            dbIssues.Should().Contain(i => i.Id.Equals(dbIssue2.Id));

            var changedDbIssue = context.Issues.Include(dbEntity => dbEntity.Milestone).Single(dbEntity => dbEntity.Id.Equals(dbIssue.Id));
            changedDbIssue.Milestone.Should().BeNull();

            var unchangedDbIssue = context.Issues.Include(dbEntity => dbEntity.Milestone).Single(dbEntity => dbEntity.Id.Equals(dbIssue2.Id));
            unchangedDbIssue.Milestone.Should().NotBeNull();
            unchangedDbIssue.Milestone!.Id.Should().Be(dbMilestone2.Id);
        });
    }

    [Test]
    public async Task DeleteMilestoneShouldDeleteMilestoneIfMilestonesIsNotEmpty()
    {
        // Arrange
        var milestone = CreateMilestone();
        var dbMilestone = new Milestone
        {
            Id = milestone.Id,
            Title = milestone.Title,
            Description = milestone.Description,
            State = milestone.State,
            LastModifiedAt = DateTime.UtcNow.AddDays(1)
        };
        var dbMilestone2 = new Milestone
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
        CheckDbContentAsync(context =>
        {
            context.Milestones.Should().ContainEquivalentOf(dbMilestone);
            context.Milestones.Should().ContainEquivalentOf(dbMilestone2);
        });
        var mutationRequest = CreateDeleteRequest(milestone);

        // Act
        var response = await GraphQLClient.SendMutationAsync<DeleteMilestoneResponse>(mutationRequest);

        // Assert
        AssertDeletedMilestone(response, milestone, dbMilestone2);
    }

    [Test]
    public async Task DeleteMilestoneShouldNotDeleteMilestoneIfMilestoneDoesNotExist()
    {
        // Arrange
        var milestone = CreateMilestone();
        CheckDbContentAsync(context =>
        {
            context.Milestones.Should().BeEmpty();
        });
        var mutationRequest = CreateDeleteRequest(milestone);

        // Act
        var response = await GraphQLClient.SendMutationAsync<DeleteMilestoneResponse>(mutationRequest);

        // Assert
        AssertMilestoneNotDeleted(response, new List<string> { $"No {nameof(Milestone)} found with id '{milestone.Id}'." });
    }

    private static GraphQLRequest CreateAddRequest(Milestone expectedMilestone)
    {
        var descriptionParameter = expectedMilestone.Description != null
            ? $"""
               , description: "{expectedMilestone.Description}"
               """
            : string.Empty;
        var mutationRequest = new GraphQLRequest
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

    private void AssertAddedMilestone(GraphQLResponse<AddMilestoneResponse> response, Milestone expectedMilestone,
        DateTime startTime, Milestone? dbMilestone = null)
    {
        DateTime endTime = DateTime.UtcNow;
        Milestone? addedMilestone;
        using (new AssertionScope())
        {
            response.Should().NotBeNull();
            response.Errors.Should().BeNullOrEmpty();
            addedMilestone = response.Data.AddMilestone.Milestone;
            addedMilestone.Id.Should().NotBeEmpty();
            addedMilestone.Title.Should().Be(expectedMilestone.Title);
            addedMilestone.Description.Should().Be(expectedMilestone.Description);
            addedMilestone.State.Should().Be(expectedMilestone.State);
            addedMilestone.Issues.Should().BeEquivalentTo(expectedMilestone.Issues);
            addedMilestone.CreatedAt.Should().BeCloseTo(startTime, TimeSpan.FromSeconds(1), "Start time").And
                .BeCloseTo(endTime, TimeSpan.FromSeconds(1), "End time");
            addedMilestone.LastModifiedAt.Should().BeNull();
        }

        CheckDbContentAsync(context =>
        {
            using (new AssertionScope())
            {
                if (dbMilestone is not null)
                {
                    context.Milestones.Any(dbMilestone1 => dbMilestone1.Id.Equals(dbMilestone.Id)).Should().BeTrue();
                }
                var addedDbMilestone = context.Milestones.Include(dbMilestone2 => dbMilestone2.Issues)
                    .First(dbMilestone1 => dbMilestone1.Id.Equals(addedMilestone.Id));
                addedDbMilestone.Should().NotBeNull();
                addedDbMilestone.Id.Should().NotBeEmpty().And.Be(addedMilestone.Id);
                addedDbMilestone.Title.Should().Be(expectedMilestone.Title);
                addedDbMilestone.Description.Should().Be(expectedMilestone.Description);
                addedDbMilestone.State.Should().Be(expectedMilestone.State);
                addedDbMilestone.Issues.Should().BeEquivalentTo(expectedMilestone.Issues);
                addedDbMilestone.CreatedAt.Should().BeCloseTo(startTime, TimeSpan.FromSeconds(1), "Start time").And
                    .BeCloseTo(endTime, TimeSpan.FromSeconds(1), "End time");
                addedDbMilestone.LastModifiedAt.Should().BeNull();
            }
        });
    }

    private void AssertMilestoneNotAdded(GraphQLResponse<AddMilestoneResponse> response, IEnumerable<string> errors)
    {
        using (new AssertionScope())
        {
            response.Should().NotBeNull();
            response.Data.AddMilestone.Errors.Should().NotBeNullOrEmpty();
            response.Data.AddMilestone.Milestone.Should().BeNull();

            var resultErrors = response.Data.AddMilestone.Errors.Select(error => error.Message);
            resultErrors.Should().BeEquivalentTo(errors);
        }

        CheckDbContentAsync(context =>
        {
            context.Milestones.Should().BeEmpty();
        });
    }

    private static GraphQLRequest CreateUpdateRequest(Milestone expectedMilestone)
    {
        var descriptionParameter = expectedMilestone.Description != null
            ? $"""
               , description: "{expectedMilestone.Description}"
               """
            : string.Empty;
        var mutationRequest = new GraphQLRequest
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

    private void AssertUpdatedMilestone(GraphQLResponse<UpdateMilestoneResponse> response, Milestone expectedMilestone,
        DateTime startTime, Milestone? dbMilestone = null, Milestone? notUpdatedMilestone = null, bool emptyIssues = true)
    {
        DateTime endTime = DateTime.UtcNow;
        Milestone? updatedMilestone;
        using (new AssertionScope())
        {
            response.Should().NotBeNull();
            response.Errors.Should().BeNullOrEmpty();
            updatedMilestone = response.Data.UpdateMilestone.Milestone;
            updatedMilestone.Id.Should().Be(expectedMilestone.Id);
            updatedMilestone.Title.Should().Be(expectedMilestone.Title);
            updatedMilestone.Description.Should().Be(expectedMilestone.Description);
            updatedMilestone.State.Should().Be(expectedMilestone.State);
            updatedMilestone.CreatedAt.Should().BeCloseTo(expectedMilestone.CreatedAt, TimeSpan.FromSeconds(1));
            updatedMilestone.LastModifiedAt.Should().BeCloseTo(startTime, TimeSpan.FromSeconds(1), "Start time").And
                .BeCloseTo(endTime, TimeSpan.FromSeconds(1), "End time");
            if (emptyIssues)
            {
                updatedMilestone.Issues.Should().BeEmpty();
            }
            else
            {
                updatedMilestone.Issues.Should().NotBeEmpty();
            }
        }

        CheckDbContentAsync(context =>
        {
            using (new AssertionScope())
            {
                if (dbMilestone is not null)
                {
                    context.Milestones.Any(dbMilestone1 => dbMilestone1.Id.Equals(dbMilestone.Id)).Should().BeTrue();
                }
                var updatedDbMilestone = context.Milestones.Include(dbMilestone2 => dbMilestone2.Issues)
                    .First(dbMilestone1 => dbMilestone1.Id.Equals(updatedMilestone.Id));
                updatedDbMilestone.Should().NotBeNull();
                updatedDbMilestone.Id.Should().NotBeEmpty().And.Be(updatedMilestone.Id);
                updatedDbMilestone.Title.Should().Be(expectedMilestone.Title);
                updatedDbMilestone.Description.Should().Be(expectedMilestone.Description);
                updatedDbMilestone.State.Should().Be(expectedMilestone.State);
                updatedDbMilestone.CreatedAt.Should().BeCloseTo(expectedMilestone.CreatedAt, TimeSpan.FromSeconds(1));
                updatedDbMilestone.LastModifiedAt.Should().BeCloseTo(startTime, TimeSpan.FromSeconds(1), "Start time").And
                    .BeCloseTo(endTime, TimeSpan.FromSeconds(1), "End time");
                if (emptyIssues)
                {
                    updatedDbMilestone.Issues.Should().BeEmpty();
                }
                else
                {
                    updatedDbMilestone.Issues.Should().NotBeEmpty();
                }

                if (notUpdatedMilestone is not null)
                {
                    var secondMilestone =
                        context.Milestones.Include(dbMilestone2 => dbMilestone2.Issues)
                            .FirstOrDefault(milestone => milestone.Id.Equals(notUpdatedMilestone.Id));
                    secondMilestone.Should().NotBeNull();
                    secondMilestone!.Id.Should().NotBeEmpty().And.Be(notUpdatedMilestone.Id);
                    secondMilestone.Title.Should().Be(notUpdatedMilestone.Title);
                    secondMilestone.Description.Should().Be(notUpdatedMilestone.Description);
                    secondMilestone.State.Should().Be(notUpdatedMilestone.State);
                    secondMilestone.CreatedAt.Should().BeCloseTo(notUpdatedMilestone.CreatedAt, TimeSpan.FromSeconds(1));
                    secondMilestone.LastModifiedAt.Should().BeCloseTo(notUpdatedMilestone.LastModifiedAt!.Value, TimeSpan.FromSeconds(1));
                    if (emptyIssues)
                    {
                        secondMilestone.Issues.Should().BeEmpty();
                    }
                    else
                    {
                        secondMilestone.Issues.Should().NotBeEmpty();
                    }
                }
            }
        });
    }

    private void AssertMilestoneNotUpdated(GraphQLResponse<UpdateMilestoneResponse> response, IEnumerable<string> errors)
    {
        using (new AssertionScope())
        {
            response.Should().NotBeNull();
            response.Data.UpdateMilestone.Errors.Should().NotBeNullOrEmpty();
            response.Data.UpdateMilestone.Milestone.Should().BeNull();

            var resultErrors = response.Data.UpdateMilestone.Errors.Select(error => error.Message);
            resultErrors.Should().BeEquivalentTo(errors);
        }

        CheckDbContentAsync(context =>
        {
            context.Milestones.Should().BeEmpty();
        });
    }

    private static GraphQLRequest CreateDeleteRequest(Milestone expectedMilestone)
    {
        var mutationRequest = new GraphQLRequest
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

    private void AssertDeletedMilestone(GraphQLResponse<DeleteMilestoneResponse> response, Milestone expectedMilestone, Milestone? dbMilestone = null)
    {
        using (new AssertionScope())
        {
            response.Should().NotBeNull();
            response.Errors.Should().BeNullOrEmpty();
            var deletedMilestone = response.Data.DeleteMilestone.Milestone;
            deletedMilestone.Id.Should().NotBeEmpty();
            deletedMilestone.Title.Should().Be(expectedMilestone.Title);
            deletedMilestone.Description.Should().Be(expectedMilestone.Description);
            deletedMilestone.State.Should().Be(expectedMilestone.State);
            deletedMilestone.CreatedAt.Should().BeCloseTo(expectedMilestone.CreatedAt, TimeSpan.FromSeconds(1));
            deletedMilestone.LastModifiedAt.Should().BeCloseTo(expectedMilestone.LastModifiedAt!.Value, TimeSpan.FromSeconds(1));
        }

        CheckDbContentAsync(context =>
        {
            using (new AssertionScope())
            {
                context.Milestones.Any(dbMilestone1 => dbMilestone1.Id.Equals(expectedMilestone.Id)).Should().BeFalse();

                if (dbMilestone is not null)
                {
                    context.Milestones.Any(dbMilestone1 => dbMilestone1.Id.Equals(dbMilestone.Id)).Should().BeTrue();
                }
            }
        });
    }

    private void AssertMilestoneNotDeleted(GraphQLResponse<DeleteMilestoneResponse> response, IEnumerable<string> errors)
    {
        using (new AssertionScope())
        {
            response.Should().NotBeNull();
            response.Data.DeleteMilestone.Errors.Should().NotBeNullOrEmpty();
            response.Data.DeleteMilestone.Milestone.Should().BeNull();

            var resultErrors = response.Data.DeleteMilestone.Errors.Select(error => error.Message);
            resultErrors.Should().BeEquivalentTo(errors);
        }

        CheckDbContentAsync(context =>
        {
            context.Milestones.Should().BeEmpty();
        });
    }

    private static Milestone CreateMilestone()
    {
        var faker = new Faker<Milestone>()
            .RuleFor(milestone => milestone.Id, f => f.Random.Guid())
            .RuleFor(milestone => milestone.Title, f => f.Random.String2(1, 50, AllowedChars))
            .RuleFor(milestone => milestone.Description, f => f.Random.String2(0, 255, AllowedChars).OrNull(f, 0.3f))
            .RuleFor(milestone => milestone.State, _ => MilestoneState.Open);
        return faker.Generate();
    }

    private static IEnumerable<Milestone> AddMilestoneCases()
    {
        var faker = new Faker<Milestone>()
            .RuleFor(milestone => milestone.Title, f => f.Random.String2(1, 50, AllowedChars))
            .RuleFor(milestone => milestone.Description, f => f.Random.String2(0, 255, AllowedChars).OrNull(f, 0.3f))
            .RuleFor(milestone => milestone.State, _ => MilestoneState.Open);
        return faker.Generate(20);
    }

    private static IEnumerable<(Milestone, IEnumerable<string>)> InvalidAddMilestoneCases()
    {
        yield return (new Milestone { Title = null!, Description = null, State = MilestoneState.Open }, new List<string> { $"The value for {nameof(Milestone.Title)} is not set.", $"The value '' for {nameof(Milestone.Title)} is too short. The length of {nameof(Milestone.Title)} has to be between 1 and 50." });
        yield return (new Milestone { Title = "", Description = null, State = MilestoneState.Open }, new List<string> { $"The value for {nameof(Milestone.Title)} is not set.", $"The value '' for {nameof(Milestone.Title)} is too short. The length of {nameof(Milestone.Title)} has to be between 1 and 50." });
        yield return (new Milestone { Title = "  \t ", Description = null, State = MilestoneState.Open }, new List<string> { $"The value for {nameof(Milestone.Title)} is not set." });
        yield return (new Milestone { Title = new string('a', 51), Description = null, State = MilestoneState.Open }, new List<string> { $"The value 'aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa' for {nameof(Milestone.Title)} is long short. The length of {nameof(Milestone.Title)} has to be between 1 and 50." });
        yield return (new Milestone { Title = "Valid", Description = new string('a', 256), State = MilestoneState.Open }, new List<string> { $"The value 'aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa' for {nameof(Milestone.Description)} is long short. The length of {nameof(Milestone.Description)} has to be less than 256." });
    }

    private static IEnumerable<(Milestone, IEnumerable<string>)> InvalidUpdateMilestoneCases()
    {
        yield return (new Milestone { Title = null!, Description = null, State = MilestoneState.Open }, new List<string> { $"The value for {nameof(Milestone.Title)} is not set.", $"The value '' for {nameof(Milestone.Title)} is too short. The length of {nameof(Milestone.Title)} has to be between 1 and 50." });
        yield return (new Milestone { Title = "", Description = null, State = MilestoneState.Open }, new List<string> { $"The value for {nameof(Milestone.Title)} is not set.", $"The value '' for {nameof(Milestone.Title)} is too short. The length of {nameof(Milestone.Title)} has to be between 1 and 50." });
        yield return (new Milestone { Title = "  \t ", Description = null, State = MilestoneState.Open }, new List<string> { $"The value for {nameof(Milestone.Title)} is not set." });
        yield return (new Milestone { Title = new string('a', 51), Description = null, State = MilestoneState.Open }, new List<string> { $"The value 'aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa' for {nameof(Milestone.Title)} is long short. The length of {nameof(Milestone.Title)} has to be between 1 and 50." });
        yield return (new Milestone { Title = "Valid", Description = new string('a', 256), State = MilestoneState.Open }, new List<string> { $"The value 'aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa' for {nameof(Milestone.Description)} is long short. The length of {nameof(Milestone.Description)} has to be less than 256." });
        yield return (new Milestone { Title = "Valid", Description = null, State = MilestoneState.Unknown }, new List<string> { $"The value for {nameof(Milestone.State)} is not set." });
    }
}
