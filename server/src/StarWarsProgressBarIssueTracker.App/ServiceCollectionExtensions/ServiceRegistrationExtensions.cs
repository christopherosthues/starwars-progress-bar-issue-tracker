using StarWarsProgressBarIssueTracker.App.Issues;
using StarWarsProgressBarIssueTracker.App.Jobs;
using StarWarsProgressBarIssueTracker.App.Labels;
using StarWarsProgressBarIssueTracker.App.Mappers;
using StarWarsProgressBarIssueTracker.App.Milestones;
using StarWarsProgressBarIssueTracker.App.Mutations;
using StarWarsProgressBarIssueTracker.App.Queries;
using StarWarsProgressBarIssueTracker.App.Releases;
using StarWarsProgressBarIssueTracker.App.Vehicles;
using StarWarsProgressBarIssueTracker.Domain;
using StarWarsProgressBarIssueTracker.Domain.Issues;
using StarWarsProgressBarIssueTracker.Domain.Labels;
using StarWarsProgressBarIssueTracker.Domain.Milestones;
using StarWarsProgressBarIssueTracker.Domain.Releases;
using StarWarsProgressBarIssueTracker.Domain.Vehicles;
using StarWarsProgressBarIssueTracker.Infrastructure.GitHub.Configuration;
using StarWarsProgressBarIssueTracker.Infrastructure.Gitlab.Configuration;

namespace StarWarsProgressBarIssueTracker.App.ServiceCollectionExtensions;

public static class ServiceRegistrationExtensions
{
    public static IServiceCollection AddIssueTrackerConfigurations(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.Configure<GitlabConfiguration>(configuration.GetSection("Gitlab"));
        serviceCollection.Configure<GitHubConfiguration>(configuration.GetSection("GitHub"));

        return serviceCollection;
    }

    public static IServiceCollection AddDataPorts(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IDataPort<Appearance>, AppearanceDataPort>();
        serviceCollection.AddScoped<IDataPort<Issue>, IssueDataPort>();
        serviceCollection.AddScoped<IDataPort<Label>, LabelDataPort>();
        serviceCollection.AddScoped<IDataPort<Milestone>, MilestoneDataPort>();
        serviceCollection.AddScoped<IDataPort<Release>, ReleaseDataPort>();

        return serviceCollection;
    }

    public static IServiceCollection AddIssueTrackerServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IAppearanceService, AppearanceService>();
        serviceCollection.AddScoped<IIssueService, IssueService>();
        serviceCollection.AddScoped<ILabelService, LabelService>();
        serviceCollection.AddScoped<IMilestoneService, MilestoneService>();
        serviceCollection.AddScoped<IReleaseService, ReleaseService>();

        serviceCollection.AddScoped<JobSchedulingService>();
        serviceCollection.AddScoped<JobExecutionService>();
        serviceCollection.AddScoped<JobFactory>();

        return serviceCollection;
    }

    public static IServiceCollection AddIssueTrackerMappers(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddAutoMapper(typeof(EntityMapperProfile));

        return serviceCollection;
    }

    public static IServiceCollection AddGraphQLQueries(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IssueTrackerQueries>();

        return serviceCollection;
    }

    public static IServiceCollection AddGraphQLMutations(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IssueTrackerMutations>();

        return serviceCollection;
    }

    public static IServiceCollection AddJobs(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<JobScheduler>();
        serviceCollection.AddScoped<GitHubSynchronizationJobScheduler>();
        serviceCollection.AddScoped<GitlabSynchronizationJobScheduler>();
        serviceCollection.AddScoped<GitHubSynchronizationJob>();
        serviceCollection.AddScoped<GitlabSynchronizationJob>();

        return serviceCollection;
    }
}
