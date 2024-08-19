using AutoMapper;
using StarWarsProgressBarIssueTracker.App.Mappers;
using StarWarsProgressBarIssueTracker.Common.Tests;

namespace StarWarsProgressBarIssueTracker.App.Tests.Mappers;

[TestFixture(TestOf = typeof(EntityMapperProfile))]
[Category(TestCategory.Unit)]
public class EntityMapperProfileTests
{
    [Test]
    public void MapperShouldBeConfiguredCorrectly()
    {
        // Arrange
        var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile<EntityMapperProfile>());

        // Act and Assert
        mapperConfiguration.AssertConfigurationIsValid();
    }
}
