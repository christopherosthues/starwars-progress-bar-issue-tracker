using Microsoft.Extensions.DependencyInjection;
using StarWarsProgressBarIssueTracker.Infrastructure.Gitlab.Networking;
using StarWarsProgressBarIssueTracker.Infrastructure.Gitlab.Services;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Gitlab;

public static class RegisterServices
{
    public static IServiceCollection AddGitlabServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<GraphQLService>();
        serviceCollection.AddScoped<RestService>();
        serviceCollection.AddScoped<GitlabSynchronizationService>();

        return serviceCollection;
    }
}
