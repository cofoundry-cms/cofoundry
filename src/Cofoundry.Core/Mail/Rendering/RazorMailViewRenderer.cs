using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

namespace Cofoundry.Core.Mail.Internal;

/// <summary>
/// Renders mail template view files using a razor view engine and a 
/// fake (out of request) ViewContext.
/// </summary>
/// <remarks>
/// Rendering technique adapted from this example:  
/// https://github.com/aspnet/Entropy/blob/master/samples/Mvc.RenderViewToString/RazorViewToStringRenderer.cs
/// </remarks>
public class RazorMailViewRenderer : IMailViewRenderer
{
    private readonly IRazorViewEngine _viewEngine;
    private readonly ITempDataProvider _tempDataProvider;
    private readonly IServiceProvider _serviceProvider;

    public RazorMailViewRenderer(
        IRazorViewEngine viewEngine,
        ITempDataProvider tempDataProvider,
        IServiceProvider serviceProvider)
    {
        _viewEngine = viewEngine;
        _tempDataProvider = tempDataProvider;
        _serviceProvider = serviceProvider;
    }

    public async Task<string?> RenderAsync(string viewPath)
    {
        var viewData = new ViewDataDictionary(
            metadataProvider: new EmptyModelMetadataProvider(),
            modelState: new ModelStateDictionary()
            );

        var result = await RenderInternalAsync(viewPath, viewData);
        return result;
    }

    public async Task<string?> RenderAsync<TModel>(string viewPath, TModel model)
    {
        var viewData = new ViewDataDictionary<TModel>(
            metadataProvider: new EmptyModelMetadataProvider(),
            modelState: new ModelStateDictionary())
        {
            Model = model
        };

        var result = await RenderInternalAsync(viewPath, viewData);
        return result;
    }

    private async Task<string?> RenderInternalAsync(string viewPath, ViewDataDictionary viewData)
    {
        var actionContext = GetActionContext();

        ViewEngineResult viewEngineResult;
        if (!string.IsNullOrEmpty(viewPath) && IsApplicationRelativePath(viewPath))
        {
            viewEngineResult = _viewEngine.GetView(null, viewPath, false);
        }
        else
        {
            viewEngineResult = _viewEngine.FindView(actionContext, viewPath, false);
        }

        if (!viewEngineResult.Success)
        {
            return null;
        }

        var view = viewEngineResult.View;

        using (var output = new StringWriter())
        {
            var viewContext = new ViewContext(
                actionContext,
                view,
                viewData,
                new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
                output,
                new HtmlHelperOptions());

            await view.RenderAsync(viewContext);

            return output.ToString();
        }
    }

    private ActionContext GetActionContext()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.RequestServices = _serviceProvider;
        return new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
    }

    private static bool IsApplicationRelativePath(string name)
    {
        return name[0] == '~' || name[0] == '/';
    }
}
