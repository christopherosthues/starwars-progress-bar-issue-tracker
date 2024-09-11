namespace StarWarsProgressBarIssueTracker.App.Tests.Helpers.GraphQL.Payloads;

public class Edge<T>
{
    public T Node { get; set; } = default!;
}
