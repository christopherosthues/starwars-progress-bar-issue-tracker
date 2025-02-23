namespace StarWarsProgressBarIssueTracker.CodeGen.GraphQL;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class MutationFieldNameAttribute(string name) : Attribute
{
    public string Name { get; } = string.IsNullOrWhiteSpace(name)
        ? throw new ArgumentException($"{nameof(name)} must not be null or whitespace", nameof(name))
        : name;
}
