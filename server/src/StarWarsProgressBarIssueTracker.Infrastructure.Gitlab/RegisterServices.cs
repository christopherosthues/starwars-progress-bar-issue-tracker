using Microsoft.Extensions.DependencyInjection;
using StarWarsProgressBarIssueTracker.Infrastructure.Gitlab.Networking;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Gitlab;

public static class RegisterServices
{
    public static IServiceCollection AddGitlabServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<GraphQLService>();
        serviceCollection.AddScoped<RestService>();

        return serviceCollection;
    }
}
