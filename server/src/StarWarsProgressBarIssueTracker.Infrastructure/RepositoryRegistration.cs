using Microsoft.Extensions.DependencyInjection;
using StarWarsProgressBarIssueTracker.Domain;
using StarWarsProgressBarIssueTracker.Domain.Issues;
using StarWarsProgressBarIssueTracker.Domain.Labels;
using StarWarsProgressBarIssueTracker.Domain.Milestones;
using StarWarsProgressBarIssueTracker.Domain.Releases;
using StarWarsProgressBarIssueTracker.Domain.Vehicles;
using StarWarsProgressBarIssueTracker.Infrastructure.Models;
using StarWarsProgressBarIssueTracker.Infrastructure.Repositories;

namespace StarWarsProgressBarIssueTracker.Infrastructure;

public static class RepositoryRegistration
{
    public static void AddRepositories(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IAppearanceRepository, AppearanceRepository>();
        serviceCollection.AddScoped<ILabelRepository, LabelRepository>();
        serviceCollection.AddScoped<IIssueRepository, IssueRepository>();
        serviceCollection.AddScoped<IMilestoneRepository, MilestoneRepository>();
        serviceCollection.AddScoped<IReleaseRepository, ReleaseRepository>();
        serviceCollection.AddScoped<ITaskRepository, TaskRepository>();
        serviceCollection.AddScoped<IRepository<DbJob>, JobRepository>();
    }
}
