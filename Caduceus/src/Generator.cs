using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

[Generator]
sealed class Generator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var applies = context.SyntaxProvider.ForAttributeWithMetadataName("DeviceOfHermes.Derive.DeriveAttribute", static (node, _) => node is ClassDeclarationSyntax, static (ctx, _) => new AppliedInfo((INamedTypeSymbol)ctx.TargetSymbol, (ClassDeclarationSyntax)ctx.TargetNode, ctx.Attributes))
            .Select(static (info, _) =>
            {
                if (!info.Syntax.Modifiers.Any(md => md.IsKind(SyntaxKind.PartialKeyword)))
                {
                    return Result<AppliedInfo>.Err(Diagnostic.Create(DiagnosticDescriptor.MustBePartial, info.Syntax.Identifier.GetLocation(), $"{info.Syntax.Identifier.Value}"));
                }

                return Result<AppliedInfo>.Ok(info);
            });

        context.RegisterSourceOutput(applies, static (ctx, v) =>
        {
            if (!v.IsOk)
            {
                ctx.ReportDiagnostic(v.Error!);
            }
        });

        var _ = applies.Where(static res => res.IsOk)
            .Select(static (res, _) => res.value);
    }
}

record struct AppliedInfo(INamedTypeSymbol Ty, ClassDeclarationSyntax Syntax, ImmutableArray<AttributeData> Attributes);
