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

        var impls = applies.Where(static res => res.IsOk)
            .Select(static (res, _) => res.value)
            .Select(static (info, ct) =>
            {
                var list = new List<INamedTypeSymbol>();

                foreach (var attribute in info.Attributes)
                {
                    if (attribute.AttributeClass?.ToDisplayString() != "DeviceOfHermes.Derive.DeriveAttribute")
                    {
                        continue;
                    }

                    var args = attribute.ConstructorArguments.FirstOrDefault().Values;

                    if (args.IsEmpty)
                    {
                        continue;
                    }

                    foreach (var ty in args)
                    {
                        var sym = ty.Value as INamedTypeSymbol;

                        if (sym is not null && sym.GetAttributes().FirstOrDefault(attr => attr.AttributeClass?.ToDisplayString() == "DeviceOfHermes.Derive.DeriveTemplateAttribute") is AttributeData template)
                        {
                            if (template.ConstructorArguments.FirstOrDefault().Value is INamedTypeSymbol templateSpecified)
                            {
                                if (IsImplementType(info.Ty, templateSpecified))
                                {
                                    list.Add(sym);
                                }
                                else
                                {
                                    return Result<ImplementationInfo>.Err(Diagnostic.Create(DiagnosticDescriptor.MustImplesRequired, info.Ty.Locations.FirstOrDefault(), info.Ty.Name, sym.Name, templateSpecified.Name));
                                }
                            }
                        }
                        else
                        {
                            return Result<ImplementationInfo>.Err(Diagnostic.Create(DiagnosticDescriptor.MustSpecifyTemplate, attribute.ApplicationSyntaxReference?.GetSyntax(ct).GetLocation(), ty.Value));
                        }
                    }
                }

                return Result<ImplementationInfo>.Ok(new(info.Ty, list.ToImmutableArray()));
            });

        context.RegisterSourceOutput(impls, static (ctx, v) =>
        {
            if (!v.IsOk)
            {
                ctx.ReportDiagnostic(v.Error!);
            }
        });
    }

    static bool IsImplementType(INamedTypeSymbol target, INamedTypeSymbol candidate)
    {
        if (target.BaseType is null)
        {
            return false;
        }

        if (target.BaseType.ToDisplayString() == "System.Object")
        {
            return candidate.ToDisplayString() == "System.Object";
        }

        if (SymbolEqualityComparer.Default.Equals(target.BaseType, candidate))
        {
            return true;
        }

        return IsImplementType(target.BaseType, candidate);
    }
}

record struct AppliedInfo(INamedTypeSymbol Ty, ClassDeclarationSyntax Syntax, ImmutableArray<AttributeData> Attributes);

record struct ImplementationInfo(INamedTypeSymbol Ty, ImmutableArray<INamedTypeSymbol> Templates);
