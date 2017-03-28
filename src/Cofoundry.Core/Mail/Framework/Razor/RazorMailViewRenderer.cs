using System;
using System.IO;

namespace Cofoundry.Core.Mail.Framework.Razor
{
    // https://github.com/aspnet/Entropy/blob/master/samples/Mvc.RenderViewToString/RazorViewToStringRenderer.cs
    // http://stackoverflow.com/questions/38247080/using-razor-outside-of-mvc-in-net-core
    public class RazorViewToStringRenderer
    {
        //private IRazorViewEngine _viewEngine;
        //private ITempDataProvider _tempDataProvider;
        //private IServiceProvider _serviceProvider;

        //public RazorViewToStringRenderer(
        //    IRazorViewEngine viewEngine,
        //    ITempDataProvider tempDataProvider,
        //    IServiceProvider serviceProvider)
        //{
        //    _viewEngine = viewEngine;
        //    _tempDataProvider = tempDataProvider;
        //    _serviceProvider = serviceProvider;
        //}

        //public string RenderViewToString<TModel>(string name, TModel model)
        //{
        //    var actionContext = GetActionContext();

        //    var viewEngineResult = _viewEngine.FindView(actionContext, name, false);

        //    if (!viewEngineResult.Success)
        //    {
        //        throw new InvalidOperationException(string.Format("Couldn't find view '{0}'", name));
        //    }

        //    var view = viewEngineResult.View;

        //    using (var output = new StringWriter())
        //    {
        //        var viewContext = new ViewContext(
        //            actionContext,
        //            view,
        //            new ViewDataDictionary<TModel>(
        //                metadataProvider: new EmptyModelMetadataProvider(),
        //                modelState: new ModelStateDictionary())
        //            {
        //                Model = model
        //            },
        //            new TempDataDictionary(
        //                actionContext.HttpContext,
        //                _tempDataProvider),
        //            output,
        //            new HtmlHelperOptions());

        //        view.RenderAsync(viewContext).GetAwaiter().GetResult();

        //        return output.ToString();
        //    }
        //}

        //private ActionContext GetActionContext()
        //{
        //    var httpContext = new DefaultHttpContext();
        //    httpContext.RequestServices = _serviceProvider;
        //    return new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        //}
    }
}
