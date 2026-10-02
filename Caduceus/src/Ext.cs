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
