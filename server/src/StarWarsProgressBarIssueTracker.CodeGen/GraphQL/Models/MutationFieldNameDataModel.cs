using System.Collections.Immutable;

namespace StarWarsProgressBarIssueTracker.CodeGen.GraphQL.Models;

public class MutationFieldNameDataModel
{
    public string MethodName { get; set; } = string.Empty;

    public string FullyQualifiedTypeName { get; set; } = string.Empty;

    public string MinimalTypeName { get; set; } = string.Empty;

    public string Namespace { get; set; } = string.Empty;

    public string FieldName { get; set; } = string.Empty;

    public string MethodReturnType { get; set; } = string.Empty;

    public ImmutableArray<string> MethodParameters { get; set; }
}
