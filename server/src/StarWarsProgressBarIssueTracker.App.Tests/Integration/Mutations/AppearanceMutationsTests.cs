using Bogus;
using GraphQL;
using Microsoft.EntityFrameworkCore;
using StarWarsProgressBarIssueTracker.App.Tests.Helpers;
using StarWarsProgressBarIssueTracker.App.Tests.Helpers.GraphQL.Payloads.Appearances;
using StarWarsProgressBarIssueTracker.Domain.Vehicles;
using StarWarsProgressBarIssueTracker.TestHelpers;

namespace StarWarsProgressBarIssueTracker.App.Tests.Integration.Mutations;

[Category(TestCategory.Integration)]
[NotInParallel(NotInParallelTests.AppearanceMutation)]
public class AppearanceMutationsTests : IntegrationTestBase
{
    private const string AllowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ß_#%";
    private const string HexCodeColorChars = "0123456789abcdef";

    [Test]
    [MethodDataSource(nameof(AddAppearanceCases))]
    public async Task AddAppearanceShouldAddAppearance(Appearance expectedAppearance)
    {
        // Arrange
        await CheckDbContentAsync(async context =>
        {
            await Assert.That(context.Appearances).IsEmpty();
        });
        GraphQLRequest mutationRequest = CreateAddRequest(expectedAppearance);

        DateTime startTime = DateTime.UtcNow;

        // Act
        GraphQLResponse<AddAppearanceResponse> response = await CreateGraphQLClient().SendMutationAsync<AddAppearanceResponse>(mutationRequest);

        // Assert
        await AssertAddedAppearanceAsync(response, expectedAppearance, startTime);
    }

