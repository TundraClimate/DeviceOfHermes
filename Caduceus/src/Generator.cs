using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

[Generator]
sealed class Generator : IIncrementalGenerator
{
    const string DERIVE_ATTR = "DeviceOfHermes.Derive.DeriveAttribute";
    const string DERIVE_TEMPLATE_ATTR = "DeviceOfHermes.Derive.DeriveTemplateAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.SyntaxProvider.ForAttributeWithMetadataName(DERIVE_ATTR, static (node, _) => node is ClassDeclarationSyntax, InitTransform)
            .Unwrap(context);
    }

    static Result<ImplementInfo?> InitTransform(GeneratorAttributeSyntaxContext ctx, CancellationToken ct)
    {
        var diagnostics = new List<Diagnostic>();
        var cls = (ClassDeclarationSyntax)ctx.TargetNode;
        var symbol = (INamedTypeSymbol)ctx.TargetSymbol;
        var derive = symbol.FindAttribute(DERIVE_ATTR)!;
        var args = derive.Get(0).Values;
        var templates = new List<INamedTypeSymbol>();

        foreach (var template in args)
        {
            var ty = (INamedTypeSymbol)template.Value!;

            if (ty.FindAttribute(DERIVE_TEMPLATE_ATTR)?.Get(0).Value is not INamedTypeSymbol mark)
            {
                diagnostics.Add(Diagnostic.Create(DiagnosticDescriptor.MustSpecifyTemplate, derive.ApplicationSyntaxReference?.GetSyntax(ct).GetLocation(), ty.Name));

                continue;
            }

            if (!symbol.IsImplemented(mark))
            {
                diagnostics.Add(Diagnostic.Create(DiagnosticDescriptor.MustImplesRequired, cls.Identifier.GetLocation(), symbol.Name, ty.Name, mark.Name));

                continue;
            }

            templates.Add(ty);
        }

        if (!cls.Modifiers.Any(mod => mod.IsKind(SyntaxKind.PartialKeyword)))
        {
            diagnostics.Add(Diagnostic.Create(DiagnosticDescriptor.MustBePartial, cls.Identifier.GetLocation(), $"{cls.Identifier.Value}"));

            return new(null, diagnostics.ToImmutableArray());
        }

        return new(new(symbol, templates.ToImmutableArray()), diagnostics.ToImmutableArray());
    }
}

record struct Result<T>(T Value, ImmutableArray<Diagnostic> Diagnostics);

record struct ImplementInfo(INamedTypeSymbol applies, ImmutableArray<INamedTypeSymbol> Templates);
