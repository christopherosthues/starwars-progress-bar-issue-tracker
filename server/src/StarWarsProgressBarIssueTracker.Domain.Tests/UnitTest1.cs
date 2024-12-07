using StarWarsProgressBarIssueTracker.TestHelpers;

namespace StarWarsProgressBarIssueTracker.Domain.Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    [Error("Error")]
    public void Test1()
    {
        Assert.Pass();
    }
}
