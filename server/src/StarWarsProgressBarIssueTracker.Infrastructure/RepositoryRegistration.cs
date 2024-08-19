using Microsoft.Extensions.DependencyInjection;
using StarWarsProgressBarIssueTracker.Infrastructure.Models;
using StarWarsProgressBarIssueTracker.Infrastructure.Repositories;

namespace StarWarsProgressBarIssueTracker.Infrastructure;

public static class RepositoryRegistration
{
    public static void AddRepositories(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IAppearanceRepository, AppearanceRepository>();
        serviceCollection.AddScoped<IRepository<DbLabel>, LabelRepository>();
        serviceCollection.AddScoped<IIssueRepository, IssueRepository>();
        serviceCollection.AddScoped<IRepository<DbMilestone>, MilestoneRepository>();
        serviceCollection.AddScoped<IRepository<DbRelease>, ReleaseRepository>();
        serviceCollection.AddScoped<ITaskRepository, TaskRepository>();
        serviceCollection.AddScoped<IRepository<DbJob>, IssueTrackerRepositoryBase<DbJob>>();
    }
}
