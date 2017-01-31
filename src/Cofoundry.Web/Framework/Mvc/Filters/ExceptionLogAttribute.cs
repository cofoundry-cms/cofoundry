using Cofoundry.Core.ErrorLogging;
using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Cofoundry.Web
{
    /// <summary>
    /// 
    /// </summary>
    /// <see cref="http://www.codeproject.com/Articles/422572/Exception-Handling-in-ASP-NET-MVC"/>
    public class ExceptionLogAttribute : HandleErrorAttribute
    {
        public ExceptionLogAttribute()
        {
        }

        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }
            if (filterContext.IsChildAction)
            {
                return;
            }

            // If custom errors are disabled, we need to let the normal ASP.NET exception handler
            // execute so that the user can see useful debugging information.
            if (filterContext.ExceptionHandled)// || !filterContext.HttpContext.IsCustomErrorEnabled
            {
                return;
            }

            Exception exception = filterContext.Exception;

            // If this is not an HTTP 500 (for example, if somebody throws an HTTP 404 from an action method),
            // ignore it.
            if (new HttpException(null, exception).GetHttpCode() != 500)
            {
                return;
            }

            if (!ExceptionType.IsInstanceOfType(exception))
            {
                return;
            }

            // Log the exception
            LogException(filterContext.Exception);

            // If we're in the middle of a redirect, let it continue.
            if (filterContext.HttpContext.Response.IsRequestBeingRedirected) return;

            // if the request is AJAX return JSON else view.
            if (filterContext.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                filterContext.Result = new JsonResult
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = new
                    {
                        error = true,
                        message = filterContext.Exception.Message
                    }
                };
            }
            else
            {
                string controllerName = (string)filterContext.RouteData.Values["controller"];
                string actionName = (string)filterContext.RouteData.Values["action"];

                ErrorPageViewModel viewModel = new ErrorPageViewModel(filterContext.Exception, controllerName, actionName)
                {
                    MetaDescription = "Sorry, there has been an error",
                    PageTitle = "Error"
                };

                ViewResult result = new ViewResult
                {
                    ViewName = View,
                    MasterName = Master,
                    ViewData = new ViewDataDictionary<ErrorPageViewModel>(viewModel),
                    TempData = filterContext.Controller.TempData
                };
                filterContext.Result = result;
            }


            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.Clear();
            filterContext.HttpContext.Response.StatusCode = 500;
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
        }

        private void LogException(Exception ex)
        {
            var errorLogginService = IckyDependencyResolution.ResolveFromMvcContext<IErrorLoggingService>();
            errorLogginService.Log(ex);
        }
    }
}
