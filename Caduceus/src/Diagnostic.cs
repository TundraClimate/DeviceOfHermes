using Microsoft.CodeAnalysis;

internal static class DiagnosticExt
{
    extension(DiagnosticDescriptor)
    {
        public static DiagnosticDescriptor Debug => new("CDC0000", "Debug title", "Debug: {0}", "Debug category", DiagnosticSeverity.Info, true);
    }
}
