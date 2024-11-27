using Microsoft.Extensions.DependencyInjection;
using Moq;
using StarWarsProgressBarIssueTracker.App.Appearances;
using StarWarsProgressBarIssueTracker.App.Extensions;
using StarWarsProgressBarIssueTracker.App.Issues;
using StarWarsProgressBarIssueTracker.App.Labels;
using StarWarsProgressBarIssueTracker.App.Milestones;
using StarWarsProgressBarIssueTracker.App.Mutations;
using StarWarsProgressBarIssueTracker.App.Queries;
using StarWarsProgressBarIssueTracker.App.Releases;
using StarWarsProgressBarIssueTracker.Domain.Issues;
using StarWarsProgressBarIssueTracker.Domain.Labels;
using StarWarsProgressBarIssueTracker.Domain.Milestones;
using StarWarsProgressBarIssueTracker.Domain.Releases;
using StarWarsProgressBarIssueTracker.Domain.Vehicles;
using StarWarsProgressBarIssueTracker.TestHelpers.Extensions;

namespace StarWarsProgressBarIssueTracker.App.Tests.Unit.Extensions;

[TestFixture(TestOf = typeof(ServiceRegistrationExtensions))]
public class ServiceCollectionExtensionsTests
{
    [Test]
    public void AddIssueTrackerServices_RegistersAllServices()
    {
        // Arrange
        Mock<IServiceCollection> serviceCollectionMock = new();
        IServiceCollection sut = serviceCollectionMock.Object;

        // Act
        sut.AddIssueTrackerServices();

        // Assert
        serviceCollectionMock.VerifyServiceRegistered(typeof(IAppearanceService), typeof(AppearanceService),
            ServiceLifetime.Scoped);
        serviceCollectionMock.VerifyServiceRegistered(typeof(IIssueService), typeof(IssueService),
            ServiceLifetime.Scoped);
        serviceCollectionMock.VerifyServiceRegistered(typeof(ILabelService), typeof(LabelService),
            ServiceLifetime.Scoped);
        serviceCollectionMock.VerifyServiceRegistered(typeof(IMilestoneService), typeof(MilestoneService),
            ServiceLifetime.Scoped);
        serviceCollectionMock.VerifyServiceRegistered(typeof(IReleaseService), typeof(ReleaseService),
            ServiceLifetime.Scoped);
    }

    [Test]
    public void AddGraphQlQueries_RegistersGraphQlQueries()
    {
        // Arrange
        Mock<IServiceCollection> serviceCollectionMock = new();
        IServiceCollection sut = serviceCollectionMock.Object;

        // Act
        sut.AddGraphQlQueries();

        // Assert
        serviceCollectionMock.VerifyServiceRegistered(typeof(IssueTrackerQueries), ServiceLifetime.Scoped);
    }

    [Test]
    public void AddGraphQlMutations_RegistersGraphQlMutations()
    {
        // Arrange
        Mock<IServiceCollection> serviceCollectionMock = new();
        IServiceCollection sut = serviceCollectionMock.Object;

        // Act
        sut.AddGraphQlMutations();

        // Assert
        serviceCollectionMock.VerifyServiceRegistered(typeof(IssueTrackerMutations), ServiceLifetime.Scoped);
    }
}
