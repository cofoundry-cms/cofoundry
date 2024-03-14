using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web.Admin.Internal;

/// <summary>
/// Default implementation of <see cref="IVisualEditorActionResultFactory"/>.
/// </summary>
public class VisualEditorActionResultFactory : IVisualEditorActionResultFactory
{
    private readonly IVisualEditorScriptGenerator _visualEditorScriptGenerator;
    private readonly IHtmlDocumentScriptInjector _htmlDocumentScriptInjector;

    public VisualEditorActionResultFactory(
        IVisualEditorScriptGenerator visualEditorScriptGenerator,
        IHtmlDocumentScriptInjector htmlDocumentScriptInjector
        )
    {
        _visualEditorScriptGenerator = visualEditorScriptGenerator;
        _htmlDocumentScriptInjector = htmlDocumentScriptInjector;
    }

    public IActionResult Create(IActionResult wrappedActionResult)
    {
        return new VisualEditorActionResult(
            wrappedActionResult,
            _visualEditorScriptGenerator,
            _htmlDocumentScriptInjector
            );
    }
}
