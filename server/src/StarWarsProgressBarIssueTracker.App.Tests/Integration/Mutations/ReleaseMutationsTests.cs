using Bogus;
using GraphQL;
using GraphQL.Client.Http;
using Microsoft.EntityFrameworkCore;
using StarWarsProgressBarIssueTracker.App.Releases;
using StarWarsProgressBarIssueTracker.App.Tests.Helpers;
using StarWarsProgressBarIssueTracker.App.Tests.Helpers.GraphQL.Payloads.Releases;
using StarWarsProgressBarIssueTracker.Domain.Issues;
using StarWarsProgressBarIssueTracker.Domain.Releases;
using StarWarsProgressBarIssueTracker.TestHelpers;

namespace StarWarsProgressBarIssueTracker.App.Tests.Integration.Mutations;

[Category(TestCategory.Integration)]
[NotInParallel(NotInParallelTests.ReleaseMutation)]
public class ReleaseMutationsTests : IntegrationTestBase
{
    // TODO: Add integration tests
    private const string AllowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ß_#%";

    [Test]
    [MethodDataSource(nameof(AddReleaseCases))]
    public async Task AddReleaseShouldAddRelease(Release expectedRelease)
    {
        // Arrange
        DateTime startTime = DateTime.UtcNow;
        await CheckDbContentAsync(async context =>
        {
            await Assert.That(context.Releases).IsEmpty();
        });
        GraphQLRequest mutationRequest = CreateAddRequest(expectedRelease);
        expectedRelease.State = ReleaseState.Open;
        GraphQLHttpClient graphQlHttpClient = await CreateAuthenticatedGraphQLClientAsync();

        // Act
        GraphQLResponse<AddReleaseResponse> response = await graphQlHttpClient.SendMutationAsync<AddReleaseResponse>(mutationRequest);

        // Assert
        await AssertAddedReleaseAsync(response, expectedRelease, startTime);
    }

    [Test]
    [MethodDataSource(nameof(AddReleaseCases))]
    public async Task AddReleaseShouldAddReleaseIfReleasesAreNotEmpty(Release expectedRelease)
    {
        // Arrange
        DateTime startTime = DateTime.UtcNow;
        Release dbRelease = new Release
        {
            Id = new Guid("87653DC5-B029-4BA6-959A-1FBFC48E2C81"),
            Title = "Title",
            Notes = "Desc",
            State = ReleaseState.Open,
            LastModifiedAt = DateTime.UtcNow.AddDays(1)
        };
        await SeedDatabaseAsync(context =>
        {
            context.Releases.Add(dbRelease);
        });
        await CheckDbContentAsync(async context =>
        {
            await Assert.That(context.Releases.ToList()).ContainsEquivalentOf(dbRelease);
        });
        GraphQLRequest mutationRequest = CreateAddRequest(expectedRelease);
        GraphQLHttpClient graphQlHttpClient = await CreateAuthenticatedGraphQLClientAsync();

        // Act
        GraphQLResponse<AddReleaseResponse> response = await graphQlHttpClient.SendMutationAsync<AddReleaseResponse>(mutationRequest);

        // Assert
        await AssertAddedReleaseAsync(response, expectedRelease, startTime, dbRelease);
    }

    [Test]
    [MethodDataSource(nameof(InvalidAddReleaseCases))]
    public async Task AddReleaseShouldNotAddRelease((Release expectedRelease, IEnumerable<string> errors) expectedResult)
    {
        // Arrange
        await CheckDbContentAsync(async context =>
        {
            await Assert.That(context.Releases).IsEmpty();
        });
        GraphQLRequest mutationRequest = CreateAddRequest(expectedResult.expectedRelease);
        GraphQLHttpClient graphQlHttpClient = await CreateAuthenticatedGraphQLClientAsync();

        // Act
        GraphQLResponse<AddReleaseResponse> response = await graphQlHttpClient.SendMutationAsync<AddReleaseResponse>(mutationRequest);

        // Assert
        await AssertReleaseNotAddedAsync(response, expectedResult.errors);
    }

