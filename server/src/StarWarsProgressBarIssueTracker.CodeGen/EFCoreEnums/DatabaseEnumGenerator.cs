using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using StarWarsProgressBarIssueTracker.CodeGen.EFCoreEnums.Models;

namespace StarWarsProgressBarIssueTracker.CodeGen.EFCoreEnums;

[Generator]
public class DatabaseEnumGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<(INamedTypeSymbol?, DatabaseEnumOptionsDataModel, List<DatabaseEnumDataModel>)> provider = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                $"{typeof(DatabaseEnumAttribute).Namespace}.{nameof(DatabaseEnumAttribute)}",
                static (node, _) => node is ClassDeclarationSyntax,//IsTargetClass(node),
                static (context, _) => GetDatabaseEnumDataModel(context));

        context.RegisterSourceOutput(context.CompilationProvider.Combine(provider.Collect()),
            (sourceContext, data) => GenerateCode(sourceContext, data.Right));
    }

    private static bool IsTargetClass(SyntaxNode syntaxNode)
    {
        if (syntaxNode is ClassDeclarationSyntax classDeclaration)
        {
            return classDeclaration.AttributeLists
                .SelectMany(al => al.Attributes)
                .Any(attribute => attribute.Name.ToString() == $"{typeof(DatabaseEnumOptionsAttribute).Namespace}.{nameof(DatabaseEnumOptionsAttribute)}" ||
                                  attribute.Name.ToString() == $"{typeof(DatabaseEnumAttribute).Namespace}.{nameof(DatabaseEnumAttribute)}");
        }

        return false;
    }

    private static (INamedTypeSymbol?, DatabaseEnumOptionsDataModel, List<DatabaseEnumDataModel>) GetDatabaseEnumDataModel(GeneratorAttributeSyntaxContext context)
    {
        if (context.TargetSymbol is not INamedTypeSymbol typeSymbol)
        {
            return (null, new DatabaseEnumOptionsDataModel(),[]);
        }

        if (typeSymbol.BaseType?.Name != "DbContext")
        {
            return (typeSymbol, new DatabaseEnumOptionsDataModel(),[]);
        }

        List<DatabaseEnumDataModel> databaseEnumDataModels = [];
        DatabaseEnumOptionsDataModel databaseEnumOptionsDataModel = new DatabaseEnumOptionsDataModel();

        foreach (AttributeData attribute in typeSymbol.GetAttributes())
        {
            if (attribute.AttributeClass?.ToDisplayString() == $"{typeof(DatabaseEnumAttribute).Namespace}.{nameof(DatabaseEnumAttribute)}")
            {
                INamedTypeSymbol? value = attribute.ConstructorArguments[0].Value as INamedTypeSymbol;
                databaseEnumDataModels.Add(new DatabaseEnumDataModel
                {
                    EnumName = value!.Name,
                    EnumNamespace = value.ContainingNamespace.ToString(),
                });
            }
            else if (attribute.AttributeClass?.ToDisplayString() == $"{typeof(DatabaseEnumOptionsAttribute).Namespace}.{nameof(DatabaseEnumOptionsAttribute)}")
            {
                databaseEnumOptionsDataModel = GetDatabaseEnumOptionsDataModel(attribute);
            }
        }

        return (typeSymbol, databaseEnumOptionsDataModel, databaseEnumDataModels);
    }

    private static DatabaseEnumOptionsDataModel GetDatabaseEnumOptionsDataModel(AttributeData attribute)
    {
        DatabaseEnumOptionsDataModel databaseEnumOptionsDataModel;
        KeyValuePair<string, TypedConstant>? configurationNamespace =
            attribute.NamedArguments.FirstOrDefault(arg =>
                arg.Key == nameof(DatabaseEnumOptionsAttribute.ConfigurationNamespace));
        string configurationNamespaceValue = configurationNamespace?.Value.Value?.ToString() ?? string.Empty;
        KeyValuePair<string, TypedConstant>? enumNamespace =
            attribute.NamedArguments.FirstOrDefault(arg =>
                arg.Key == nameof(DatabaseEnumOptionsAttribute.EnumNamespace));
        string enumNamespaceValue = enumNamespace?.Value.Value?.ToString() ?? string.Empty;
        KeyValuePair<string, TypedConstant>? schemaName =
            attribute.NamedArguments.FirstOrDefault(arg =>
                arg.Key == nameof(DatabaseEnumOptionsAttribute.SchemaName));
        string schemaNameValue = schemaName?.Value.Value?.ToString() ?? string.Empty;
        KeyValuePair<string, TypedConstant>? entityNameFormat =
            attribute.NamedArguments.FirstOrDefault(arg =>
                arg.Key == nameof(DatabaseEnumOptionsAttribute.EntityNameFormat));
        string entityNameFormatValue = entityNameFormat?.Value.Value?.ToString() ?? "{0}Entity";
        KeyValuePair<string, TypedConstant>? enumNameMaxLength =
            attribute.NamedArguments.FirstOrDefault(arg =>
                arg.Key == nameof(DatabaseEnumOptionsAttribute.EnumNameMaxLength));
        string maxLength = enumNameMaxLength?.Value.Value?.ToString() ?? "255";
        databaseEnumOptionsDataModel = new DatabaseEnumOptionsDataModel
        {
            ConfigurationNamespace = configurationNamespaceValue,
            EnumNamespace = enumNamespaceValue,
            SchemaName = schemaNameValue,
            EntityNameFormat = entityNameFormatValue,
            EnumNameMaxLength = int.Parse(maxLength),
        };
        return databaseEnumOptionsDataModel;
    }

    private static void GenerateCode(SourceProductionContext context,
        ImmutableArray<(INamedTypeSymbol?, DatabaseEnumOptionsDataModel, List<DatabaseEnumDataModel>)> dataModels)
    {
        foreach ((INamedTypeSymbol? dbContextSymbol, DatabaseEnumOptionsDataModel databaseEnumOptionsDataModel, List<DatabaseEnumDataModel> databaseEnumDataModels) in dataModels)
        {
            string enumDbSets = string.Empty;
            foreach (DatabaseEnumDataModel databaseEnumDataModel in databaseEnumDataModels)
            {
                string enumTypeName = databaseEnumDataModel.EnumName;
                string fullQualifiedEnumTypeName = databaseEnumDataModel.EnumNamespace + "." + enumTypeName;
                string enumEntityName =
                    string.Format(databaseEnumOptionsDataModel.EntityNameFormat, enumTypeName);
                string fullQualifiedEntityName = databaseEnumOptionsDataModel.EnumNamespace + "." + enumEntityName;
                string enumEntityCode =
                    $$"""
                      // <auto-generated />
                      #nullable enable
                      
                      namespace {{databaseEnumOptionsDataModel.EnumNamespace}};
                      
                      public class {{enumEntityName}}
                      {
                          public int Id { get; set; }
                      
                          public string Name { get; set; } = null!;
                      }
                      """;
                context.AddSource($"{enumEntityName}.g.cs",
                    SourceText.From(enumEntityCode, Encoding.UTF8));

                string tableName = GetTableName(enumTypeName);
                enumDbSets += $$"""
                                    public DbSet<{{fullQualifiedEntityName}}> {{tableName}} { get; init; } = null!;

                                """;

                string configurationCode =
                    $$"""
                      // <auto-generated />
                      #nullable enable
                      using Microsoft.EntityFrameworkCore;
                      using Microsoft.EntityFrameworkCore.Metadata.Builders;
                      
                      namespace {{databaseEnumOptionsDataModel.ConfigurationNamespace}};
                      
                      public partial class {{enumTypeName}}Configuration : IEntityTypeConfiguration<{{fullQualifiedEntityName}}>
                      {
                          public void Configure(EntityTypeBuilder<{{fullQualifiedEntityName}}> builder)
                          {
                              builder.ToTable("{{tableName}}", "{{databaseEnumOptionsDataModel.SchemaName}}");
                              builder.HasKey(enumEntity => enumEntity.Id);
                              builder.Property(enumEntity => enumEntity.Id).IsRequired();
                              builder.Property(enumEntity => enumEntity.Name).IsRequired()
                                  .HasMaxLength({{databaseEnumOptionsDataModel.EnumNameMaxLength}});
                      
                              builder.HasData(
                                  Enum.GetValues(typeof({{fullQualifiedEnumTypeName}}))
                                      .Cast<{{fullQualifiedEnumTypeName}}>()
                                      .Except([({{fullQualifiedEnumTypeName}})0])
                                      .Select(enumValue => new {{fullQualifiedEntityName}}
                                      {
                                          Id = (int)enumValue, Name = Enum.GetName(enumValue) ?? ((int)enumValue).ToString()
                                      }));
                          }
                      }
                      """;
                context.AddSource($"{enumTypeName}Configuration.g.cs",
                    SourceText.From(configurationCode, Encoding.UTF8));
            }

            if (dbContextSymbol is not null)
            {
                string dbContextCode = $$"""
                                         // <auto-generated />
                                         #nullable enable
                                         
                                         using Microsoft.EntityFrameworkCore;
                                         
                                         namespace {{dbContextSymbol.ContainingNamespace}};
                                         
                                         public partial class {{dbContextSymbol.Name}}
                                         {
                                         {{enumDbSets}}
                                         }
                                         """;
                context.AddSource($"{dbContextSymbol.Name}.g.cs",
                    SourceText.From(dbContextCode, Encoding.UTF8));
            }
        }
    }

    private static string GetTableName(string fullQualifiedEnumTypeName)
    {
        if (fullQualifiedEnumTypeName.EndsWith("s"))
        {
            return fullQualifiedEnumTypeName + "es";
        }

        if (fullQualifiedEnumTypeName.EndsWith("y"))
        {
            return fullQualifiedEnumTypeName.Substring(0, fullQualifiedEnumTypeName.Length - 1) + "ies";
        }

        return fullQualifiedEnumTypeName + "s";
    }
}
