using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace KirisameLib.Core.SourceGenerator;

public class TypeWithAttributeGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var typeDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => node is TypeDeclarationSyntax,
                transform: static (context, _) => (TypeDeclarationSyntax)context.Node
            )
            .Where(static typeDeclaration => typeDeclaration.AttributeLists.Count > 0);
        //todo
    }
}