    [Test]
    [MethodDataSource(nameof(AddReleaseCases))]
    public async Task UpdateReleaseShouldUpdateRelease(Release expectedRelease)
    {
        // Arrange
        DateTime startTime = DateTime.UtcNow;
        Release dbRelease = new Release
        {
            Id = new Guid("87653DC5-B029-4BA6-959A-1FBFC48E2C81"),
            Title = "Title",
            Notes = "Desc",
            State = ReleaseState.Open,
            LastModifiedAt = DateTime.UtcNow.AddDays(1)
        };
        await SeedDatabaseAsync(context =>
        {
            context.Releases.Add(dbRelease);
        });
        expectedRelease.Id = dbRelease.Id;
        expectedRelease.CreatedAt = dbRelease.CreatedAt;
        await CheckDbContentAsync(async context =>
        {
            await Assert.That(context.Releases.ToList()).ContainsEquivalentOf(dbRelease);
        });
        GraphQLRequest mutationRequest = CreateUpdateRequest(expectedRelease);
        GraphQLHttpClient graphQlHttpClient = await CreateAuthenticatedGraphQLClientAsync();

        // Act
        GraphQLResponse<UpdateReleaseResponse> response = await graphQlHttpClient.SendMutationAsync<UpdateReleaseResponse>(mutationRequest);

        // Assert
        await AssertUpdatedReleaseAsync(response, expectedRelease, startTime);
    }

    [Test]
    [MethodDataSource(nameof(AddReleaseCases))]
    public async Task UpdateReleaseShouldUpdateReleaseIfReleasesAreNotEmpty(Release expectedRelease)
    {
        // Arrange
        DateTime startTime = DateTime.UtcNow;
        Release dbRelease = new Release
        {
            Id = new Guid("87653DC5-B029-4BA6-959A-1FBFC48E2C81"),
            Title = "Title",
            Notes = "Desc",
            State = ReleaseState.Open,
            LastModifiedAt = DateTime.UtcNow.AddDays(1)
        };
        Release dbRelease2 = new Release
        {
            Id = new Guid("0609F93C-CBCC-4650-BA4C-B8D5FF93A877"),
            Title = "Title 2",
            Notes = "Desc 2",
            State = ReleaseState.Finished,
            LastModifiedAt = DateTime.UtcNow.AddDays(2)
        };

        await SeedDatabaseAsync(context =>
        {
            context.Releases.Add(dbRelease);
            context.Releases.Add(dbRelease2);
        });
        expectedRelease.Id = dbRelease.Id;
        expectedRelease.CreatedAt = dbRelease.CreatedAt;
        await CheckDbContentAsync(async context =>
        {
            using (Assert.Multiple())
            {
                List<Release> releases = context.Releases.ToList();
                await Assert.That(releases).ContainsEquivalentOf(dbRelease);
                await Assert.That(releases).ContainsEquivalentOf(dbRelease2);
            }
        });
        GraphQLRequest mutationRequest = CreateUpdateRequest(expectedRelease);
        GraphQLHttpClient graphQlHttpClient = await CreateAuthenticatedGraphQLClientAsync();

        // Act
        GraphQLResponse<UpdateReleaseResponse> response = await graphQlHttpClient.SendMutationAsync<UpdateReleaseResponse>(mutationRequest);

        // Assert
        await AssertUpdatedReleaseAsync(response, expectedRelease, startTime, dbRelease, dbRelease2);
    }

    [Test]
    [MethodDataSource(nameof(InvalidUpdateReleaseCases))]
    public async Task UpdateReleaseShouldNotUpdateRelease((Release expectedRelease, IEnumerable<string> errors) expectedResult)
    {
        // Arrange
        await CheckDbContentAsync(async context =>
        {
            await Assert.That(context.Releases).IsEmpty();
        });
        GraphQLRequest mutationRequest = CreateUpdateRequest(expectedResult.expectedRelease);
        GraphQLHttpClient graphQlHttpClient = await CreateAuthenticatedGraphQLClientAsync();

        // Act
        GraphQLResponse<UpdateReleaseResponse> response = await graphQlHttpClient.SendMutationAsync<UpdateReleaseResponse>(mutationRequest);

        // Assert
        await AssertReleaseNotUpdatedAsync(response, expectedResult.errors);
    }

