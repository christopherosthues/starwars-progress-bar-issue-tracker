namespace StarWarsProgressBarIssueTracker.CodeGen.EFCoreEnums;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class DatabaseEnumOptionsAttribute : Attribute
{
    public string EntityNameFormat { get; set; } = "{0}Entity";

    public int EnumNameMaxLength { get; set; } = 255;

    public string EnumNamespace { get; set; } = string.Empty;

    public string ConfigurationNamespace { get; set; } = string.Empty;

    public string SchemaName { get; set; } = string.Empty;
}
