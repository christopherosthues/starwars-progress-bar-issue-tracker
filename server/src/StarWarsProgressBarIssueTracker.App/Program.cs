using System.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using StarWarsProgressBarIssueTracker.App.Authorization;
using StarWarsProgressBarIssueTracker.App.Extensions;
using StarWarsProgressBarIssueTracker.App.Mutations;
using StarWarsProgressBarIssueTracker.App.Queries;
using StarWarsProgressBarIssueTracker.Infrastructure;
using StarWarsProgressBarIssueTracker.Infrastructure.Gitlab;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders().AddConsole();

IConfigurationSection keycloakConfig = builder.Configuration.GetSection("Keycloak");
builder.Services.AddKeycloakAuthorization(keycloakConfig);

IConfigurationSection gitlabConfig = builder.Configuration.GetSection("Gitlab");
string gitlabToken = gitlabConfig.GetValue<string>("Token")!;
Uri gitlabGraphQlUrl = new Uri(gitlabConfig.GetValue<string>("GraphQLUrl") ?? string.Empty);
builder.Services.AddGitlabClient().ConfigureHttpClient(client =>
{
    client.BaseAddress = GetGraphQLUri(gitlabGraphQlUrl);
    client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", gitlabToken);
}
     // httpClientBuilder => httpClientBuilder.AddPolly()
     ).ConfigureWebSocketClient(client =>
{
    client.Uri = GetGraphQLStreamingUri(gitlabGraphQlUrl);
    // client.Socket.Options.SetRequestHeader("Authorization", $"Bearer {gitlabToken}");
});
builder.Services.AddGitHubClient();

string connectionString = builder.Configuration.GetConnectionString("IssueTrackerContext")!;
builder.Services.RegisterDbContext(connectionString);

builder.Services
    .AddGraphQLServer()
    .AddAuthorization()
    .AddMutationConventions(applyToAllMutations: true)
    // .AddQueryConventions()
    .AddQueryType<IssueTrackerQueries>()
    .AddMutationType<IssueTrackerMutations>()
    .AddAppTypes()
    .AddPagingArguments()
    .AddSorting()
    .ModifyPagingOptions(options =>
    {
        options.DefaultPageSize = 50;
        options.MaxPageSize = 100;
        options.IncludeTotalCount = true;
    });
    // .AddErrorInterfaceType<IUserError>(); TODO: User error

// builder.Services.AddResiliencePipeline("job-pipeline", pipelineBuilder =>
// {
//     pipelineBuilder.AddRetry(new Polly.Retry.RetryStrategyOptions()
//     {
//         Delay = TimeSpan.FromSeconds(1),
//         MaxRetryAttempts = 3,
//         UseJitter = true,
//         BackoffType = DelayBackoffType.Exponential
//     })
//     .AddTimeout(TimeSpan.FromSeconds(5))
//     .AddCircuitBreaker(new Polly.CircuitBreaker.CircuitBreakerStrategyOptions());
// });

// builder.Services.AddQuartz(q =>
// {
//     // var jobKey = new JobKey(nameof(JobScheduler));
//     // q.AddJob<JobScheduler>(opts => opts.WithIdentity(jobKey));
//
//     // q.AddTrigger(opts => opts.ForJob(jobKey)
//     //                          .WithIdentity($"{nameof(JobScheduler)}-trigger")
//     //                          .WithCronSchedule("*/30 * * * * ?"));
//
//     // var jobKey2 = new JobKey(nameof(GitlabSynchronizationJobScheduler));
//     // q.AddJob<GitlabSynchronizationJobScheduler>(opts => opts.WithIdentity(jobKey2));
//
//     // q.AddTrigger(opts => opts.ForJob(jobKey2)
//     //                          .WithIdentity($"{nameof(GitlabSynchronizationJobScheduler)}-trigger")
//     //                          .WithCronSchedule("0 */1 * * * ?"));
//
//     // var jobKey3 = new JobKey(nameof(GitHubSynchronizationJobScheduler));
//     // q.AddJob<GitHubSynchronizationJobScheduler>(opts => opts.WithIdentity(jobKey3));
//
//     // q.AddTrigger(opts => opts.ForJob(jobKey3)
//     //                          .WithIdentity($"{nameof(GitHubSynchronizationJobScheduler)}-trigger")
//     //                          .WithCronSchedule("0 */1 * * * ?"));
// });

// builder.Services.AddQuartzServer(options =>
// {
//     options.WaitForJobsToComplete = true;
// });

builder.Services.AddJobs();
builder.Services.AddRepositories();
builder.Services.AddIssueTrackerConfigurations(builder.Configuration);
builder.Services.AddIssueTrackerServices();
builder.Services.AddMappers();
builder.Services.AddGraphQlQueries();
builder.Services.AddGraphQlMutations();
builder.Services.AddGitlabServices();
builder.Services.AddHttpClient();

builder.Services.AddCors(corsOptions =>
    corsOptions.AddDefaultPolicy(corsPolicyBuilder =>
        corsPolicyBuilder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.CustomSchemaIds(id => id.FullName!.Replace("+", "-"));
    o.AddSecurityDefinition("Keycloak",
        new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows
            {
                Implicit = new OpenApiOAuthFlow
                {
                    AuthorizationUrl =
                        new Uri(keycloakConfig.GetValue<string>("AuthorizationUrl")!),
                    Scopes = new Dictionary<string, string>
                    {
                        { "openid", "openid" }, { "profile", "profile" }
                    }
                }
            }
        });
    var openApiSecurityRequirement = new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Id = "Keycloak", Type = ReferenceType.SecurityScheme },
                In = ParameterLocation.Header,
                Name = "Bearer",
                Scheme = "Bearer",
            }
            ,[]
        }
    };
    o.AddSecurityRequirement(openApiSecurityRequirement);
});

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    await app.Services.ConfigureDatabaseAsync();

    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // TODO: seed only production relevant data
    await app.Services.ConfigureDatabaseAsync();
}

app.UseCors();
app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapGraphQL();

if (app.Environment.IsDevelopment())
{
    app.MapSwagger();
}

app.MapAuthenticationEndpoints();

await app.RunAsync();
return;


static Uri GetGraphQLUri(in Uri uri) => new UriBuilder(Uri.UriSchemeHttps, uri.Host, uri.Port, uri.PathAndQuery).Uri;

static Uri GetGraphQLStreamingUri(in Uri uri) => new UriBuilder(Uri.UriSchemeWs, uri.Host, uri.Port, uri.PathAndQuery).Uri;

/// <summary>
/// Used for integration tests. Entry point class has to accessible from the custom WebApplicationFactory.
/// </summary>
public partial class Program;
