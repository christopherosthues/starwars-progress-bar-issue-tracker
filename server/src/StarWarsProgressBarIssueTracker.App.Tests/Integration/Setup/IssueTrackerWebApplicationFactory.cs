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
        .WithResourceMapping("./Import/", "/opt/keycloak/data/import")
        // .WithResourceMapping("./Import/import.json", "/opt/keycloak/data/standalone/import")
        .WithCommand("--import-realm")
        .WithEnvironment(new Dictionary<string, string>
        {
            {"KC_BOOTSTRAP_ADMIN_USERNAME", "admin"},
            {"KC_BOOTSTRAP_ADMIN_PASSWORD", "admin"},
            {"KC_HTTP_ENABLED", "true"},
        })
        .Build();



    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            string? keycloakBaseAddress = _keycloakContainer.GetBaseAddress();
            configuration.AddInMemoryCollection(
                new Dictionary<string, string>
                {
                    { "Keycloak:Audience", "TestTracker" },
                    { "Keycloak:MetadataAddress", $"{keycloakBaseAddress}realms/{KeycloakConfig.Realm}/.well-known/openid-configuration" },
                    { "Keycloak:ValidIssuer", $"{keycloakBaseAddress}realms/{KeycloakConfig.Realm}" },
                    { "Keycloak:AuthorizationUrl", $"{keycloakBaseAddress}realms/{KeycloakConfig.Realm}/protocol/openid-connect/auth" },
                    { "Keycloak:ClientId", KeycloakConfig.ClientId },
                    { "Keycloak:ClientSecret", KeycloakConfig.TestClientSecret },
                    { "Keycloak:Authority", $"{keycloakBaseAddress}realms/{KeycloakConfig.Realm}" },
                    { "Keycloak:RegistrationUrl", $"{keycloakBaseAddress}admin/realms/{KeycloakConfig.Realm}/users" },
                    { "Keycloak:TokenUrl", $"{keycloakBaseAddress}realms/{KeycloakConfig.Realm}/protocol/openid-connect/token" },
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

    public string? KeycloakBaseAddress()
    {
        return _keycloakContainer.GetBaseAddress();
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
