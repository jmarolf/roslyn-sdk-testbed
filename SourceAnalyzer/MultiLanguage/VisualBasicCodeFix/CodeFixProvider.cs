namespace VisualBasicCodeFix;

[ExportCodeFixProvider(LanguageNames.VisualBasic, Name = nameof(VisualBasicCodeFixProvider)), Shared]
public class VisualBasicCodeFixProvider : CodeFixProvider
{
    public sealed override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create("AnalyzerDiagnosticId");

    public sealed override FixAllProvider GetFixAllProvider()
        => WellKnownFixAllProviders.BatchFixer;

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        var diagnostic = context.Diagnostics.First();
        var diagnosticSpan = diagnostic.Location.SourceSpan;
        var declaration = root?.FindToken(diagnosticSpan.Start).Parent?.AncestorsAndSelf().OfType<DeclareStatementSyntax>().First();

        if (declaration == null)
        {
            return;
        }

        context.RegisterCodeFix(
            CodeAction.Create(
                title: "Make Upper Case",
                createChangedSolution: c => MakeUppercaseAsync(context.Document, declaration, c),
                equivalenceKey: "CodeFixTitle"),
            diagnostic);
    }

    private static async Task<Solution> MakeUppercaseAsync(Document document, DeclareStatementSyntax typeDecl, CancellationToken cancellationToken)
    {
        var identifierToken = typeDecl.Identifier;
        var newName = identifierToken.Text.ToUpperInvariant();
        var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
        var typeSymbol = semanticModel.GetDeclaredSymbol(typeDecl, cancellationToken);
        var originalSolution = document.Project.Solution;
        var optionSet = originalSolution.Workspace.Options;
        var newSolution = await Renamer.RenameSymbolAsync(document.Project.Solution, typeSymbol, newName, optionSet, cancellationToken).ConfigureAwait(false);
        return newSolution;
    }
}