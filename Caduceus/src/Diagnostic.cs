using Microsoft.CodeAnalysis;

internal static class DiagnosticExt
{
    const string USAGE = "Usage";

    extension(DiagnosticDescriptor)
    {
        public static DiagnosticDescriptor Debug => new("CDC0000", "Debug title", "Debug: {0}", "Debug category", DiagnosticSeverity.Info, true);

        public static DiagnosticDescriptor MustBePartial => new("CDC0001", "Class must be declured partial", "A class '{0}' must be declured partial", USAGE, DiagnosticSeverity.Error, true);

        public static DiagnosticDescriptor MustSpecifyTemplate => new("CDC0002", "Derive arguments must be specified Template type", "A class specified '{0}' is not Template type", USAGE, DiagnosticSeverity.Error, true);

        public static DiagnosticDescriptor MustImplesRequired => new("CDC0003", "Derived class must be implements the Template spcified class", "{1}: A class '{0}' must be implements the '{2}'", USAGE, DiagnosticSeverity.Error, true);
    }
}
