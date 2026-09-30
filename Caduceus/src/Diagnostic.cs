using Microsoft.CodeAnalysis;

internal static class DiagnosticExt
{
    const string USAGE = "Usage";

    extension(DiagnosticDescriptor)
    {
        public static DiagnosticDescriptor Debug => new("CDC0000", "Debug title", "Debug: {0}", "Debug category", DiagnosticSeverity.Info, true);

        public static DiagnosticDescriptor MustBePartial => new("CDC0001", "Class must be declured partial", "A class '{0}' must be declured partial", USAGE, DiagnosticSeverity.Error, true);
    }
}
