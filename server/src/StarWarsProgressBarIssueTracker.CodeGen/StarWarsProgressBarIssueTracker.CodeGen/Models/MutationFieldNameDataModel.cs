using System.Collections.Immutable;

namespace StarWarsProgressBarIssueTracker.CodeGen.Models;

public class MutationFieldNameDataModel
{
    public required string MethodName { get; set; }

    public required string FullyQualifiedTypeName { get; set; }

    public required string MinimalTypeName { get; set; }

    public required string Namespace { get; set; }

    public required string FieldName { get; set; }

    public required string MethodReturnType { get; set; }

    public required ImmutableArray<string> MethodParameters { get; set; }
}