    [Test]
    public async Task UpdateReleaseShouldNotUpdateReleaseIfReleaseDoesNotExist()
    {
        // Arrange
        Release release = CreateRelease();
        await CheckDbContentAsync(async context =>
        {
            await Assert.That(context.Releases).IsEmpty();
        });
        GraphQLRequest mutationRequest = CreateUpdateRequest(release);
        GraphQLHttpClient graphQlHttpClient = await CreateAuthenticatedGraphQLClientAsync();

        // Act
        GraphQLResponse<UpdateReleaseResponse> response = await graphQlHttpClient.SendMutationAsync<UpdateReleaseResponse>(mutationRequest);

        // Assert
        await AssertReleaseNotUpdatedAsync(response, new List<string> { $"No {nameof(Release)} found with id '{release.Id}'." });
    }

    [Test]
    public async Task DeleteReleaseShouldDeleteRelease()
    {
        // Arrange
        Release release = CreateRelease();
        Release dbRelease = new Release
        {
            Id = release.Id,
            Title = release.Title,
            Notes = release.Notes,
            State = release.State,
            LastModifiedAt = DateTime.UtcNow.AddDays(1)
        };
        await SeedDatabaseAsync(context =>
        {
            context.Releases.Add(dbRelease);
        });
        release.CreatedAt = dbRelease.CreatedAt;
        release.LastModifiedAt = dbRelease.LastModifiedAt;
        await CheckDbContentAsync(async context =>
        {
            await Assert.That(context.Releases.ToList()).ContainsEquivalentOf(dbRelease);
        });
        GraphQLRequest mutationRequest = CreateDeleteRequest(release);
        GraphQLHttpClient graphQlHttpClient = await CreateAuthenticatedGraphQLClientAsync();

        // Act
        GraphQLResponse<DeleteReleaseResponse> response = await graphQlHttpClient.SendMutationAsync<DeleteReleaseResponse>(mutationRequest);

        // Assert
        await AssertDeletedReleaseAsync(response, release);
    }

    [Test]
    public async Task DeleteReleaseShouldDeleteReleaseAndReferenceToIssues()
    {
        // Arrange
        Release release = CreateRelease();
        Release dbRelease = new Release
        {
            Id = release.Id,
            Title = release.Title,
            Notes = release.Notes,
            State = release.State,
            LastModifiedAt = DateTime.UtcNow.AddDays(1)
        };
        Issue dbIssue = new Issue
        {
            Id = new Guid("87A2F9BF-CAB7-41D3-84F9-155135FA41D7"),
            Title = "IssueTitle",
            Release = dbRelease
        };
        dbRelease.Issues.Add(dbIssue);
        Release dbRelease2 = new Release
        {
            Id = new Guid("B961A621-9848-429A-8B44-B1AF1F0182CE"),
            State = ReleaseState.Finished,
            Title = "Title 2"
        };
        Issue dbIssue2 = new Issue
        {
            Id = new Guid("74AE8DD4-7669-4428-8E81-FB8A24A217A3"),
            Title = "IssueTitle",
            Release = dbRelease2
        };
        dbRelease2.Issues.Add(dbIssue2);
        await SeedDatabaseAsync(context =>
        {
            context.Releases.Add(dbRelease);
            context.Releases.Add(dbRelease2);
            context.Issues.Add(dbIssue);
            context.Issues.Add(dbIssue2);
        });
        release.CreatedAt = dbRelease.CreatedAt;
        release.LastModifiedAt = dbRelease.LastModifiedAt;
        await CheckDbContentAsync(async context =>
        {
            await Assert.That(context.Releases.ToList()).ContainsEquivalentOf(dbRelease);
        });
        GraphQLRequest mutationRequest = CreateDeleteRequest(release);
        GraphQLHttpClient graphQlHttpClient = await CreateAuthenticatedGraphQLClientAsync();

        // Act
        GraphQLResponse<DeleteReleaseResponse> response = await graphQlHttpClient.SendMutationAsync<DeleteReleaseResponse>(mutationRequest);

        // Assert
        await AssertDeletedReleaseAsync(response, release);
        await CheckDbContentAsync(async context =>
        {
            using (Assert.Multiple())
            {
                List<Issue> dbIssues = context.Issues.Include(dbEntity => dbEntity.Release).ToList();
                await Assert.That(dbIssues).Contains(i => i.Id.Equals(dbIssue.Id));
                await Assert.That(dbIssues).Contains(i => i.Id.Equals(dbIssue2.Id));

                Issue changedDbIssue = context.Issues.Include(dbEntity => dbEntity.Release).Single(dbEntity => dbEntity.Id.Equals(dbIssue.Id));
                await Assert.That(changedDbIssue.Release).IsNull();

                Issue unchangedDbIssue = context.Issues.Include(dbEntity => dbEntity.Release).Single(dbEntity => dbEntity.Id.Equals(dbIssue2.Id));
                await Assert.That(unchangedDbIssue.Release).IsNotNull();
                await Assert.That(unchangedDbIssue.Release!.Id).IsEqualTo(dbRelease2.Id);
            }
        });
    }

