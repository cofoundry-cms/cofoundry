using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Cofoundry.Core.Mail.Internal
{
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
        private IRazorViewEngine _viewEngine;
        private ITempDataProvider _tempDataProvider;
        private IServiceProvider _serviceProvider;

        public RazorMailViewRenderer(
            IRazorViewEngine viewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider)
        {
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
        }

        public Task<string> RenderAsync(string viewPath)
        {
            return RenderAsync<dynamic>(viewPath, null);
        }

        public async Task<string> RenderAsync<TModel>(string viewPath, TModel model)
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
                throw new InvalidOperationException(string.Format("Couldn't find mail template '{0}'", viewPath));
            }

            var view = viewEngineResult.View;

            using (var output = new StringWriter())
            {
                var viewContext = new ViewContext(
                    actionContext,
                    view,
                    new ViewDataDictionary<TModel>(
                        metadataProvider: new EmptyModelMetadataProvider(),
                        modelState: new ModelStateDictionary())
                    {
                        Model = model
                    },
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
}
