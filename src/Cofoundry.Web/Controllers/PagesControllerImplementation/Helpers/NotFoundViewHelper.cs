﻿using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Cofoundry.Web;

/// <summary>
/// Default implementation of <see cref="INotFoundViewHelper"/>.
/// </summary>
public class NotFoundViewHelper : INotFoundViewHelper
{
    private readonly IQueryExecutor _queryExecutor;
    private readonly IPageViewModelBuilder _pageViewModelBuilder;
    private readonly IRazorViewEngine _razorViewEngine;

    public NotFoundViewHelper(
        IQueryExecutor queryExecutor,
        IPageViewModelBuilder pageViewModelBuilder,
        IRazorViewEngine razorViewEngine
        )
    {
        _queryExecutor = queryExecutor;
        _pageViewModelBuilder = pageViewModelBuilder;
        _razorViewEngine = razorViewEngine;
    }

    /// <inheritdoc/>
    public async Task<ActionResult> GetViewAsync(Controller controller)
    {
        var vmParameters = GetViewModelBuilderParameters(controller);

        var result = await GetRewriteResultAsync(vmParameters);
        if (result != null)
        {
            return result;
        }

        var vm = await _pageViewModelBuilder.BuildNotFoundPageViewModelAsync(vmParameters);

        // in some situations the status code may not be set, so make sure it is.
        if (controller.Response.StatusCode != vm.StatusCode)
        {
            controller.Response.StatusCode = vm.StatusCode;
        }

        var viewName = FindView();
        return controller.View(viewName, vm);
    }

    private static NotFoundPageViewModelBuilderParameters GetViewModelBuilderParameters(Controller controller)
    {
        var request = controller.Request;
        var feature = controller.HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

        var vmParams = new NotFoundPageViewModelBuilderParameters()
        {
            Path = feature?.OriginalPath ?? request.Path,
            PathBase = feature?.OriginalPathBase ?? request.PathBase,
            QueryString = feature?.OriginalQueryString ?? request.QueryString.Value
        };

        return vmParams;
    }

    /// <summary>
    /// If a page isnt found, check to see if we have a redirection rule
    /// in place for the url.
    /// </summary>
    private async Task<ActionResult?> GetRewriteResultAsync(NotFoundPageViewModelBuilderParameters vmParameters)
    {
        var rewriteRule = await _queryExecutor.ExecuteAsync(new GetRewriteRuleSummaryByPathQuery()
        {
            Path = vmParameters.Path
        });

        if (rewriteRule != null)
        {
            return new RedirectResult(rewriteRule.WriteTo, true);
        }

        return null;
    }

    private string FindView()
    {
        const string GENERIC_404_VIEW = "~/Views/Shared/404.cshtml";
        const string GENERIC_NOTFOUND_VIEW = "~/Views/Shared/NotFound.cshtml";
        const string GENERIC_ERROR_VIEW = "~/Views/Shared/Error.cshtml";

        if (DoesViewExist(GENERIC_404_VIEW))
        {
            return GENERIC_404_VIEW;
        }

        if (DoesViewExist(GENERIC_NOTFOUND_VIEW))
        {
            return GENERIC_NOTFOUND_VIEW;
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
