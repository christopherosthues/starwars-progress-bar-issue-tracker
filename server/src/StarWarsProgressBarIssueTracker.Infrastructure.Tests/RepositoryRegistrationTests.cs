using Microsoft.Extensions.DependencyInjection;
using Moq;
using StarWarsProgressBarIssueTracker.Common.Tests;
using StarWarsProgressBarIssueTracker.Domain;
using StarWarsProgressBarIssueTracker.Domain.Issues;
using StarWarsProgressBarIssueTracker.Domain.Labels;
using StarWarsProgressBarIssueTracker.Domain.Milestones;
using StarWarsProgressBarIssueTracker.Domain.Releases;
using StarWarsProgressBarIssueTracker.Domain.Vehicles;
using StarWarsProgressBarIssueTracker.Infrastructure.Models;
using StarWarsProgressBarIssueTracker.Infrastructure.Repositories;
using StarWarsProgressBarIssueTracker.TestHelpers.Extensions;

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
        serviceCollectionMock.VerifyServiceRegistered(typeof(IAppearanceRepository), typeof(AppearanceRepository), ServiceLifetime.Scoped);
        serviceCollectionMock.VerifyServiceRegistered(typeof(ILabelRepository), typeof(LabelRepository), ServiceLifetime.Scoped);
        serviceCollectionMock.VerifyServiceRegistered(typeof(IIssueRepository), typeof(IssueRepository), ServiceLifetime.Scoped);
        serviceCollectionMock.VerifyServiceRegistered(typeof(IMilestoneRepository), typeof(MilestoneRepository), ServiceLifetime.Scoped);
        serviceCollectionMock.VerifyServiceRegistered(typeof(IReleaseRepository), typeof(ReleaseRepository), ServiceLifetime.Scoped);
        serviceCollectionMock.VerifyServiceRegistered(typeof(ITaskRepository), typeof(TaskRepository), ServiceLifetime.Scoped);
        serviceCollectionMock.VerifyServiceRegistered(typeof(IRepository<DbJob>), typeof(JobRepository), ServiceLifetime.Scoped);
    }
}
