using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Cofoundry.Web;

/// <inheritdoc/>
public class RazorViewRenderer : IRazorViewRenderer
{
    private readonly IRazorViewEngine _razorViewEngine;
    private ITempDataProvider _tempDataProvider;

    public RazorViewRenderer(
        IRazorViewEngine razorViewEngine,
        ITempDataProvider tempDataProvider
        )
    {
        _razorViewEngine = razorViewEngine;
        _tempDataProvider = tempDataProvider;
    }

    public Task<string> RenderViewAsync(
        ViewContext viewContext,
        string viewName
        )
    {
        var viewResult = FindView(viewContext, viewName);
        ValidateViewFound(viewName, viewResult);

        return RenderViewAsync(viewContext, viewResult, null);
    }

    public Task<string> RenderViewAsync(
        ViewContext viewContext,
        string viewName,
        object model
        )
    {
        var viewResult = FindView(viewContext, viewName);
        ValidateViewFound(viewName, viewResult);

        return RenderViewAsync(viewContext, viewResult, model);
    }

    public Task<string> RenderViewAsync(
        ActionContext actionContext,
        string viewName
        )
    {
        var viewResult = FindView(actionContext, viewName);
        ValidateViewFound(viewName, viewResult);

        return RenderViewAsync(actionContext, viewResult, null);
    }

    public Task<string> RenderViewAsync(
        ActionContext actionContext,
        string viewName,
        object model
        )
    {
        var viewResult = FindView(actionContext, viewName);
        ValidateViewFound(viewName, viewResult);

        return RenderViewAsync(actionContext, viewResult, model);
    }

    private ViewEngineResult FindView(ActionContext viewContext, string viewName)
    {
        ViewEngineResult viewResult;
        if (!string.IsNullOrEmpty(viewName) && IsApplicationRelativePath(viewName))
        {
            viewResult = _razorViewEngine.GetView(null, viewName, false);
        }
        else
        {
            viewResult = _razorViewEngine.FindView(viewContext, viewName, false);
        }

        ValidateViewFound(viewName, viewResult);
        return viewResult;
    }

    private static bool IsApplicationRelativePath(string name)
    {
        return name[0] == '~' || name[0] == '/';
    }

    private async Task<string> RenderViewAsync(ActionContext actionContext, ViewEngineResult viewResult, object model)
    {
        var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
        {
            Model = model
        };

        using (var sw = new StringWriter())
        {
            var componentViewContext = new ViewContext(
                actionContext,
                viewResult.View,
                viewDictionary,
                new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
                sw,
                new HtmlHelperOptions()
            );

            await viewResult.View.RenderAsync(componentViewContext);

            return sw.ToString();
        }
    }

    private async Task<string> RenderViewAsync(ViewContext viewContext, ViewEngineResult viewResult, object model)
    {
        var view = viewResult.View;

        var viewData = new ViewDataDictionary<object>(viewContext.ViewData, model);

        using (var sw = new StringWriter())
        using (view as IDisposable)
        {
            var componentViewContext = new ViewContext(
                viewContext,
                view,
                viewData,
                sw
            );

            await viewResult.View.RenderAsync(componentViewContext);

            return sw.ToString();
        }
    }

    private static void ValidateViewFound(string viewName, ViewEngineResult viewResult)
    {
        if (viewResult == null || viewResult.View == null)
        {
            throw new ViewNotFoundException(viewName, viewResult?.SearchedLocations);
        }
    }
}