using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

internal static class Ext
{
    extension<T>(IncrementalValuesProvider<Result<T>> provider)
    {
        public IncrementalValuesProvider<T> Unwrap(IncrementalGeneratorInitializationContext ctx)
        {
            ctx.RegisterSourceOutput(provider, static (ctx, res) =>
            {
                foreach (var di in res.Diagnostics)
                {
                    ctx.ReportDiagnostic(di);
                }
            });

            return provider.Where(static res => res.Value is not null).Select(static (res, _) => res.Value);
        }
    }

    extension(INamedTypeSymbol symbol)
    {
        public AttributeData? FindAttribute(string metadataName)
        {
            return symbol.GetAttributes().FirstOrDefault(attr => attr.AttributeClass?.ToDisplayString() == metadataName);
        }

        public bool IsImplemented(INamedTypeSymbol spec)
        {
            if (symbol.BaseType is null)
            {
                return false;
            }

            if (SymbolEqualityComparer.Default.Equals(symbol.BaseType, spec))
            {
                return true;
            }

            return IsImplemented(symbol.BaseType, spec);
        }

        public IMethodSymbol? FindMethod(string methodName, IEnumerable<ITypeSymbol>? methodParams = null)
        {
            var methods = symbol.GetMembers().OfType<IMethodSymbol>();

            if (methodParams is null)
            {
                return methods.FirstOrDefault(m => m.Name == methodName);
            }

            return methods.FirstOrDefault(m => m.Name == methodName && m.Parameters.Select(p => p.Type).SequenceEqual(methodParams, SymbolEqualityComparer.Default));
        }

        public bool CanOverride(IMethodSymbol rootMethod)
        {
            var name = rootMethod.Name;
            var methodParams = rootMethod.Parameters.Select(p => p.Type);

            if (symbol.FindMethod(name, methodParams) is IMethodSymbol find && find.IsOverride)
            {
                return false;
            }

            var targetCls = symbol.BaseType;

            while (targetCls is not null)
            {
                if (targetCls.FindMethod(name, methodParams) is IMethodSymbol m && m.IsOverride && m.IsSealed)
                {
                    return false;
                }

                targetCls = targetCls.BaseType;
            }

            return true;
        }
    }

    extension(IMethodSymbol symbol)
    {
        public AttributeData? FindAttribute(string metadataName)
        {
            return symbol.GetAttributes().FirstOrDefault(attr => attr.AttributeClass?.ToDisplayString() == metadataName);
        }

        public bool IsRootVirtual()
        {
            return symbol.IsVirtual
                && !symbol.IsOverride
                && !symbol.IsStatic
                && !symbol.IsSealed
                && symbol.DeclaredAccessibility is
                    Accessibility.Public or
                    Accessibility.Protected;
        }

        public MethodDeclarationSyntax ConvertOverrideMethodSyntax()
        {
            var returnType = SyntaxFactory.ParseTypeName(symbol.ReturnType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
            var parameters = symbol.Parameters.Select(ConvertParameterSyntax).ToArray();

            var vis = symbol.DeclaredAccessibility switch
            {
                Accessibility.Public => SyntaxKind.PublicKeyword,
                Accessibility.Protected => SyntaxKind.ProtectedKeyword,
                Accessibility.ProtectedAndInternal => SyntaxKind.ProtectedKeyword,
                Accessibility.Internal => SyntaxKind.InternalKeyword,
                Accessibility.Private => SyntaxKind.PrivateKeyword,
                Accessibility.ProtectedOrInternal => SyntaxKind.ProtectedKeyword,

                _ => SyntaxKind.None
            };

            var accessibility = vis != SyntaxKind.None ? SyntaxFactory.Token(vis) : default;

            var method = SyntaxFactory.MethodDeclaration(returnType, SyntaxFactory.Identifier(symbol.Name))
                .AddModifiers(accessibility, SyntaxFactory.Token(SyntaxKind.OverrideKeyword))
                .AddParameterListParameters(parameters)
                .WithLeadingTrivia(SyntaxFactory.ParseLeadingTrivia("/// <inheritdoc/>\r\n"));

            return method;
        }

        public StatementSyntax GenerateBaseInvokeStatement()
        {
            var expr = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.BaseExpression(), SyntaxFactory.IdentifierName(symbol.Name));
            var args = SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(symbol.Parameters.Select(p => SyntaxFactory.Argument(SyntaxFactory.IdentifierName(p.Name)))));
            var invoke = SyntaxFactory.InvocationExpression(expr, args);

            return SyntaxFactory.ExpressionStatement(invoke);
        }

        public StatementSyntax GenerateStaticInvokeStatement(IMethodSymbol correspondedMethod)
        {
            ArgumentSyntax[][] argList = [[SyntaxFactory.Argument(SyntaxFactory.ThisExpression())], correspondedMethod.Parameters.Select(p => SyntaxFactory.Argument(SyntaxFactory.IdentifierName(p.Name))).ToArray()];

            var expr = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.ParseTypeName(symbol.ContainingType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)), SyntaxFactory.IdentifierName(symbol.Name));
            var args = SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(argList.Take(symbol.Parameters.Length).SelectMany(a => a)));
            var invoke = SyntaxFactory.InvocationExpression(expr, args);

            return SyntaxFactory.ExpressionStatement(invoke);
        }
    }

    extension(IParameterSymbol symbol)
    {
        public ParameterSyntax ConvertParameterSyntax()
        {
            var type = SyntaxFactory.ParseTypeName(symbol.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
            var syntax = SyntaxFactory.Parameter(SyntaxFactory.Identifier(symbol.Name)).WithType(type);

            return symbol.RefKind switch
            {
                RefKind.Ref => syntax.WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.RefKeyword))),
                RefKind.Out => syntax.WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.OutKeyword))),
                RefKind.In => syntax.WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.InKeyword))),

                _ => syntax
            };
        }
    }

    extension(AttributeData attribute)
    {
        public TypedConstant Get(int index)
        {
            var args = attribute.ConstructorArguments;

            if (index >= args.Length)
            {
                return default;
            }

            return args[index];
        }
    }
}
