using Common.Generators.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Generators;

[Generator]
public class EFConfigGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsSyntaxTargetForGeneration(s),
                transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx)
            )
            .Where(static m => m is not null);

        context.RegisterSourceOutput(classDeclarations,
            static (ctx, source) => SourceOutput(ctx, source));

        context.RegisterPostInitializationOutput(PostInitializationOutput);
    }
    private static readonly Dictionary<string, int> _countPerFileName = [];
    private static void SourceOutput(SourceProductionContext context, ClassToGenerate? classToGenerate)
    {
        if (classToGenerate is null)
        {
            return;
        }

        var namespaceName = classToGenerate.NamespaceName;
        var className = classToGenerate.ClassName;
        var fileName = $"{namespaceName}.{className}.g.cs";

        if (_countPerFileName.ContainsKey(fileName))
        {
            _countPerFileName[fileName]++;
        }
        else
        {
            _countPerFileName[fileName] = 1;
        }

        var configurations = string.Join("\n",
            classToGenerate.Configs.Select(config => $$"""
                        {
                            var config = new {{config.ConfigTypeName}}();
                            config.Configure(modelBuilder.Entity<{{config.EntityTypeName}}>());
                        }
            """)
        );

        var source = $$"""
        // Generation count: {{_countPerFileName[fileName]}}
        namespace {{namespaceName}}
        {
            partial class {{className}}
            {
                public void ApplyAutoConfigurations(global::Microsoft.EntityFrameworkCore.ModelBuilder modelBuilder)
                {
        {{configurations}}
                }
            }
        }
        """;

        context.AddSource(fileName, source);
    }



    private static void PostInitializationOutput(IncrementalGeneratorPostInitializationContext context)
    {
        context.AddSource(
            hintName: "Common.Generators.AutoConfigurableAttribute.g.cs",
            source: """
            namespace Common.Generators
            {
                /// <summary>
                /// Marks a class to be auto-configured by the EFConfigGenerator. <br/>
                /// Pass the entity (table) type and the configuration type that implements IEntityTypeConfiguration&lt;T&gt;.
                /// </summary>
                [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true)]
                internal sealed class AutoConfigurableAttribute : System.Attribute
                {
                    public AutoConfigurableAttribute(System.Type entityType, System.Type configType)
                    {
                        EntityType = entityType;
                        ConfigType = configType;
                    }

                    public System.Type EntityType { get; }
                    public System.Type ConfigType { get; }
                }
            }
            
            """
        );
    }

    private static ClassToGenerate? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        if (context.Node is not ClassDeclarationSyntax classDeclarationSyntax)
            return null;

        if (context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax) is not INamedTypeSymbol classSymbol)
            return null;

        var attributeSymbol = context.SemanticModel.Compilation
            .GetTypeByMetadataName("Common.Generators.AutoConfigurableAttribute");

        if (attributeSymbol is null)
            return null;

        var configTypeNames = classSymbol.GetAttributes()
            .Where(attr => SymbolEqualityComparer.Default.Equals(attr.AttributeClass, attributeSymbol))
            .Select(attr =>
            {
                var entityType = attr.ConstructorArguments.ElementAtOrDefault(0).Value as INamedTypeSymbol;
                var configType = attr.ConstructorArguments.ElementAtOrDefault(1).Value as INamedTypeSymbol;

                if (entityType == null || configType == null)
                    return null;

                return new ClassToGenerate.ConfigMapping(
                    entityType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                    configType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                );
            })
            .Where(x => x is not null)
            .Select(x => x!)
            .ToList();

        if (configTypeNames.Count == 0)
            return null;

        return new ClassToGenerate(
            classSymbol.ContainingNamespace.ToDisplayString(),
            classSymbol.Name,
            configTypeNames
        );
    }


    private static bool IsSyntaxTargetForGeneration(SyntaxNode syntaxNode)
    {
        return syntaxNode is ClassDeclarationSyntax classDeclarationSyntax
            && classDeclarationSyntax.AttributeLists.Count > 0;
    }
}
