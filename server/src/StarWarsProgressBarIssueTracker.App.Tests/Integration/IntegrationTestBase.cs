using System.Text.Json;
using System.Text.Json.Serialization;
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
    [ClassDataSource<IssueTrackerWebApplicationFactory>(Shared = SharedType.PerClass)]
    public required IssueTrackerWebApplicationFactory ApiFactory { get; set; }

    protected TimeSpan Tolerance => TimeSpan.FromMilliseconds(300);

    protected GraphQLHttpClient CreateGraphQLClient()
    {
        HttpClient httpClient = ApiFactory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        GraphQLHttpClient graphQLClient =
            new(new GraphQLHttpClientOptions { EndPoint = new Uri("http://localhost:8080/graphql"), },
                new SystemTextJsonSerializer(new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter() }
                }),
                httpClient);
        return graphQLClient;
    }

    protected async Task SeedDatabaseAsync(Action<IssueTrackerContext> seed)
    {
        using IServiceScope scope = ApiFactory.Services.CreateScope();
        IServiceProvider serviceProvider = scope.ServiceProvider;
        IssueTrackerContext dbContext = serviceProvider.GetRequiredService<IssueTrackerContext>();
        seed(dbContext);
        await dbContext.SaveChangesAsync();
    }

    protected async Task CheckDbContentAsync(Func<IssueTrackerContext, Task> checkAsync)
    {
        using IServiceScope scope = ApiFactory.Services.CreateScope();
        IServiceProvider serviceProvider = scope.ServiceProvider;
        IssueTrackerContext dbContext = serviceProvider.GetRequiredService<IssueTrackerContext>();
        await checkAsync(dbContext);
    }

    [After(Test)]
    public async Task TearDownBase()
    {
        using IServiceScope scope = ApiFactory.Services.CreateScope();
        IServiceProvider serviceProvider = scope.ServiceProvider;
        IssueTrackerContext dbContext = serviceProvider.GetRequiredService<IssueTrackerContext>();
        await ResetDatabase(dbContext);

        // GraphQLClient.Dispose();
        // await ApiFactory.DisposeAsync();
    }

    /// <summary>
    /// This method reset the database after each test. This is the place where you can clear the database. The default
    /// implementation deletes all data from the database.
    /// </summary>
    /// <param name="dbContext">The database context used to reset the database</param>
    private static async Task ResetDatabase(IssueTrackerContext dbContext)
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
}