    [Test]
    public async Task DeleteReleaseShouldDeleteReleaseIfReleasesIsNotEmpty()
    {
        // Arrange
        Release release = CreateRelease();
        Release dbRelease = new Release
        {
            Id = release.Id,
            Title = release.Title,
            Notes = release.Notes,
            State = release.State,
            LastModifiedAt = DateTime.UtcNow.AddDays(1)
        };
        Release dbRelease2 = new Release
        {
            Id = new Guid("0609F93C-CBCC-4650-BA4C-B8D5FF93A877"),
            Title = "Title 2",
            Notes = "Desc 2",
            State = ReleaseState.Finished,
            LastModifiedAt = DateTime.UtcNow.AddDays(2)
        };

        await SeedDatabaseAsync(context =>
        {
            context.Releases.Add(dbRelease);
            context.Releases.Add(dbRelease2);
        });
        release.CreatedAt = dbRelease.CreatedAt;
        release.LastModifiedAt = dbRelease.LastModifiedAt;
        await CheckDbContentAsync(async context =>
        {
            using (Assert.Multiple())
            {
                List<Release> releases = context.Releases.ToList();
                await Assert.That(releases).ContainsEquivalentOf(dbRelease);
                await Assert.That(releases).ContainsEquivalentOf(dbRelease2);
            }
        });
        GraphQLRequest mutationRequest = CreateDeleteRequest(release);
        GraphQLHttpClient graphQlHttpClient = await CreateAuthenticatedGraphQLClientAsync();

        // Act
        GraphQLResponse<DeleteReleaseResponse> response = await graphQlHttpClient.SendMutationAsync<DeleteReleaseResponse>(mutationRequest);

        // Assert
        await AssertDeletedReleaseAsync(response, release, dbRelease2);
    }

    [Test]
    public async Task DeleteReleaseShouldNotDeleteReleaseIfReleaseDoesNotExist()
    {
        // Arrange
        Release release = CreateRelease();
        await CheckDbContentAsync(async context =>
        {
            await Assert.That(context.Releases).IsEmpty();
        });
        GraphQLRequest mutationRequest = CreateDeleteRequest(release);
        GraphQLHttpClient graphQlHttpClient = await CreateAuthenticatedGraphQLClientAsync();

        // Act
        GraphQLResponse<DeleteReleaseResponse> response = await graphQlHttpClient.SendMutationAsync<DeleteReleaseResponse>(mutationRequest);

        // Assert
        await AssertReleaseNotDeletedAsync(response, new List<string> { $"No {nameof(Release)} found with id '{release.Id}'." });
    }

