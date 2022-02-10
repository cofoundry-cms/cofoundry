namespace Cofoundry.Domain
{
    public static class IUserContextExtensions
    {
        /// <summary>
        /// Indicates if the user is signed in i.e. <see cref="IUserContext.UserId"/>
        /// is not <see langword="null"/>.
        /// </summary>
        public static bool IsSignedIn(this IUserContext userContext)
        {
            return userContext.UserId.HasValue;
        }
    }
}