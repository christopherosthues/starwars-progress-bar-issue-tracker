using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using StarWarsProgressBarIssueTracker.Infrastructure.Gitlab.Networking;
using StarWarsProgressBarIssueTracker.TestHelpers;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Gitlab.Tests;

[Category(TestCategory.Unit)]
public class RegisterServicesTests
{
    [Test]
    public void AddGitlabServicesShouldRegisterGitlabGraphQlAndRestServices()
    {
        // Arrange
        IServiceCollection? serviceCollectionMock = Substitute.For<IServiceCollection>();

        // Act
        serviceCollectionMock.AddGitlabServices();

        // Assert
        serviceCollectionMock.Received(1).Add(Arg.Is<ServiceDescriptor>(sd =>
                sd.ServiceType == typeof(GraphQLService) && sd.ImplementationType == typeof(GraphQLService) &&
                sd.Lifetime == ServiceLifetime.Scoped));
        serviceCollectionMock.Received(1).Add(Arg.Is<ServiceDescriptor>(sd =>
                sd.ServiceType == typeof(RestService) && sd.ImplementationType == typeof(RestService) &&
                sd.Lifetime == ServiceLifetime.Scoped));
    }
}