    [Test]
    [MethodDataSource(nameof(AddAppearanceCases))]
    public async Task AddAppearanceShouldAddAppearanceIfAppearancesAreNotEmpty(Appearance expectedAppearance)
    {
        // Arrange
        Appearance dbAppearance = new Appearance
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
            context.Appearances.Add(dbAppearance);
        });
        await CheckDbContentAsync(async context =>
        {
            await Assert.That(context.Appearances).Contains(dbAppearance);
        });
        GraphQLRequest mutationRequest = CreateAddRequest(expectedAppearance);

        DateTime startTime = DateTime.UtcNow;

        // Act
        GraphQLResponse<AddAppearanceResponse> response = await CreateGraphQLClient().SendMutationAsync<AddAppearanceResponse>(mutationRequest);

        // Assert
        await AssertAddedAppearanceAsync(response, expectedAppearance, startTime, dbAppearance);
    }

    [Test]
    [MethodDataSource(nameof(InvalidAddAppearanceCases))]
    public async Task AddAppearanceShouldNotAddAppearance((Appearance expectedAppearance, IEnumerable<string> errors) expectedResult)
    {
        // Arrange
        await CheckDbContentAsync(async context =>
        {
            await Assert.That(context.Appearances).IsEmpty();
        });
        GraphQLRequest mutationRequest = CreateAddRequest(expectedResult.expectedAppearance);

        // Act
        GraphQLResponse<AddAppearanceResponse> response = await CreateGraphQLClient().SendMutationAsync<AddAppearanceResponse>(mutationRequest);

        // Assert
        await AssertAppearanceNotAddedAsync(response, expectedResult.errors);
    }

    [Test]
    [MethodDataSource(nameof(AddAppearanceCases))]
    public async Task UpdateAppearanceShouldUpdateAppearance(Appearance expectedAppearance)
    {
        // Arrange
        Appearance dbAppearance = new Appearance
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
            context.Appearances.Add(dbAppearance);
        });
        expectedAppearance.Id = dbAppearance.Id;
        expectedAppearance.CreatedAt = dbAppearance.CreatedAt;
        await CheckDbContentAsync(async context =>
        {
            await Assert.That(context.Appearances).Contains(dbAppearance);
        });
        GraphQLRequest mutationRequest = CreateUpdateRequest(expectedAppearance);

        DateTime startTime = DateTime.UtcNow;

        // Act
        GraphQLResponse<UpdateAppearanceResponse> response = await CreateGraphQLClient().SendMutationAsync<UpdateAppearanceResponse>(mutationRequest);

        // Assert
        await AssertUpdatedAppearanceAsync(response, expectedAppearance, startTime);
    }

    [Test]
    [MethodDataSource(nameof(AddAppearanceCases))]
    public async Task UpdateAppearanceShouldUpdateAppearanceIfAppearancesAreNotEmpty(Appearance expectedAppearance)
    {
        // Arrange
        Appearance dbAppearance = new Appearance
        {
            Id = new Guid("87653DC5-B029-4BA6-959A-1FBFC48E2C81"),
            Title = "Title",
            Description = "Desc",
            Color = "#001122",
            TextColor = "#334455",
            LastModifiedAt = DateTime.UtcNow.AddDays(1)
        };
        Appearance dbAppearance2 = new Appearance
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
            context.Appearances.Add(dbAppearance);
            context.Appearances.Add(dbAppearance2);
        });
        expectedAppearance.Id = dbAppearance.Id;
        expectedAppearance.CreatedAt = dbAppearance.CreatedAt;
        await CheckDbContentAsync(async context =>
        {
            using (Assert.Multiple())
            {
                await Assert.That(context.Appearances).Contains(dbAppearance);
                await Assert.That(context.Appearances).Contains(dbAppearance2);
            }
        });
        GraphQLRequest mutationRequest = CreateUpdateRequest(expectedAppearance);

        DateTime startTime = DateTime.UtcNow;

        // Act
        GraphQLResponse<UpdateAppearanceResponse> response = await CreateGraphQLClient().SendMutationAsync<UpdateAppearanceResponse>(mutationRequest);

        // Assert
        await AssertUpdatedAppearanceAsync(response, expectedAppearance, startTime, dbAppearance, dbAppearance2);
    }

    [Test]
    [MethodDataSource(nameof(InvalidAddAppearanceCases))]
    public async Task UpdateAppearanceShouldNotUpdateAppearance((Appearance expectedAppearance, IEnumerable<string> errors) expectedResult)
    {
        // Arrange
        await CheckDbContentAsync(async context =>
        {
            await Assert.That(context.Appearances).IsEmpty();
        });
        GraphQLRequest mutationRequest = CreateUpdateRequest(expectedResult.expectedAppearance);

        // Act
        GraphQLResponse<UpdateAppearanceResponse> response = await CreateGraphQLClient().SendMutationAsync<UpdateAppearanceResponse>(mutationRequest);

        // Assert
        await AssertAppearanceNotUpdatedAsync(response, expectedResult.errors);
    }

    [Test]
    public async Task UpdateAppearanceShouldNotUpdateAppearanceIfAppearanceDoesNotExist()
    {
        // Arrange
        Appearance appearance = CreateAppearance();
        await CheckDbContentAsync(async context =>
        {
            await Assert.That(context.Appearances).IsEmpty();
        });
        GraphQLRequest mutationRequest = CreateUpdateRequest(appearance);

        // Act
        GraphQLResponse<UpdateAppearanceResponse> response = await CreateGraphQLClient().SendMutationAsync<UpdateAppearanceResponse>(mutationRequest);

        // Assert
        await AssertAppearanceNotUpdatedAsync(response, new List<string> { $"No {nameof(Appearance)} found with id '{appearance.Id}'." });
    }

    [Test]
    public async Task DeleteAppearanceShouldDeleteAppearance()
    {
        // Arrange
        Appearance appearance = CreateAppearance();
        Appearance dbAppearance = new Appearance
        {
            Id = appearance.Id,
            Title = appearance.Title,
            Description = appearance.Description,
            Color = appearance.Color,
            TextColor = appearance.TextColor,
            LastModifiedAt = DateTime.UtcNow.AddDays(1)
        };
        await SeedDatabaseAsync(context =>
        {
            context.Appearances.Add(dbAppearance);
        });
        appearance.CreatedAt = dbAppearance.CreatedAt;
        appearance.LastModifiedAt = dbAppearance.LastModifiedAt;
        await CheckDbContentAsync(async context =>
        {
            await Assert.That(context.Appearances).Contains(dbAppearance);
        });
        GraphQLRequest mutationRequest = CreateDeleteRequest(appearance);

        // Act
        GraphQLResponse<DeleteAppearanceResponse> response = await CreateGraphQLClient().SendMutationAsync<DeleteAppearanceResponse>(mutationRequest);

        // Assert
        await AssertDeletedAppearanceAsync(response, appearance);
    }

    [Test]
    public async Task DeleteAppearanceShouldDeleteAppearanceAndReferenceToVehicles()
    {
        // Arrange
        Appearance appearance = CreateAppearance();
        Appearance dbAppearance = new Appearance
        {
            Id = appearance.Id,
            Title = appearance.Title,
            Description = appearance.Description,
            Color = appearance.Color,
            TextColor = appearance.TextColor,
            LastModifiedAt = DateTime.UtcNow.AddDays(1)
        };
        Appearance dbAppearance2 = new Appearance
        {
            Id = new Guid("B961A621-9848-429A-8B44-B1AF1F0182CE"),
            Color = "#778899",
            TextColor = "#665544",
            Title = "Title 2"
        };
        Vehicle dbVehicle2 = new Vehicle
        {
            Id = new Guid("74AE8DD4-7669-4428-8E81-FB8A24A217A3"),
            EngineColor = EngineColor.Green,
            Appearances =
            [
                dbAppearance,
                dbAppearance2
            ]
        };
        await SeedDatabaseAsync(context =>
        {
            Vehicle dbVehicle = new Vehicle
            {
                Id = new Guid("87A2F9BF-CAB7-41D3-84F9-155135FA41D7"),
                EngineColor = EngineColor.Blue,
                Appearances = [dbAppearance]
            };
            context.Appearances.Add(dbAppearance);
            context.Vehicles.Add(dbVehicle);
            context.Vehicles.Add(dbVehicle2);
        });
        appearance.CreatedAt = dbAppearance.CreatedAt;
        appearance.LastModifiedAt = dbAppearance.LastModifiedAt;
        await CheckDbContentAsync(async context =>
        {
            await Assert.That(context.Appearances).Contains(dbAppearance);
        });
        GraphQLRequest mutationRequest = CreateDeleteRequest(appearance);

        // Act
        GraphQLResponse<DeleteAppearanceResponse> response = await CreateGraphQLClient().SendMutationAsync<DeleteAppearanceResponse>(mutationRequest);

        // Assert
        await AssertDeletedAppearanceAsync(response, appearance);
        await CheckDbContentAsync(async context =>
        {
            using (Assert.Multiple())
            {
                List<Vehicle> dbVehicles = context.Vehicles.Include(dbVehicle => dbVehicle.Appearances).ToList();
                foreach (Vehicle dbVehicle in dbVehicles)
                {
                    await Assert.That(dbVehicle.Appearances).DoesNotContain(dbAppearance);
                }

                await Assert.That(dbVehicles.First(dbVehicle => dbVehicle.Id.Equals(dbVehicle2.Id)).Appearances).Contains(dbAppearance2);
            }
        });
    }

    [Test]
    public async Task DeleteAppearanceShouldDeleteAppearanceIfAppearancesIsNotEmpty()
    {
        // Arrange
        Appearance appearance = CreateAppearance();
        Appearance dbAppearance = new Appearance
        {
            Id = appearance.Id,
            Title = appearance.Title,
            Description = appearance.Description,
            Color = appearance.Color,
            TextColor = appearance.TextColor,
            LastModifiedAt = DateTime.UtcNow.AddDays(1)
        };
        Appearance dbAppearance2 = new Appearance
        {
            Id = new Guid("0609F93C-CBCC-4650-BA4C-B8D5FF93A877"),
            Title = "Title 2",
            Description = "Desc 2",
            Color = "#221100",
            TextColor = "#554433",
            CreatedAt = DateTime.UtcNow.AddDays(-3),
            LastModifiedAt = DateTime.UtcNow.AddDays(-2)
        };


        await SeedDatabaseAsync(context =>
        {
            context.Appearances.Add(dbAppearance);
            context.Appearances.Add(dbAppearance2);
        });
        appearance.CreatedAt = dbAppearance.CreatedAt;
        appearance.LastModifiedAt = dbAppearance.LastModifiedAt;
        await CheckDbContentAsync(async context =>
        {
            using (Assert.Multiple())
            {
                await Assert.That(context.Appearances).Contains(dbAppearance);
                await Assert.That(context.Appearances).Contains(dbAppearance2);
            }
        });
        GraphQLRequest mutationRequest = CreateDeleteRequest(appearance);

        // Act
        GraphQLResponse<DeleteAppearanceResponse> response = await CreateGraphQLClient().SendMutationAsync<DeleteAppearanceResponse>(mutationRequest);

        // Assert
        await AssertDeletedAppearanceAsync(response, appearance, dbAppearance2);
    }

    [Test]
    public async Task DeleteAppearanceShouldNotDeleteAppearanceIfAppearanceDoesNotExist()
    {
        // Arrange
        Appearance appearance = CreateAppearance();
        await CheckDbContentAsync(async context =>
        {
            await Assert.That(context.Appearances).IsEmpty();
        });
        GraphQLRequest mutationRequest = CreateDeleteRequest(appearance);

        // Act
        GraphQLResponse<DeleteAppearanceResponse> response = await CreateGraphQLClient().SendMutationAsync<DeleteAppearanceResponse>(mutationRequest);

        // Assert
        await AssertAppearanceNotDeletedAsync(response, new List<string> { $"No {nameof(Appearance)} found with id '{appearance.Id}'." });
    }

    private static GraphQLRequest CreateAddRequest(Appearance expectedAppearance)
    {
        string descriptionParameter = expectedAppearance.Description != null
            ? $"""
               , description: "{expectedAppearance.Description}"
               """
            : string.Empty;
        GraphQLRequest mutationRequest = new GraphQLRequest
        {
            Query = $$"""
                      mutation addAppearance
                      {
                          addAppearance(input: {title: "{{expectedAppearance.Title}}", color: "{{expectedAppearance.Color}}", textColor: "{{expectedAppearance.TextColor}}"{{descriptionParameter}}})
                          {
                              appearance
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
            OperationName = "addAppearance"
        };
        return mutationRequest;
    }

    private async Task AssertAddedAppearanceAsync(GraphQLResponse<AddAppearanceResponse> response, Appearance expectedAppearance,
        DateTime startTime, Appearance? dbAppearance = null)
    {
        DateTime endTime = DateTime.UtcNow;
        Appearance? addedAppearance;
        using (Assert.Multiple())
        {
            await Assert.That(response).IsNotNull();
            await Assert.That(response.Errors).IsNull().Or.IsEmpty();
            addedAppearance = response.Data.AddAppearance.Appearance;
            await Assert.That(addedAppearance.Id).IsNotDefault();
            await Assert.That(addedAppearance.Title).IsEqualTo(expectedAppearance.Title);
            await Assert.That(addedAppearance.Description).IsEqualTo(expectedAppearance.Description);
            await Assert.That(addedAppearance.Color).IsEqualTo(expectedAppearance.Color);
            await Assert.That(addedAppearance.TextColor).IsEqualTo(expectedAppearance.TextColor);
            await Assert.That(addedAppearance.CreatedAt).IsGreaterThanOrEqualTo(startTime).And
                .IsLessThanOrEqualTo(endTime);
            await Assert.That(addedAppearance.LastModifiedAt).IsNull();
        }

        await CheckDbContentAsync(async context =>
        {
            using (Assert.Multiple())
            {
                if (dbAppearance is not null)
                {
                    await Assert.That(context.Appearances.ToList())
                        .Contains(dbAppearance1 => dbAppearance1.Id.Equals(dbAppearance.Id));
                }
                Appearance addedDbAppearance = context.Appearances.First(dbAppearance1 => dbAppearance1.Id.Equals(addedAppearance.Id));
                await Assert.That(addedDbAppearance).IsNotNull();
                await Assert.That(addedDbAppearance.Id).IsNotDefault().And.IsEqualTo(addedAppearance.Id);
                await Assert.That(addedDbAppearance.Title).IsEqualTo(expectedAppearance.Title);
                await Assert.That(addedDbAppearance.Description).IsEqualTo(expectedAppearance.Description);
                await Assert.That(addedDbAppearance.Color).IsEqualTo(expectedAppearance.Color);
                await Assert.That(addedDbAppearance.TextColor).IsEqualTo(expectedAppearance.TextColor);
                await Assert.That(addedDbAppearance.CreatedAt).IsGreaterThanOrEqualTo(startTime).And
                    .IsLessThanOrEqualTo(endTime);
                await Assert.That(addedDbAppearance.LastModifiedAt).IsNull();
            }
        });
    }

    private async Task AssertAppearanceNotAddedAsync(GraphQLResponse<AddAppearanceResponse> response, IEnumerable<string> errors)
    {
        using (Assert.Multiple())
        {
            await Assert.That(response).IsNotNull();
            await Assert.That(response.Data.AddAppearance.Errors).IsNotNull().And.IsNotEmpty();
            await Assert.That(response.Data.AddAppearance.Appearance).IsNull();

            IEnumerable<string> resultErrors = response.Data.AddAppearance.Errors.Select(error => error.Message);
            await Assert.That(resultErrors).IsEquivalentTo(errors);
        }

        await CheckDbContentAsync(async context =>
        {
            await Assert.That(context.Appearances).IsEmpty();
        });
    }

    private static GraphQLRequest CreateUpdateRequest(Appearance expectedAppearance)
    {
        string descriptionParameter = expectedAppearance.Description != null
            ? $"""
               , description: "{expectedAppearance.Description}"
               """
            : string.Empty;
        GraphQLRequest mutationRequest = new GraphQLRequest
        {
            Query = $$"""
                      mutation updateAppearance
                      {
                          updateAppearance(input: {id: "{{expectedAppearance.Id}}", title: "{{expectedAppearance.Title}}", color: "{{expectedAppearance.Color}}", textColor: "{{expectedAppearance.TextColor}}"{{descriptionParameter}}})
                          {
                              appearance
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
            OperationName = "updateAppearance"
        };
        return mutationRequest;
    }

    private async Task AssertUpdatedAppearanceAsync(GraphQLResponse<UpdateAppearanceResponse> response, Appearance expectedAppearance,
        DateTime startTime, Appearance? dbAppearance = null, Appearance? notUpdatedAppearance = null)
    {
        DateTime endTime = DateTime.UtcNow;
        Appearance? updatedAppearance;
        using (Assert.Multiple())
        {
            await Assert.That(response).IsNotNull();
            await Assert.That(response.Errors).IsNull().Or.IsEmpty();
            updatedAppearance = response.Data.UpdateAppearance.Appearance;
            await Assert.That(updatedAppearance.Id).IsEqualTo(expectedAppearance.Id);
            await Assert.That(updatedAppearance.Title).IsEqualTo(expectedAppearance.Title);
            await Assert.That(updatedAppearance.Description).IsEqualTo(expectedAppearance.Description);
            await Assert.That(updatedAppearance.Color).IsEqualTo(expectedAppearance.Color);
            await Assert.That(updatedAppearance.TextColor).IsEqualTo(expectedAppearance.TextColor);
            await Assert.That(updatedAppearance.CreatedAt).IsEqualTo(expectedAppearance.CreatedAt);
            await Assert.That(updatedAppearance.LastModifiedAt!.Value).IsGreaterThanOrEqualTo(startTime).And
                .IsLessThanOrEqualTo(endTime);
        }

        await CheckDbContentAsync(async context =>
        {
            using (Assert.Multiple())
            {
                if (dbAppearance is not null)
                {
                    await Assert.That(context.Appearances.ToList())
                        .Contains(dbAppearance1 => dbAppearance1.Id.Equals(dbAppearance.Id));
                }
                Appearance updatedDbAppearance = context.Appearances.First(dbAppearance1 => dbAppearance1.Id.Equals(updatedAppearance.Id));
                await Assert.That(updatedDbAppearance).IsNotNull();
                await Assert.That(updatedDbAppearance.Id).IsNotDefault().And.IsEqualTo(updatedAppearance.Id);
                await Assert.That(updatedDbAppearance.Title).IsEqualTo(expectedAppearance.Title);
                await Assert.That(updatedDbAppearance.Description).IsEqualTo(expectedAppearance.Description);
                await Assert.That(updatedDbAppearance.Color).IsEqualTo(expectedAppearance.Color);
                await Assert.That(updatedDbAppearance.TextColor).IsEqualTo(expectedAppearance.TextColor);
                await Assert.That(updatedDbAppearance.CreatedAt).IsEqualTo(expectedAppearance.CreatedAt);
                await Assert.That(updatedDbAppearance.LastModifiedAt!.Value).IsGreaterThanOrEqualTo(startTime).And
                    .IsLessThanOrEqualTo(endTime);

                if (notUpdatedAppearance is not null)
                {
                    Appearance? secondAppearance =
                        context.Appearances.FirstOrDefault(appearance => appearance.Id.Equals(notUpdatedAppearance.Id));
                    await Assert.That(secondAppearance).IsNotNull();
                    await Assert.That(secondAppearance!.Id).IsNotDefault().And.IsEqualTo(notUpdatedAppearance.Id);
                    await Assert.That(secondAppearance.Title).IsEqualTo(notUpdatedAppearance.Title);
                    await Assert.That(secondAppearance.Description).IsEqualTo(notUpdatedAppearance.Description);
                    await Assert.That(secondAppearance.Color).IsEqualTo(notUpdatedAppearance.Color);
                    await Assert.That(secondAppearance.TextColor).IsEqualTo(notUpdatedAppearance.TextColor);
                    await Assert.That(secondAppearance.CreatedAt).IsEqualTo(notUpdatedAppearance.CreatedAt);
                    await Assert.That(secondAppearance.LastModifiedAt).IsEqualTo(notUpdatedAppearance.LastModifiedAt!.Value);
                }
            }
        });
    }

    private async Task AssertAppearanceNotUpdatedAsync(GraphQLResponse<UpdateAppearanceResponse> response, IEnumerable<string> errors)
    {
        using (Assert.Multiple())
        {
            await Assert.That(response).IsNotNull();
            await Assert.That(response.Data.UpdateAppearance.Errors).IsNotNull().And.IsNotEmpty();
            await Assert.That(response.Data.UpdateAppearance.Appearance).IsNull();

            IEnumerable<string> resultErrors = response.Data.UpdateAppearance.Errors.Select(error => error.Message);
            await Assert.That(resultErrors).IsEquivalentTo(errors);
        }

        await CheckDbContentAsync(async context =>
        {
            await Assert.That(context.Appearances).IsEmpty();
        });
    }

    private static GraphQLRequest CreateDeleteRequest(Appearance expectedAppearance)
    {
        GraphQLRequest mutationRequest = new GraphQLRequest
        {
            Query = $$"""
                      mutation deleteAppearance
                      {
                          deleteAppearance(input: {id: "{{expectedAppearance.Id}}"})
                          {
                              appearance
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
            OperationName = "deleteAppearance"
        };
        return mutationRequest;
    }

    private async Task AssertDeletedAppearanceAsync(GraphQLResponse<DeleteAppearanceResponse> response, Appearance expectedAppearance, Appearance? dbAppearance = null)
    {
        using (Assert.Multiple())
        {
            await Assert.That(response).IsNotNull();
            await Assert.That(response.Errors).IsNull().Or.IsEmpty();
            Appearance deletedAppearance = response.Data.DeleteAppearance.Appearance;
            await Assert.That(deletedAppearance.Id).IsNotDefault();
            await Assert.That(deletedAppearance.Title).IsEqualTo(expectedAppearance.Title);
            await Assert.That(deletedAppearance.Description).IsEqualTo(expectedAppearance.Description);
            await Assert.That(deletedAppearance.Color).IsEqualTo(expectedAppearance.Color);
            await Assert.That(deletedAppearance.TextColor).IsEqualTo(expectedAppearance.TextColor);
            await Assert.That(deletedAppearance.CreatedAt).IsEqualTo(expectedAppearance.CreatedAt);
            await Assert.That(deletedAppearance.LastModifiedAt).IsEqualTo(expectedAppearance.LastModifiedAt!.Value);
        }

        await CheckDbContentAsync(async context =>
        {
            using (Assert.Multiple())
            {
                await Assert.That(context.Appearances.ToList())
                    .DoesNotContain(dbAppearance1 => dbAppearance1.Id.Equals(expectedAppearance.Id));

                if (dbAppearance is not null)
                {
                    await Assert.That(context.Appearances.ToList())
                        .Contains(dbAppearance1 => dbAppearance1.Id.Equals(dbAppearance.Id));
                }
            }
        });
    }

    private async Task AssertAppearanceNotDeletedAsync(GraphQLResponse<DeleteAppearanceResponse> response, IEnumerable<string> errors)
    {
        using (Assert.Multiple())
        {
            await Assert.That(response).IsNotNull();
            await Assert.That(response.Data.DeleteAppearance.Errors).IsNotNull().And.IsNotEmpty();
            await Assert.That(response.Data.DeleteAppearance.Appearance).IsNull();

            IEnumerable<string> resultErrors = response.Data.DeleteAppearance.Errors.Select(error => error.Message);
            await Assert.That(resultErrors).IsEquivalentTo(errors);
        }

        await CheckDbContentAsync(async context =>
        {
            await Assert.That(context.Appearances).IsEmpty();
        });
    }

    private static Appearance CreateAppearance()
    {
        Faker<Appearance>? faker = new Faker<Appearance>()
            .RuleFor(appearance => appearance.Id, f => f.Random.Guid())
            .RuleFor(appearance => appearance.Title, f => f.Random.String2(1, 50, AllowedChars))
            .RuleFor(appearance => appearance.Description, f => f.Random.String2(0, 255, AllowedChars).OrNull(f, 0.3f))
            .RuleFor(appearance => appearance.Color, f => "#" + f.Random.String2(6, 6, HexCodeColorChars))
            .RuleFor(appearance => appearance.TextColor, f => "#" + f.Random.String2(6, 6, HexCodeColorChars));
        return faker.Generate();
    }

    public static IEnumerable<Func<Appearance>> AddAppearanceCases()
    {
        Faker<Appearance>? faker = new Faker<Appearance>()
            .RuleFor(appearance => appearance.Title, f => f.Random.String2(1, 50, AllowedChars))
            .RuleFor(appearance => appearance.Description, f => f.Random.String2(0, 255, AllowedChars).OrNull(f, 0.3f))
            .RuleFor(appearance => appearance.Color, f => "#" + f.Random.String2(6, 6, HexCodeColorChars))
            .RuleFor(appearance => appearance.TextColor, f => "#" + f.Random.String2(6, 6, HexCodeColorChars));
        List<Appearance>? appearances = faker.Generate(20);
        return appearances.Select<Appearance, Func<Appearance>>(appearance => () => appearance);
    }

    public static IEnumerable<Func<(Appearance, IEnumerable<string>)>> InvalidAddAppearanceCases()
    {
        yield return () => (new Appearance { Title = null!, Description = null, Color = "#001122", TextColor = "#334455" }, new List<string> { $"The value for {nameof(Appearance.Title)} is not set.", $"The value '' for {nameof(Appearance.Title)} is too short. The length of {nameof(Appearance.Title)} has to be between 1 and 50." });
        yield return () => (new Appearance { Title = "", Description = null, Color = "#001122", TextColor = "#334455" }, new List<string> { $"The value for {nameof(Appearance.Title)} is not set.", $"The value '' for {nameof(Appearance.Title)} is too short. The length of {nameof(Appearance.Title)} has to be between 1 and 50." });
        yield return () => (new Appearance { Title = "  \t ", Description = null, Color = "#001122", TextColor = "#334455" }, new List<string> { $"The value for {nameof(Appearance.Title)} is not set." });
        yield return () => (new Appearance { Title = new string('a', 51), Description = null, Color = "#001122", TextColor = "#334455" }, new List<string> { $"The value 'aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa' for {nameof(Appearance.Title)} is long short. The length of {nameof(Appearance.Title)} has to be between 1 and 50." });
        yield return () => (new Appearance { Title = "Valid", Description = new string('a', 256), Color = "#001122", TextColor = "#334455" }, new List<string> { $"The value 'aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa' for {nameof(Appearance.Description)} is long short. The length of {nameof(Appearance.Description)} has to be less than 256." });
        yield return () => (new Appearance { Title = "Valid", Description = null, Color = null!, TextColor = "#334455" }, new List<string> { $"The value for {nameof(Appearance.Color)} is not set.", $"The value '' for field {nameof(Appearance.Color)} has a wrong format. Only colors in RGB hex format with 6 digits are allowed." });
        yield return () => (new Appearance { Title = "Valid", Description = null, Color = "01122", TextColor = "#334455" }, new List<string> { $"The value '01122' for field {nameof(Appearance.Color)} has a wrong format. Only colors in RGB hex format with 6 digits are allowed." });
        yield return () => (new Appearance { Title = "Valid", Description = null, Color = "#01122", TextColor = "#334455" }, new List<string> { $"The value '#01122' for field {nameof(Appearance.Color)} has a wrong format. Only colors in RGB hex format with 6 digits are allowed." });
        yield return () => (new Appearance { Title = "Valid", Description = null, Color = "001122", TextColor = "#334455" }, new List<string> { $"The value '001122' for field {nameof(Appearance.Color)} has a wrong format. Only colors in RGB hex format with 6 digits are allowed." });
        yield return () => (new Appearance { Title = "Valid", Description = null, Color = "", TextColor = "#334455" }, new List<string> { $"The value for {nameof(Appearance.Color)} is not set.", $"The value '' for field {nameof(Appearance.Color)} has a wrong format. Only colors in RGB hex format with 6 digits are allowed." });
        yield return () => (new Appearance { Title = "Valid", Description = null, Color = " ", TextColor = "#334455" }, new List<string> { $"The value for {nameof(Appearance.Color)} is not set.", $"The value ' ' for field {nameof(Appearance.Color)} has a wrong format. Only colors in RGB hex format with 6 digits are allowed." });
        yield return () => (new Appearance { Title = "Valid", Description = null, Color = "g", TextColor = "#334455" }, new List<string> { $"The value 'g' for field {nameof(Appearance.Color)} has a wrong format. Only colors in RGB hex format with 6 digits are allowed." });
        yield return () => (new Appearance { Title = "Valid", Description = null, TextColor = null!, Color = "#334455" }, new List<string> { $"The value for {nameof(Appearance.TextColor)} is not set.", $"The value '' for field {nameof(Appearance.TextColor)} has a wrong format. Only colors in RGB hex format with 6 digits are allowed." });
        yield return () => (new Appearance { Title = "Valid", Description = null, TextColor = "01122", Color = "#334455" }, new List<string> { $"The value '01122' for field {nameof(Appearance.TextColor)} has a wrong format. Only colors in RGB hex format with 6 digits are allowed." });
        yield return () => (new Appearance { Title = "Valid", Description = null, TextColor = "#01122", Color = "#334455" }, new List<string> { $"The value '#01122' for field {nameof(Appearance.TextColor)} has a wrong format. Only colors in RGB hex format with 6 digits are allowed." });
        yield return () => (new Appearance { Title = "Valid", Description = null, TextColor = "001122", Color = "#334455" }, new List<string> { $"The value '001122' for field {nameof(Appearance.TextColor)} has a wrong format. Only colors in RGB hex format with 6 digits are allowed." });
        yield return () => (new Appearance { Title = "Valid", Description = null, TextColor = "", Color = "#334455" }, new List<string> { $"The value for {nameof(Appearance.TextColor)} is not set.", $"The value '' for field {nameof(Appearance.TextColor)} has a wrong format. Only colors in RGB hex format with 6 digits are allowed." });
        yield return () => (new Appearance { Title = "Valid", Description = null, TextColor = " ", Color = "#334455" }, new List<string> { $"The value for {nameof(Appearance.TextColor)} is not set.", $"The value ' ' for field {nameof(Appearance.TextColor)} has a wrong format. Only colors in RGB hex format with 6 digits are allowed." });
        yield return () => (new Appearance { Title = "Valid", Description = null, TextColor = "g", Color = "#334455" }, new List<string> { $"The value 'g' for field {nameof(Appearance.TextColor)} has a wrong format. Only colors in RGB hex format with 6 digits are allowed." });
    }
}
