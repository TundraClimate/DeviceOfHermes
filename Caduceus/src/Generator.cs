using System.Collections.Concurrent;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

[Generator]
sealed class Generator : IIncrementalGenerator
{
    const string DERIVE_ATTR = "DeviceOfHermes.Derive.DeriveAttribute";
    const string DERIVE_TEMPLATE_ATTR = "DeviceOfHermes.Derive.DeriveTemplateAttribute";
    const string DERIVE_USAGE_ATTR = "DeviceOfHermes.Derive.DeriveUsageAttribute";
    const string DERIVE_PRIORITY_ATTR = "DeviceOfHermes.Derive.DerivePriorityAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var output = context.SyntaxProvider.ForAttributeWithMetadataName(DERIVE_ATTR, static (node, _) => node is ClassDeclarationSyntax, InitTransform)
            .Unwrap(context)
            .Select(GetCandidates);

        context.RegisterSourceOutput(output, GenerateAdditionalSource);
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

    static CandidatesInfo GetCandidates(ImplementInfo? res, CancellationToken ct)
    {
        var info = res!.Value;
        var methods = info.Templates.SelectMany(t => t.GetMembers().OfType<IMethodSymbol>());

        ConcurrentDictionary<IMethodSymbol, List<(int, IMethodSymbol)>> methodPriorityList = new(SymbolEqualityComparer.Default);

        foreach (var method in methods)
        {
            if (method.FindAttribute(DERIVE_USAGE_ATTR) is not AttributeData usage || usage.Get(0).Value is not INamedTypeSymbol targetType || usage.Get(1).Value is not string targetName || !info.Applies.IsImplemented(targetType))
            {
                continue;
            }

            var targetParams = !usage.Get(2).Values.IsDefault ? usage.Get(2).Values.Select(v => v.Value).Cast<ITypeSymbol>() : null;

            if (targetType.FindMethod(targetName, targetParams) is not IMethodSymbol targetMethod || !targetMethod.IsRootVirtual() || !info.Applies.CanOverride(targetMethod))
            {
                continue;
            }

            var priority = method.FindAttribute(DERIVE_PRIORITY_ATTR)?.Get(0).Value as int? ?? 0;

            methodPriorityList.GetOrAdd(targetMethod, _ => new()).Add((priority, method));
        }

        var dict = methodPriorityList.ToImmutableDictionary<KeyValuePair<IMethodSymbol, List<(int, IMethodSymbol)>>, IMethodSymbol, ImmutableArray<IMethodSymbol>>(e => e.Key, e => e.Value.OrderByDescending(v => v.Item1).Select(v => v.Item2).ToImmutableArray(), SymbolEqualityComparer.Default);

        return new(info.Applies, dict);
    }

    static void GenerateAdditionalSource(SourceProductionContext ctx, CandidatesInfo info)
    {
        var methods = info.Candidates.Select(cand =>
        {
            var baseMethod = cand.Key.ConvertOverrideMethodSyntax();
            List<List<StatementSyntax>> body = [[cand.Key.GenerateBaseInvokeStatement()], cand.Value.Select(c => c.GenerateStaticInvokeStatement(cand.Key)).ToList()];

            return baseMethod.WithBody(SyntaxFactory.Block(body.SelectMany(s => s)));
        });

        var cls = (ClassDeclarationSyntax)info.Applies.DeclaringSyntaxReferences.First().GetSyntax();

        cls = cls.WithAttributeLists(default).WithMembers(default).WithoutTrivia()
            .AddMembers(methods.ToArray());

        var ns = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(info.Applies.ContainingNamespace.ToDisplayString()))
            .AddMembers(cls);

        var fileName = $"CADUCEUS_{info.Applies.ToDisplayString()}.g.cs";
        var compilation = SyntaxFactory.CompilationUnit().AddMembers(ns)
            .NormalizeWhitespace().ToFullString();

        ctx.AddSource(fileName, compilation);
    }
}

record struct Result<T>(T Value, ImmutableArray<Diagnostic> Diagnostics);

record struct ImplementInfo(INamedTypeSymbol Applies, ImmutableArray<INamedTypeSymbol> Templates);

record struct CandidatesInfo(INamedTypeSymbol Applies, ImmutableDictionary<IMethodSymbol, ImmutableArray<IMethodSymbol>> Candidates);
