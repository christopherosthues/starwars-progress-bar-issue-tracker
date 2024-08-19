using Microsoft.Extensions.DependencyInjection;
using Moq;
using StarWarsProgressBarIssueTracker.Common.Tests;
using StarWarsProgressBarIssueTracker.Infrastructure.Models;
using StarWarsProgressBarIssueTracker.Infrastructure.Repositories;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Tests;

[TestFixture(TestOf = typeof(RepositoryRegistration))]
[Category(TestCategory.Unit)]
public class RepositoryRegistrationTests
{
    [Test]
    public void AddRepositoriesShouldRegisterRepositories()
    {
        // Arrange
        var serviceCollectionMock = new Mock<IServiceCollection>();

        // Act
        serviceCollectionMock.Object.AddRepositories();

        // Assert
        serviceCollectionMock.Verify(
            mock => mock.Add(It.Is<ServiceDescriptor>(sd =>
                sd.ServiceType == typeof(IAppearanceRepository) && sd.ImplementationType == typeof(AppearanceRepository) &&
                sd.Lifetime == ServiceLifetime.Scoped)), Times.Once);
        serviceCollectionMock.Verify(
            mock => mock.Add(It.Is<ServiceDescriptor>(sd =>
                sd.ServiceType == typeof(IRepository<DbLabel>) && sd.ImplementationType == typeof(LabelRepository) &&
                sd.Lifetime == ServiceLifetime.Scoped)), Times.Once);
        serviceCollectionMock.Verify(
            mock => mock.Add(It.Is<ServiceDescriptor>(sd =>
                sd.ServiceType == typeof(IIssueRepository) && sd.ImplementationType == typeof(IssueRepository) &&
                sd.Lifetime == ServiceLifetime.Scoped)), Times.Once);
        serviceCollectionMock.Verify(
            mock => mock.Add(It.Is<ServiceDescriptor>(sd =>
                sd.ServiceType == typeof(IRepository<DbMilestone>) && sd.ImplementationType == typeof(MilestoneRepository) &&
                sd.Lifetime == ServiceLifetime.Scoped)), Times.Once);
        serviceCollectionMock.Verify(
            mock => mock.Add(It.Is<ServiceDescriptor>(sd =>
                sd.ServiceType == typeof(IRepository<DbRelease>) && sd.ImplementationType == typeof(ReleaseRepository) &&
                sd.Lifetime == ServiceLifetime.Scoped)), Times.Once);
        serviceCollectionMock.Verify(
            mock => mock.Add(It.Is<ServiceDescriptor>(sd =>
                sd.ServiceType == typeof(ITaskRepository) && sd.ImplementationType == typeof(TaskRepository) &&
                sd.Lifetime == ServiceLifetime.Scoped)), Times.Once);
        serviceCollectionMock.Verify(
            mock => mock.Add(It.Is<ServiceDescriptor>(sd =>
                sd.ServiceType == typeof(IRepository<DbJob>) && sd.ImplementationType == typeof(IssueTrackerRepositoryBase<DbJob>) &&
                sd.Lifetime == ServiceLifetime.Scoped)), Times.Once);
    }
}
