namespace VisualBasicAnalyzer;

[DiagnosticAnalyzer(LanguageNames.VisualBasic)]
public class Analyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "AnalyzerDiagnosticId";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        title: "Type name contains lowercase letters",
        messageFormat: "Type name '{0}' contains lowercase letters",
        description: "Type names should be all uppercase",
        category: "Naming",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // TODO: Implement Analyzer
        context.RegisterSyntaxNodeAction(AnalyzeSyntax, SyntaxKind.ClassStatement);
    }

    private void AnalyzeSyntax(SyntaxNodeAnalysisContext context)
    {
        var classStatement = (ClassStatementSyntax)context.Node;
        if (classStatement.Identifier.Text.ToCharArray().Any(char.IsLower))
        {
            var diagnostic = Diagnostic.Create(Rule, classStatement.Identifier.GetLocation(), classStatement.Identifier.Text);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