    private static GraphQLRequest CreateAddRequest(Release expectedRelease)
    {
        string descriptionParameter = expectedRelease.Notes != null
            ? $"""
               , releaseNotes: "{expectedRelease.Notes}"
               """
            : string.Empty;
        GraphQLRequest mutationRequest = new GraphQLRequest
        {
            Query = $$"""
                      mutation addRelease
                      {
                          addRelease(input: {title: "{{expectedRelease.Title}}"{{descriptionParameter}}})
                          {
                              release
                              {
                                  id
                                  title
                                  notes
                                  date
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
            OperationName = "addRelease"
        };
        return mutationRequest;
    }

    private async Task AssertAddedReleaseAsync(GraphQLResponse<AddReleaseResponse> response, Release expectedRelease,
        DateTime startTime, Release? dbRelease = null)
    {
        DateTime endTime = DateTime.UtcNow;
        ReleaseDto? addedRelease;
        using (Assert.Multiple())
        {
            await Assert.That(response).IsNotNull();
            await Assert.That(response.Errors).IsNull().Or.IsEmpty();
            addedRelease = response.Data.AddRelease.Release;
            await Assert.That(addedRelease.Id).IsNotDefault();
            await Assert.That(addedRelease.Title).IsEqualTo(expectedRelease.Title);
            await Assert.That(addedRelease.Notes).IsEqualTo(expectedRelease.Notes);
            await Assert.That(addedRelease.State).IsEqualTo(expectedRelease.State);
            await Assert.That(addedRelease.Issues).IsEquivalentTo(expectedRelease.Issues);
            await Assert.That(addedRelease.CreatedAt).IsBetween(startTime, endTime).WithInclusiveBounds();
            await Assert.That(addedRelease.LastModifiedAt).IsNull();
        }

        await CheckDbContentAsync(async context =>
        {
            using (Assert.Multiple())
            {
                if (dbRelease is not null)
                {
                    await Assert.That(context.Releases.ToList())
                        .Contains(dbRelease1 => dbRelease1.Id.Equals(dbRelease.Id));
                }
                Release addedDbRelease = context.Releases.Include(dbRelease2 => dbRelease2.Issues)
                    .First(dbRelease1 => dbRelease1.Id.Equals(addedRelease.Id));
                await Assert.That(addedDbRelease).IsNotNull();
                await Assert.That(addedDbRelease.Id).IsNotDefault().And.IsEqualTo(addedRelease.Id);
                await Assert.That(addedDbRelease.Title).IsEqualTo(expectedRelease.Title);
                await Assert.That(addedDbRelease.Notes).IsEqualTo(expectedRelease.Notes);
                await Assert.That(addedDbRelease.State).IsEqualTo(expectedRelease.State);
                await Assert.That(addedDbRelease.Issues).IsEquivalentTo(expectedRelease.Issues);
                await Assert.That(addedDbRelease.CreatedAt).IsBetween(startTime, endTime).WithInclusiveBounds();
                await Assert.That(addedDbRelease.LastModifiedAt).IsNull();
            }
        });
    }

    private async Task AssertReleaseNotAddedAsync(GraphQLResponse<AddReleaseResponse> response, IEnumerable<string> errors)
    {
        using (Assert.Multiple())
        {
           await Assert.That(response).IsNotNull();
           await Assert.That(response.Data.AddRelease.Errors).IsNotNull().And.IsNotEmpty();
           await Assert.That(response.Data.AddRelease.Release).IsNull();

            IEnumerable<string> resultErrors = response.Data.AddRelease.Errors.Select(error => error.Message);
            await Assert.That(resultErrors).IsEquivalentTo(errors);
        }

        await CheckDbContentAsync(async context =>
        {
            await Assert.That(context.Releases).IsEmpty();
        });
    }

    private static GraphQLRequest CreateUpdateRequest(Release expectedRelease)
    {
        string descriptionParameter = expectedRelease.Notes != null
            ? $"""
               , releaseNotes: "{expectedRelease.Notes}"
               """
            : string.Empty;
        GraphQLRequest mutationRequest = new GraphQLRequest
        {
            Query = $$"""
                      mutation updateRelease
                      {
                          updateRelease(input: {id: "{{expectedRelease.Id}}", title: "{{expectedRelease.Title}}", state: {{expectedRelease.State.ToString().ToUpper()}}{{descriptionParameter}}})
                          {
                              release
                              {
                                  id
                                  title
                                  notes
                                  date
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
            OperationName = "updateRelease"
        };
        return mutationRequest;
    }

    private async Task AssertUpdatedReleaseAsync(GraphQLResponse<UpdateReleaseResponse> response, Release expectedRelease,
        DateTime startTime, Release? dbRelease = null, Release? notUpdatedRelease = null, bool emptyIssues = true)
    {
        DateTime endTime = DateTime.UtcNow;
        ReleaseDto? updatedRelease;
        using (Assert.Multiple())
        {
            await Assert.That(response).IsNotNull();
            await Assert.That(response.Errors).IsNull().Or.IsEmpty();
            updatedRelease = response.Data.UpdateRelease.Release;
            await Assert.That(updatedRelease.Id).IsEqualTo(expectedRelease.Id);
            await Assert.That(updatedRelease.Title).IsEqualTo(expectedRelease.Title);
            await Assert.That(updatedRelease.Notes).IsEqualTo(expectedRelease.Notes);
            await Assert.That(updatedRelease.State).IsEqualTo(expectedRelease.State);
            await Assert.That(updatedRelease.CreatedAt).IsEquivalentTo(expectedRelease.CreatedAt);
            await Assert.That(updatedRelease.LastModifiedAt!.Value).IsBetween(startTime, endTime).WithInclusiveBounds();
            if (emptyIssues)
            {
                await Assert.That(updatedRelease.Issues).IsEmpty();
            }
            else
            {
                await Assert.That(updatedRelease.Issues).IsNotEmpty();
            }
        }

        await CheckDbContentAsync(async context =>
        {
            using (Assert.Multiple())
            {
                if (dbRelease is not null)
                {
                    await Assert.That(context.Releases.ToList())
                        .Contains(dbRelease1 => dbRelease1.Id.Equals(dbRelease.Id));
                }
                Release updatedDbRelease = context.Releases.Include(dbRelease2 => dbRelease2.Issues)
                    .First(dbRelease1 => dbRelease1.Id.Equals(updatedRelease.Id));
                await Assert.That(updatedDbRelease).IsNotNull();
                await Assert.That(updatedDbRelease.Id).IsNotDefault().And.IsEqualTo(updatedRelease.Id);
                await Assert.That(updatedDbRelease.Title).IsEqualTo(expectedRelease.Title);
                await Assert.That(updatedDbRelease.Notes).IsEqualTo(expectedRelease.Notes);
                await Assert.That(updatedDbRelease.State).IsEqualTo(expectedRelease.State);
                await Assert.That(updatedDbRelease.CreatedAt).IsEquivalentTo(expectedRelease.CreatedAt);
                await Assert.That(updatedDbRelease.LastModifiedAt!.Value).IsBetween(startTime, endTime).WithInclusiveBounds();
                if (emptyIssues)
                {
                    await Assert.That(updatedDbRelease.Issues).IsEmpty();
                }
                else
                {
                    await Assert.That(updatedDbRelease.Issues).IsNotEmpty();
                }

                if (notUpdatedRelease is not null)
                {
                    Release? secondRelease =
                        context.Releases.Include(dbRelease2 => dbRelease2.Issues)
                            .FirstOrDefault(release => release.Id.Equals(notUpdatedRelease.Id));
                    await Assert.That(secondRelease).IsNotNull();
                    await Assert.That(secondRelease!.Id).IsNotDefault().And.IsEqualTo(notUpdatedRelease.Id);
                    await Assert.That(secondRelease.Title).IsEqualTo(notUpdatedRelease.Title);
                    await Assert.That(secondRelease.Notes).IsEqualTo(notUpdatedRelease.Notes);
                    await Assert.That(secondRelease.State).IsEqualTo(notUpdatedRelease.State);
                    await Assert.That(secondRelease.CreatedAt).IsEquivalentTo(notUpdatedRelease.CreatedAt);
                    await Assert.That(secondRelease.LastModifiedAt).IsEquivalentTo(notUpdatedRelease.LastModifiedAt!.Value);
                    if (emptyIssues)
                    {
                        await Assert.That(secondRelease.Issues).IsEmpty();
                    }
                    else
                    {
                        await Assert.That(secondRelease.Issues).IsNotEmpty();
                    }
                }
            }
        });
    }

    private async Task AssertReleaseNotUpdatedAsync(GraphQLResponse<UpdateReleaseResponse> response, IEnumerable<string> errors)
    {
        using (Assert.Multiple())
        {
            await Assert.That(response).IsNotNull();
            await Assert.That(response.Data.UpdateRelease.Errors).IsNotNull().And.IsNotEmpty();
            await Assert.That(response.Data.UpdateRelease.Release).IsNull();

            IEnumerable<string> resultErrors = response.Data.UpdateRelease.Errors.Select(error => error.Message);
            await Assert.That(resultErrors).IsEquivalentTo(errors);
        }

        await CheckDbContentAsync(async context =>
        {
            await Assert.That(context.Releases).IsEmpty();
        });
    }

    private static GraphQLRequest CreateDeleteRequest(Release expectedRelease)
    {
        GraphQLRequest mutationRequest = new GraphQLRequest
        {
            Query = $$"""
                      mutation deleteRelease
                      {
                          deleteRelease(input: {id: "{{expectedRelease.Id}}"})
                          {
                              release
                              {
                                  id
                                  title
                                  notes
                                  date
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
            OperationName = "deleteRelease"
        };
        return mutationRequest;
    }

    private async Task AssertDeletedReleaseAsync(GraphQLResponse<DeleteReleaseResponse> response, Release expectedRelease, Release? dbRelease = null)
    {
        using (Assert.Multiple())
        {
            await Assert.That(response).IsNotNull();
            await Assert.That(response.Errors).IsNull().Or.IsEmpty();
            ReleaseDto deletedRelease = response.Data.DeleteRelease.Release;
            await Assert.That(deletedRelease.Id).IsNotDefault();
            await Assert.That(deletedRelease.Title).IsEqualTo(expectedRelease.Title);
            await Assert.That(deletedRelease.Notes).IsEqualTo(expectedRelease.Notes);
            await Assert.That(deletedRelease.State).IsEqualTo(expectedRelease.State);
            await Assert.That(deletedRelease.CreatedAt).IsEquivalentTo(expectedRelease.CreatedAt);
            await Assert.That(deletedRelease.LastModifiedAt).IsEquivalentTo(expectedRelease.LastModifiedAt!.Value);
        }

        await CheckDbContentAsync(async context =>
        {
            using (Assert.Multiple())
            {
                await Assert.That(context.Releases.ToList())
                    .DoesNotContain(dbRelease1 => dbRelease1.Id.Equals(expectedRelease.Id));

                if (dbRelease is not null)
                {
                    await Assert.That(context.Releases.ToList())
                        .Contains(dbRelease1 => dbRelease1.Id.Equals(dbRelease.Id));
                }
            }
        });
    }

    private async Task AssertReleaseNotDeletedAsync(GraphQLResponse<DeleteReleaseResponse> response, IEnumerable<string> errors)
    {
        using (Assert.Multiple())
        {
            await Assert.That(response).IsNotNull();
            await Assert.That(response.Data.DeleteRelease.Errors).IsNotNull().And.IsNotEmpty();
            await Assert.That(response.Data.DeleteRelease.Release).IsNull();

            IEnumerable<string> resultErrors = response.Data.DeleteRelease.Errors.Select(error => error.Message);
            await Assert.That(resultErrors).IsEquivalentTo(errors);
        }

        await CheckDbContentAsync(async context =>
        {
            await Assert.That(context.Releases).IsEmpty();
        });
    }

    private static Release CreateRelease()
    {
        Faker<Release>? faker = new Faker<Release>()
            .RuleFor(release => release.Id, f => f.Random.Guid())
            .RuleFor(release => release.Title, f => f.Random.String2(1, 50, AllowedChars))
            .RuleFor(release => release.Notes, f => f.Random.String2(0, 255, AllowedChars).OrNull(f, 0.3f))
            .RuleFor(release => release.State, _ => ReleaseState.Open);
        return faker.Generate();
    }

    public static IEnumerable<Func<Release>> AddReleaseCases()
    {
        Faker<Release>? faker = new Faker<Release>()
            .RuleFor(release => release.Title, f => f.Random.String2(1, 50, AllowedChars))
            .RuleFor(release => release.Notes, f => f.Random.String2(0, 255, AllowedChars).OrNull(f, 0.3f))
            .RuleFor(release => release.State, _ => ReleaseState.Open);
        List<Release>? releases = faker.Generate(20);
        return releases.Select<Release, Func<Release>>(release => () => release);
    }

    // TODO: Release date tests
    public static IEnumerable<Func<(Release, IEnumerable<string>)>> InvalidAddReleaseCases()
    {
        yield return () => (new Release { Title = null!, Notes = null, State = ReleaseState.Open }, new List<string> { $"The value for {nameof(Release.Title)} is not set.", $"The value '' for {nameof(Release.Title)} is too short. The length of {nameof(Release.Title)} has to be between 1 and 255." });
        yield return () => (new Release { Title = "", Notes = null, State = ReleaseState.Open }, new List<string> { $"The value for {nameof(Release.Title)} is not set.", $"The value '' for {nameof(Release.Title)} is too short. The length of {nameof(Release.Title)} has to be between 1 and 255." });
        yield return () => (new Release { Title = "  \t ", Notes = null, State = ReleaseState.Open }, new List<string> { $"The value for {nameof(Release.Title)} is not set." });
        yield return () => (new Release { Title = new string('a', 256), Notes = null, State = ReleaseState.Open }, new List<string> { $"The value '{new string('a', 256)}' for {nameof(Release.Title)} is long short. The length of {nameof(Release.Title)} has to be between 1 and 255." });
        yield return () => (new Release { Title = "Valid", Notes = new string('a', 1001), State = ReleaseState.Open }, new List<string> { $"The value '{new string('a', 1001)}' for {nameof(Release.Notes)} is long short. The length of {nameof(Release.Notes)} has to be less than 1001." });
    }

    public static IEnumerable<Func<(Release, IEnumerable<string>)>> InvalidUpdateReleaseCases()
    {
        yield return () => (new Release { Title = null!, Notes = null, State = ReleaseState.Open }, new List<string> { $"The value for {nameof(Release.Title)} is not set.", $"The value '' for {nameof(Release.Title)} is too short. The length of {nameof(Release.Title)} has to be between 1 and 255." });
        yield return () => (new Release { Title = "", Notes = null, State = ReleaseState.Open }, new List<string> { $"The value for {nameof(Release.Title)} is not set.", $"The value '' for {nameof(Release.Title)} is too short. The length of {nameof(Release.Title)} has to be between 1 and 255." });
        yield return () => (new Release { Title = "  \t ", Notes = null, State = ReleaseState.Open }, new List<string> { $"The value for {nameof(Release.Title)} is not set." });
        yield return () => (new Release { Title = new string('a', 256), Notes = null, State = ReleaseState.Open }, new List<string> { $"The value '{new string('a', 256)}' for {nameof(Release.Title)} is long short. The length of {nameof(Release.Title)} has to be between 1 and 255." });
        yield return () => (new Release { Title = "Valid", Notes = new string('a', 1001), State = ReleaseState.Open }, new List<string> { $"The value '{new string('a', 1001)}' for {nameof(Release.Notes)} is long short. The length of {nameof(Release.Notes)} has to be less than 1001." });
        yield return () => (new Release { Title = "Valid", Notes = null, State = ReleaseState.Unknown }, new List<string> { $"The value for {nameof(Release.State)} is not set." });
    }
}
