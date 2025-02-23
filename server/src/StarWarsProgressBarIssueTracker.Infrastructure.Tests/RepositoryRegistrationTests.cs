using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using StarWarsProgressBarIssueTracker.Domain;
using StarWarsProgressBarIssueTracker.Domain.Issues;
using StarWarsProgressBarIssueTracker.Domain.Labels;
using StarWarsProgressBarIssueTracker.Domain.Milestones;
using StarWarsProgressBarIssueTracker.Domain.Releases;
using StarWarsProgressBarIssueTracker.Domain.Vehicles;
using StarWarsProgressBarIssueTracker.Infrastructure.Entities;
using StarWarsProgressBarIssueTracker.Infrastructure.Repositories;
using StarWarsProgressBarIssueTracker.TestHelpers;
using StarWarsProgressBarIssueTracker.TestHelpers.Extensions;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Tests;

[Category(TestCategory.Unit)]
public class RepositoryRegistrationTests
{
    [Test]
    public void AddRepositoriesShouldRegisterRepositories()
    {
        // Arrange
        IServiceCollection? serviceCollectionMock = Substitute.For<IServiceCollection>();

        // Act
        serviceCollectionMock.AddRepositories();

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
