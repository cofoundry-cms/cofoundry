using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web;

public static class IApiResponseHelperExtensions
{
    /// <summary>
    /// Formats a command response wrapping it in a SimpleCommandResponse object and setting
    /// properties based on the presence of a validation error.
    /// </summary>
    /// <param name="apiResponseHelper">
    /// <see cref="IApiResponseHelper"/> to extend.
    /// </param>
    /// <param name="validationError">Validation error, if any, to be returned.</param>
    public static JsonResult SimpleCommandResponse(this IApiResponseHelper apiResponseHelper, ValidationError validationError)
    {
        return apiResponseHelper.SimpleCommandResponse([validationError]);
    }
}
