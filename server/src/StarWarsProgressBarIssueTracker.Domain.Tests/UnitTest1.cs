using StarWarsProgressBarIssueTracker.TestHelpers;

namespace StarWarsProgressBarIssueTracker.Domain.Tests;

public class Tests
{
    [Before(Test)]
    public void Setup()
    {
    }

    [Test]
    [Error("Error")]
    public void Test1()
    {
        Assert.Fail("Error test");
    }
}
