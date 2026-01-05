using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web;

/// <summary>
/// Extension methods for <see cref="IApiResponseHelper"/>.
/// </summary>
public static class IApiResponseHelperExtensions
{
    extension(IApiResponseHelper apiResponseHelper)
    {
        /// <summary>
        /// Formats a command response wrapping it in a SimpleCommandResponse object and setting
        /// properties based on the presence of a validation error.
        /// </summary>
        /// <param name="validationError">Validation error, if any, to be returned.</param>
        public JsonResult SimpleCommandResponse(ValidationError validationError)
        {
            return apiResponseHelper.SimpleCommandResponse([validationError]);
        }
    }
}
