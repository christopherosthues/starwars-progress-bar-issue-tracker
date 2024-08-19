using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using StarWarsProgressBarIssueTracker.App.Tests.Integration.Setup;
using StarWarsProgressBarIssueTracker.Infrastructure.Database;

namespace StarWarsProgressBarIssueTracker.App.Tests.Integration;

/// <summary>
/// Base class for all integration tests.
/// </summary>
public class IntegrationTestBase
{
    protected IssueTrackerWebApplicationFactory ApiFactory = default!;
    protected HttpClient HttpClient = default!;
    protected GraphQLHttpClient GraphQLClient = default!;

    [OneTimeSetUp]
    public async Task SetUpOnceBase()
    {
        AssertionOptions.AssertEquivalencyUsing(options =>
            options.Using<DateTime>(ctx =>
                    ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromMilliseconds(300)))
                .WhenTypeIs<DateTime>());
        AssertionOptions.AssertEquivalencyUsing(options =>
            options.Using<DateTime?>(ctx =>
                {
                    if (ctx.Expectation.HasValue)
                    {
                        ctx.Subject.Should().NotBeNull();
                        ctx.Subject.Should().BeCloseTo(ctx.Expectation.Value, TimeSpan.FromMilliseconds(300));
                    }
                    else
                    {
                        ctx.Subject.Should().BeNull();
                    }
                })
                .WhenTypeIs<DateTime?>());

        ApiFactory = new IssueTrackerWebApplicationFactory();
        await ApiFactory.StartAsync();
        HttpClient = ApiFactory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        GraphQLClient =
            new GraphQLHttpClient(new GraphQLHttpClientOptions { EndPoint = new Uri("http://localhost:8080/graphql"), },
                new SystemTextJsonSerializer(new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter() }
                }),
                HttpClient);
    }

    [SetUp]
    public async Task SetUpBase()
    {
        using var scope = ApiFactory.Services.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var dbContext = serviceProvider.GetRequiredService<IssueTrackerContext>();
        await SeedInitialDatabase(dbContext);
    }

    /// <summary>
    /// This method seeds the database before each test. This is the place where you can provide test specific data
    /// with which the database is seeded. The default implementation seeds the database with some mocked data.
    /// </summary>
    /// <param name="dbContext">The database context used to seed the database</param>
    protected virtual async Task SeedInitialDatabase(IssueTrackerContext dbContext)
    {
        // TODO: seed db with data
        await dbContext.SaveChangesAsync();
    }

    protected async Task SeedDatabaseAsync(Action<IssueTrackerContext> seed)
    {
        using var scope = ApiFactory.Services.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var dbContext = serviceProvider.GetRequiredService<IssueTrackerContext>();
        seed(dbContext);
        await dbContext.SaveChangesAsync();
    }

    protected void CheckDbContent(Action<IssueTrackerContext> check)
    {
        using var scope = ApiFactory.Services.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var dbContext = serviceProvider.GetRequiredService<IssueTrackerContext>();
        check(dbContext);
    }

    [TearDown]
    public async Task TearDownBase()
    {
        using var scope = ApiFactory.Services.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var dbContext = serviceProvider.GetRequiredService<IssueTrackerContext>();
        await ResetDatabase(dbContext);
    }

    /// <summary>
    /// This method reset the database after each test. This is the place where you can clear the database. The default
    /// implementation deletes all data from the database.
    /// </summary>
    /// <param name="dbContext">The database context used to reset the database</param>
    protected virtual async Task ResetDatabase(IssueTrackerContext dbContext)
    {
        dbContext.Issues.RemoveRange(dbContext.Issues);
        dbContext.IssueLinks.RemoveRange(dbContext.IssueLinks);
        dbContext.Labels.RemoveRange(dbContext.Labels);
        dbContext.Milestones.RemoveRange(dbContext.Milestones);
        dbContext.Appearances.RemoveRange(dbContext.Appearances);
        dbContext.Releases.RemoveRange(dbContext.Releases);
        dbContext.Vehicles.RemoveRange(dbContext.Vehicles);
        dbContext.Photos.RemoveRange(dbContext.Photos);
        dbContext.Translations.RemoveRange(dbContext.Translations);
        dbContext.Jobs.RemoveRange(dbContext.Jobs);
        dbContext.Tasks.RemoveRange(dbContext.Tasks);

        await dbContext.SaveChangesAsync();
    }

    [OneTimeTearDown]
    public async Task TearDownOnceBase()
    {
        HttpClient.Dispose();
        GraphQLClient.Dispose();
        await ApiFactory.StopAsync();
        await ApiFactory.DisposeAsync();
    }
}
