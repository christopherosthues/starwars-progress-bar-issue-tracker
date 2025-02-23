namespace StarWarsProgressBarIssueTracker.CodeGen.EFCoreEnums;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DatabaseEnumAttribute(Type enumType) : Attribute
{
    public Type EnumType => enumType;
}
