using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cofoundry.Web;

/// <summary>
/// Helper for working with strings that are expected to contain urls
/// for internal redirects
/// </summary>
public class RedirectUrlHelper
{
    /// <summary>
    /// Validates that a <paramref name="url"/> is valid for internal
    /// redirection, ensuring that it is a local url and is not the same
    /// as the current path.
    /// </summary>
    /// <param name="controller">Currently executing controller instance to use in validation.</param>
    /// <param name="url">Url to validate.</param>
    public static bool IsValid(ControllerBase controller, string url)
    {
        return IsValidInternal(controller.Request, controller.Url, url);
    }

    /// <summary>
    /// Validates that a <paramref name="url"/> is valid for internal
    /// redirection, ensuring that it is a local url and is not the same
    /// as the current path.
    /// </summary>
    /// <param name="pageModel">Currently executing razor page instance to use in validation.</param>
    /// <param name="url">Url to validate.</param>
    public static bool IsValid(PageModel pageModel, string url)
    {
        return IsValidInternal(pageModel.Request, pageModel.Url, url);
    }

    /// <summary>
    /// Finds the "ReturnUrl" query parameter, returning the value if it's valid
    /// for redirection; otherwise <see langword="null"/> is returned.
    /// </summary>
    /// <param name="controller">Currently executing controller instance to use in validation.</param>
    public static string GetAndValidateReturnUrl(ControllerBase controller)
    {
        return GetAndValidateReturnUrlInternal(controller.Request, controller.Url);
    }

    /// <summary>
    /// Finds the "ReturnUrl" query parameter, returning the value if it's valid
    /// for redirection; otherwise <see langword="null"/> is returned.
    /// </summary>
    /// <param name="pageModel">Currently executing razor page instance to use in validation.</param>
    public static string GetAndValidateReturnUrl(PageModel pageModel)
    {
        return GetAndValidateReturnUrlInternal(pageModel.Request, pageModel.Url);
    }

    private static bool IsValidInternal(HttpRequest request, IUrlHelper urlHelper, string url)
    {
        var isValid = !string.IsNullOrEmpty(url)
            && urlHelper.IsLocalUrl(url)
            && !RelativePathHelper.IsWellFormattedAndEqual(request.Path, url);

        return isValid;
    }

    private static string GetAndValidateReturnUrlInternal(HttpRequest request, IUrlHelper urlHelper)
    {
        var returnUrl = request.Query["ReturnUrl"].FirstOrDefault();

        return IsValidInternal(request, urlHelper, returnUrl) ? returnUrl : null;
    }
}
