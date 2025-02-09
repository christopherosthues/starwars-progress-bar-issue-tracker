using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
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

public class ServiceCollectionExtensionsTests
{
    [Test]
    public void AddIssueTrackerServices_RegistersAllServices()
    {
        // Arrange
        IServiceCollection sut = Substitute.For<IServiceCollection>();

        // Act
        sut.AddIssueTrackerServices();

        // Assert
        sut.VerifyServiceRegistered(typeof(IAppearanceService), typeof(AppearanceService),
            ServiceLifetime.Scoped);
        sut.VerifyServiceRegistered(typeof(IIssueService), typeof(IssueService),
            ServiceLifetime.Scoped);
        sut.VerifyServiceRegistered(typeof(ILabelService), typeof(LabelService),
            ServiceLifetime.Scoped);
        sut.VerifyServiceRegistered(typeof(IMilestoneService), typeof(MilestoneService),
            ServiceLifetime.Scoped);
        sut.VerifyServiceRegistered(typeof(IReleaseService), typeof(ReleaseService),
            ServiceLifetime.Scoped);
    }

    [Test]
    public void AddGraphQlQueries_RegistersGraphQlQueries()
    {
        // Arrange
        IServiceCollection sut = Substitute.For<IServiceCollection>();

        // Act
        sut.AddGraphQlQueries();

        // Assert
        sut.VerifyServiceRegistered(typeof(IssueTrackerQueries), ServiceLifetime.Scoped);
    }

    [Test]
    public void AddGraphQlMutations_RegistersGraphQlMutations()
    {
        // Arrange
        IServiceCollection sut = Substitute.For<IServiceCollection>();

        // Act
        sut.AddGraphQlMutations();

        // Assert
        sut.VerifyServiceRegistered(typeof(IssueTrackerMutations), ServiceLifetime.Scoped);
    }
}
