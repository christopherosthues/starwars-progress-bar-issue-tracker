using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using StarWarsProgressBarIssueTracker.Infrastructure.Database;
using StarWarsProgressBarIssueTracker.Infrastructure.GitHub.Configuration;
using StarWarsProgressBarIssueTracker.Infrastructure.Gitlab.Configuration;
using Testcontainers.PostgreSql;
using TUnit.Core.Interfaces;

namespace StarWarsProgressBarIssueTracker.App.Tests.Integration.Setup;

public class IssueTrackerWebApplicationFactory : WebApplicationFactory<Program>, IAsyncInitializer
{
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder().Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            Quartz.Logging.LogContext.SetCurrentLogProvider(NullLoggerFactory.Instance);

            services.Configure<GitlabConfiguration>(opts =>
            {
                opts.GraphQLUrl = "http://localhost:8081/api/graphql";
            });

            services.Configure<GitHubConfiguration>(opts =>
            {
                opts.GraphQLUrl = "http://localhost:8082";
            });

            // services.ReplaceDbContext();

            var dbContextDescriptor =
                services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<IssueTrackerContext>));
            if (dbContextDescriptor is not null)
            {
                services.Remove(dbContextDescriptor);
            }

            services.AddDbContext<IssueTrackerContext>((_, options) =>
            {
                options.UseNpgsql(_postgreSqlContainer.GetConnectionString());
            });


            services.AddGitlabClient().ConfigureHttpClient(client =>
                {
                    var gitlabGraphQlUrl = new Uri("http://localhost:8081/api/graphql");
                    client.BaseAddress = new UriBuilder(Uri.UriSchemeHttps, gitlabGraphQlUrl.Host, gitlabGraphQlUrl.Port, gitlabGraphQlUrl.PathAndQuery).Uri;
                },
                httpClientBuilder => httpClientBuilder.ConfigurePrimaryHttpMessageHandler(() =>
                {
                    return new HttpClientHandler
                    {
                        ClientCertificateOptions = ClientCertificateOption.Manual,
                        ServerCertificateCustomValidationCallback = ((_, _, _, _) => true)
                    };
                })
            ).ConfigureWebSocketClient(client =>
            {
                var gitlabGraphQlUrl = new Uri("http://localhost:8081/api/graphql");
                client.Uri = new UriBuilder(Uri.UriSchemeWs, gitlabGraphQlUrl.Host, gitlabGraphQlUrl.Port, gitlabGraphQlUrl.PathAndQuery).Uri;
            });
        });
    }

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();
    }

    public override async ValueTask DisposeAsync()
    {
        await _postgreSqlContainer.StopAsync();
        await base.DisposeAsync();
    }
}
