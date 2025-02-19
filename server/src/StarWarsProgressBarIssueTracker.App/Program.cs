using System.Net.Http.Headers;
using StarWarsProgressBarIssueTracker.App.Extensions;
using StarWarsProgressBarIssueTracker.App.Mutations;
using StarWarsProgressBarIssueTracker.App.Queries;
using StarWarsProgressBarIssueTracker.Infrastructure;
using StarWarsProgressBarIssueTracker.Infrastructure.Gitlab;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders().AddConsole();

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

builder.Services.AddGraphQLServer()
    .AddMutationConventions(applyToAllMutations: true)
    // .AddQueryConventions()
    .AddQueryType<IssueTrackerQueries>()
    .AddMutationType<IssueTrackerMutations>()
    .AddAppTypes()
    .AddPagingArguments()
    .AddSorting();
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
builder.Services.AddDataPorts();
builder.Services.AddIssueTrackerConfigurations(builder.Configuration);
builder.Services.AddIssueTrackerServices();
builder.Services.AddMappers();
builder.Services.AddGraphQlQueries();
builder.Services.AddGraphQlMutations();
builder.Services.AddGitlabServices();

// builder.Services.AddCors(corsOptions =>
//     corsOptions.AddDefaultPolicy(corsPolicyBuilder =>
//         corsPolicyBuilder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    await app.Services.ConfigureDatabaseAsync();
}
else
{
    // TODO: seed only production relevant data
    await app.Services.ConfigureDatabaseAsync();
}

// app.UseCors();

app.UseHttpsRedirection();

app.MapGraphQL();

await app.RunAsync();
return;


static Uri GetGraphQLUri(in Uri uri) => new UriBuilder(Uri.UriSchemeHttps, uri.Host, uri.Port, uri.PathAndQuery).Uri;

static Uri GetGraphQLStreamingUri(in Uri uri) => new UriBuilder(Uri.UriSchemeWs, uri.Host, uri.Port, uri.PathAndQuery).Uri;

/// <summary>
/// Used for integration tests. Entry point class has to accessible from the custom WebApplicationFactory.
/// </summary>
public partial class Program;
