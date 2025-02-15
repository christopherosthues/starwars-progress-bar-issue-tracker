using TUnit.Core.Interfaces;

namespace StarWarsProgressBarIssueTracker.TestHelpers;

public record IssueTrackerParallelLimit : IParallelLimit
{
    public int Limit => 5;
}
