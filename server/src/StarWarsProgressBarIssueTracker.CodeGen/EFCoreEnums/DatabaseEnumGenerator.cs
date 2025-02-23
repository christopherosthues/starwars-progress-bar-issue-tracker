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
        // IncrementalValuesProvider<ClassDeclarationSyntax?> syntaxReceiver = context.SyntaxProvider
        //     .CreateSyntaxProvider(
        //         (node, _) => IsTargetClass(node),
        //         (ctx, _) => ctx.Node as ClassDeclarationSyntax
        //     ).Where(cls => cls is not null);
        // IncrementalValuesProvider<DatabaseEnumOptionsDataModel?> optionsProvider = context.SyntaxProvider
        //     .ForAttributeWithMetadataName(
        //         $"{typeof(DatabaseEnumOptionsAttribute).Namespace}.{nameof(DatabaseEnumOptionsAttribute)}",
        //         static (node, _) => node is ClassDeclarationSyntax,
        //         static (context, _) => GetDatabaseEnumOptionsDataModel(context))
        //     .Where(static cls => cls is not null);
        // var options = optionsProvider.Collect();
        //
        IncrementalValuesProvider<(DatabaseEnumOptionsDataModel, List<DatabaseEnumDataModel>)> provider = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                $"{typeof(DatabaseEnumAttribute).Namespace}.{nameof(DatabaseEnumAttribute)}",
                static (node, _) => node is ClassDeclarationSyntax,//IsTargetClass(node),
                static (context, _) => GetDatabaseEnumDataModel(context));

        context.RegisterSourceOutput(context.CompilationProvider.Combine(provider.Collect()),
            (sourceContext, data) => GenerateCode(sourceContext, data.Right));
        // context.RegisterSourceOutput(syntaxReceiver, (productionContext, syntax) =>
        // {
        //     DatabaseEnumOptionsDataModel? config = GetConfiguration(syntax);
        //     if (config != null)
        //     {
        //         // Find classes with ActionAttribute and generate code based on that
        //         var actions = GetActionAttributes(syntax);
        //
        //         if (actions.Any())
        //         {
        //             // Generate code using the configuration and action attributes
        //             var generatedCode = GenerateCode(config.Value, actions);
        //             productionContext.AddSource($"{syntax.Identifier.Text}_Generated.cs", SourceText.From(generatedCode, Encoding.UTF8));
        //         }
        //     }
        // });
    }

    private static bool IsTargetClass(SyntaxNode syntaxNode)
    {
        if (syntaxNode is ClassDeclarationSyntax classDeclaration)
        {
            return classDeclaration.AttributeLists
                .SelectMany(al => al.Attributes)
                .Any(attribute => attribute.Name.ToString() == nameof(DatabaseEnumOptionsAttribute) ||
                                  attribute.Name.ToString() == nameof(DatabaseEnumAttribute));
        }

        return false;
    }

    // private static DatabaseEnumOptionsDataModel GetConfiguration(ImmutableArray<ClassDeclarationSyntax> classDeclarations)
    // {
    //     // Look for the ConfigurationAttribute
    //     foreach (var classDeclaration in classDeclarations)
    //     {
    //         AttributeSyntax? attribute = classDeclaration.AttributeLists
    //             .SelectMany(al => al.Attributes)
    //             .FirstOrDefault(attr => attr.Name.ToString() == nameof(DatabaseEnumOptionsAttribute));
    //
    //         // Extract the properties of the ConfigurationAttribute
    //         SeparatedSyntaxList<AttributeArgumentSyntax>? namedArguments = attribute?.ArgumentList?.Arguments;
    //         attribute.
    //         if (namedArguments != null)
    //         {
    //             string? enumNameMaxLength = namedArguments
    //                 .Value
    //                 .FirstOrDefault(arg =>
    //                     arg.NameEquals?.Name.ToString() == nameof(DatabaseEnumOptionsAttribute.EnumNameMaxLength))
    //                 ?.Expression.ToString().Trim('"');
    //             return new DatabaseEnumOptionsDataModel
    //             {
    //                 EntityNameFormat = namedArguments
    //                     .Value
    //                     .FirstOrDefault(arg =>
    //                         arg.NameEquals?.Name.ToString() == nameof(DatabaseEnumOptionsAttribute.EntityNameFormat))
    //                     ?.Expression.ToString().Trim('"') ?? "{0}Entity",
    //                 EnumNameMaxLength = enumNameMaxLength is not null ? int.Parse(enumNameMaxLength) : 255,
    //                 EnumNamespace = namedArguments
    //                     .Value
    //                     .FirstOrDefault(arg =>
    //                         arg.NameEquals?.Name.ToString() == nameof(DatabaseEnumOptionsAttribute.EnumNamespace))
    //                     ?.Expression.ToString().Trim('"') ?? string.Empty,
    //                 ConfigurationNamespace = namedArguments
    //                     .Value
    //                     .FirstOrDefault(arg =>
    //                         arg.NameEquals?.Name.ToString() == nameof(DatabaseEnumOptionsAttribute.ConfigurationNamespace))
    //                     ?.Expression.ToString().Trim('"') ?? string.Empty,
    //                 SchemaName = namedArguments
    //                     .Value
    //                     .FirstOrDefault(arg =>
    //                         arg.NameEquals?.Name.ToString() == nameof(DatabaseEnumOptionsAttribute.SchemaName))
    //                     ?.Expression.ToString().Trim('"') ?? string.Empty,
    //             };
    //         }
    //     }
    //
    //     return new DatabaseEnumOptionsDataModel();
    // }

    // private List<(string ActionName, int Order)> GetActionAttributes(ClassDeclarationSyntax classDeclaration)
    // {
    //     // Collect the ActionAttributes applied to the class
    //     var actions = new List<(string ActionName, int Order)>();
    //
    //     var actionAttributes = classDeclaration.AttributeLists
    //         .SelectMany(al => al.Attributes)
    //         .Where(attr => attr.Name.ToString() == "ActionAttribute");
    //
    //     var firstAttribute = classDeclaration.AttributeLists[0].Attributes.First();
    //
    //     foreach (var actionAttribute in actionAttributes)
    //     {
    //         var namedArguments = actionAttribute.ArgumentList?.Arguments;
    //         if (namedArguments != null)
    //         {
    //             var actionName = namedArguments.Value
    //                 .FirstOrDefault(arg => arg.NameEquals?.Name?.ToString() == "ActionName")
    //                 ?.Expression.ToString().Trim('"');
    //
    //             var order = 0;
    //             var orderArg = namedArguments.Value
    //                 .FirstOrDefault(arg => arg.NameEquals?.Name?.ToString() == "Order")
    //                 ?.Expression.ToString();
    //             if (orderArg != null)
    //             {
    //                 int.TryParse(orderArg, out order);
    //             }
    //
    //             if (actionName != null)
    //             {
    //                 actions.Add((actionName, order));
    //             }
    //         }
    //     }
    //
    //     return actions;
    // }

    private static DatabaseEnumOptionsDataModel GetDatabaseEnumOptionsDataModel(INamedTypeSymbol attributeClass)
    {
        ImmutableArray<AttributeData> attributes = attributeClass.ContainingType.GetAttributes();

        AttributeData? optionsAttribute = attributes.FirstOrDefault(attribute =>
            attribute.AttributeClass?.Name == nameof(DatabaseEnumOptionsAttribute));
        if (optionsAttribute is null)
        {
            return new DatabaseEnumOptionsDataModel();
        }

        KeyValuePair<string, TypedConstant>? configurationNamespace =
            optionsAttribute.NamedArguments.FirstOrDefault(arg =>
                arg.Key == nameof(DatabaseEnumOptionsAttribute.ConfigurationNamespace));
        KeyValuePair<string, TypedConstant>? enumNamespace =
            optionsAttribute.NamedArguments.FirstOrDefault(arg =>
                arg.Key == nameof(DatabaseEnumOptionsAttribute.EnumNamespace));
        KeyValuePair<string, TypedConstant>? schemaName =
            optionsAttribute.NamedArguments.FirstOrDefault(arg =>
                arg.Key == nameof(DatabaseEnumOptionsAttribute.SchemaName));
        KeyValuePair<string, TypedConstant>? entityNameFormat =
            optionsAttribute.NamedArguments.FirstOrDefault(arg =>
                arg.Key == nameof(DatabaseEnumOptionsAttribute.EntityNameFormat));
        KeyValuePair<string, TypedConstant>? enumNameMaxLength =
            optionsAttribute.NamedArguments.FirstOrDefault(arg =>
                arg.Key == nameof(DatabaseEnumOptionsAttribute.EnumNameMaxLength));
        return new DatabaseEnumOptionsDataModel
        {
            ConfigurationNamespace = configurationNamespace?.Value.ToString() ?? string.Empty,
            EnumNamespace = enumNamespace?.Value.ToString() ?? string.Empty,
            SchemaName = schemaName.Value.ToString(),
            EntityNameFormat = entityNameFormat?.Value.ToString() ?? "{0}Entity",
            EnumNameMaxLength = enumNameMaxLength is not null ? int.Parse(enumNameMaxLength.Value.ToString()) : 255,
        };
    }

    private static (DatabaseEnumOptionsDataModel, List<DatabaseEnumDataModel>) GetDatabaseEnumDataModel(GeneratorAttributeSyntaxContext context)
    {
        if (context.TargetSymbol is not INamedTypeSymbol typeSymbol)
        {
            return (new DatabaseEnumOptionsDataModel(),[]);
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
            }
        }

        return (databaseEnumOptionsDataModel, databaseEnumDataModels);
    }

    private static void GenerateCode(SourceProductionContext context,
        ImmutableArray<(DatabaseEnumOptionsDataModel, List<DatabaseEnumDataModel>)> dataModels)
    {
        foreach ((DatabaseEnumOptionsDataModel databaseEnumOptionsDataModel, List<DatabaseEnumDataModel> databaseEnumDataModels) in dataModels)
        {
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
