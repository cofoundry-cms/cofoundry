using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web;

public static class IApiResponseHelperExtensions
{
    /// <summary>
    /// Formats a command response wrapping it in a SimpleCommandResponse object and setting
    /// properties based on the presence of a validation error.
    /// </summary>
    /// <param name="validationErrors">Validation error, if any, to be returned.</param>
    public static JsonResult SimpleCommandResponse(this IApiResponseHelper apiResponseHelper, ValidationError validationError)
    {
        return apiResponseHelper.SimpleCommandResponse(new ValidationError[] { validationError });
    }
}
