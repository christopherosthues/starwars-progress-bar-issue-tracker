using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using StarWarsProgressBarIssueTracker.App.Tests.Helpers;
using StarWarsProgressBarIssueTracker.Infrastructure.Database;
using StarWarsProgressBarIssueTracker.Infrastructure.GitHub.Configuration;
using StarWarsProgressBarIssueTracker.Infrastructure.Gitlab.Configuration;
using Testcontainers.Keycloak;
using Testcontainers.PostgreSql;
using TUnit.Core.Interfaces;

namespace StarWarsProgressBarIssueTracker.App.Tests.Integration.Setup;

public class IssueTrackerWebApplicationFactory : WebApplicationFactory<Program>, IAsyncInitializer
{
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder().Build();
    private readonly KeycloakContainer _keycloakContainer = new KeycloakBuilder()
        .WithImage("quay.io/keycloak/keycloak:latest")
        .WithName(KeycloakConfig.KeycloakHostName)
        .WithPortBinding(KeycloakConfig.Port, 8080)
        .WithResourceMapping("./Import/", "/opt/keycloak/data/import")
        // .WithResourceMapping("./Import/import.json", "/opt/keycloak/data/standalone/import")
        .WithCommand("--import-realm")
        .Build();



    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(
                new Dictionary<string, string>
                {
                    { "Keycloak:Audience", "Test" },
                    { "Keycloak:MetadataAddress", $"http://{KeycloakConfig.KeycloakHostName}:8080/realms/{KeycloakConfig.Realm}/.well-known/openid-configuration" },
                    { "Keycloak:ValidIssuer", $"http://localhost:{KeycloakConfig.Port}/realms/{KeycloakConfig.Realm}" },
                    { "Keycloak:AuthorizationUrl", $"http://localhost:{KeycloakConfig.Port}/realms/{KeycloakConfig.Realm}/protocol/openid-connect/auth" },
                }!
            );
        });

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

            // TODO: configure Keycloak settings
            // TODO: Configure authentication

            ServiceDescriptor? dbContextDescriptor =
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
                    Uri gitlabGraphQlUrl = new Uri("http://localhost:8081/api/graphql");
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
                Uri gitlabGraphQlUrl = new Uri("http://localhost:8081/api/graphql");
                client.Uri = new UriBuilder(Uri.UriSchemeWs, gitlabGraphQlUrl.Host, gitlabGraphQlUrl.Port, gitlabGraphQlUrl.PathAndQuery).Uri;
            });
        });
    }

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();
        await _keycloakContainer.StartAsync();
    }

    public override async ValueTask DisposeAsync()
    {
        await _postgreSqlContainer.StopAsync();
        await _keycloakContainer.StopAsync();
        await base.DisposeAsync();
    }
}
