namespace StarWarsProgressBarIssueTracker.CodeGen.EFCoreEnums.Models;

public class DatabaseEnumOptionsDataModel
{
    public string EntityNameFormat { get; set; } = string.Empty;

    public int EnumNameMaxLength { get; set; } = 255;

    public string EnumNamespace { get; set; } = string.Empty;

    public string ConfigurationNamespace { get; set; } = string.Empty;

    public string SchemaName { get; set; } = string.Empty;
}
