using Microsoft.CodeAnalysis;

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
