using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    public class CofoundryErrorController : Controller
    {
        #region constructor

        private readonly INotFoundViewHelper _notFoundViewHelper;
        private readonly IPageViewModelBuilder _pageViewModelBuilder;
        private readonly IRazorViewEngine _razorViewEngine;

        public CofoundryErrorController(
            INotFoundViewHelper notFoundViewHelper,
            IPageViewModelBuilder pageViewModelBuilder,
            IRazorViewEngine razorViewEngine
            )
        {
            _notFoundViewHelper = notFoundViewHelper;
            _pageViewModelBuilder = pageViewModelBuilder;
            _razorViewEngine = razorViewEngine;
        }

        #endregion

        /// <summary>
        /// Route to be used with UseExceptionHandler(exceptionHandlingPath) when 
        /// using the default Cofoundry error handling page.
        /// </summary>
        public static string ExceptionHandlerPath = "/cofoundryerror/exception/";

        /// <summary>
        /// Route to be used with UseStatusCodePagesWithReExecute(pathFormat) when using
        /// Cofoundry default status code pages.
        /// </summary>
        public static string StatusCodePagesRoute = "/cofoundryerror/errorcode/{0}";

        public Task<IActionResult> Exception()
        {
            const int STATUS_CODE = 500;

            var feature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var request = HttpContext.Request;
                        
            var vmParameters = new ErrorPageViewModelBuilderParameters()
            {
                StatusCode = STATUS_CODE,
                Path = feature.Path,
                PathBase = request.PathBase,
                QueryString = request.QueryString.Value
            };

            return GetErrorView(STATUS_CODE, vmParameters);
        }

        public async Task<IActionResult> ErrorCode(int statusCode)
        {
            var request = HttpContext.Request;
            var feature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            if (statusCode == (int)HttpStatusCode.NotFound)
            {
                return await _notFoundViewHelper.GetViewAsync(this);
            }

            var vmParameters = new ErrorPageViewModelBuilderParameters()
            {
                StatusCode = statusCode,
                Path = feature?.OriginalPath ?? request.Path,
                PathBase = feature?.OriginalPathBase ?? request.PathBase,
                QueryString = feature?.OriginalQueryString ?? request.QueryString.Value
            };

            return await GetErrorView(statusCode, vmParameters);
        }

        private async Task<IActionResult> GetErrorView(int statusCode, ErrorPageViewModelBuilderParameters vmParameters)
        {
            var viewName = FindView(statusCode);
            var viewModel = await _pageViewModelBuilder.BuildErrorPageViewModelAsync(vmParameters);

            return View(viewName, viewModel);
        }

        private string FindView(int statusCode)
        {
            const string VIEW_FORMAT = "~/Views/Shared/{0}.cshtml";
            const string GENERIC_ERROR_VIEW = "~/Views/Shared/Error.cshtml";

            // Check first for exact status code match, e.g. "403.cshtml"
            var statusCodePath = string.Format(VIEW_FORMAT, statusCode.ToString());
            if (DoesViewExist(statusCodePath))
            {
                return statusCodePath;
            }

            // Fall back to generic error, i.e. "Error.cshtml"
            return GENERIC_ERROR_VIEW;
        }

        private bool DoesViewExist(string viewName)
        {
            var result = _razorViewEngine.GetView(null, viewName, false);

            return result.Success;
        }
    }
}
