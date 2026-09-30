using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

[Generator]
sealed class Generator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var prov = context.SyntaxProvider.ForAttributeWithMetadataName("DeviceOfHermes.Derive.DeriveAttribute", GetCandidate, Transform);

        context.RegisterSourceOutput(prov, GenerateAddtionalSource);
    }

    static bool GetCandidate(SyntaxNode node, CancellationToken token)
    {
        return node is ClassDeclarationSyntax;
    }

    static ISymbol? Transform(GeneratorAttributeSyntaxContext ctx, CancellationToken token)
    {
        return ctx.TargetSymbol;
    }

    static void GenerateAddtionalSource(SourceProductionContext ctx, ISymbol? symbol)
    {
    }
}
