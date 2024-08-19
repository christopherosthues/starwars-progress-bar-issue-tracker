using Microsoft.Extensions.DependencyInjection;
using Moq;
using StarWarsProgressBarIssueTracker.Common.Tests;
using StarWarsProgressBarIssueTracker.Infrastructure.Gitlab.Networking;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Gitlab.Tests;

[TestFixture(TestOf = typeof(RegisterServices))]
[Category(TestCategory.Unit)]
public class RegisterServicesTests
{
    [Test]
    public void AddGitlabServicesShouldRegisterGitlabGraphQlAndRestServices()
    {
        // Arrange
        var serviceCollectionMock = new Mock<IServiceCollection>();

        // Act
        serviceCollectionMock.Object.AddGitlabServices();

        // Assert
        serviceCollectionMock.Verify(
            mock => mock.Add(It.Is<ServiceDescriptor>(sd =>
                sd.ServiceType == typeof(GraphQLService) && sd.ImplementationType == typeof(GraphQLService) &&
                sd.Lifetime == ServiceLifetime.Scoped)), Times.Once);
        serviceCollectionMock.Verify(
            mock => mock.Add(It.Is<ServiceDescriptor>(sd =>
                sd.ServiceType == typeof(RestService) && sd.ImplementationType == typeof(RestService) &&
                sd.Lifetime == ServiceLifetime.Scoped)), Times.Once);
    }
}
