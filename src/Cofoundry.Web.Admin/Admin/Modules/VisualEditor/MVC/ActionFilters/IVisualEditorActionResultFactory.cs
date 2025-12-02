using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web.Admin.Internal;

/// <summary>
/// Factory for generating the VisualEditorActionResult
/// that wraps the current action current AxctionResult
/// and modified it to include visual editor scripts.
/// </summary>
public interface IVisualEditorActionResultFactory
{
    IActionResult Create(IActionResult wrappedActionResult);
}
