using System;

namespace Cofoundry.Domain
{
    public static class IUserContextExtensions
    {
        /// <summary>
        /// Indicated if the user belongs to the Cofoundry user area.
        /// </summary>
        public static bool IsLoggedIn(this IUserContext userContext)
        {
            return userContext.UserId.HasValue;
        }
    }
}
