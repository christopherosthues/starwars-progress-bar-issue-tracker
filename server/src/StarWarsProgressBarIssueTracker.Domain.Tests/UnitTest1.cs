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
    public async Task Test1()
    {
        var errorTest = "Error test";
        await Assert.That(errorTest).IsNotEmpty();
    }
}
