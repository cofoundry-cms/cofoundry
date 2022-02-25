using System.Threading.Tasks;

namespace Cofoundry.Web
{
    /// <summary>
    /// A view helper for providing information about the currently logged in user
    /// </summary>
    public interface ICurrentUserViewHelper
    {
        /// <summary>
        /// Returns information about the currently logged in user. If your 
        /// project has multiple user areas then this method will run on the
        /// user area marked as the default auth scheme. Once the user data is 
        /// loaded it is cached so you don't have to worry about calling this 
        /// multiple times.
        /// </summary>
        Task<ICurrentUserViewHelperContext> GetAsync();

        /// <summary>
        /// Returns information about the currently logged in user for a 
        /// specific user area. This is useful if you have multiple user
        /// areas because only one can be set as the default auth schema.
        /// Once the user data is loaded it is cached so you don't have to worry 
        /// about calling this multiple times.
        /// </summary>
        /// <param name="userAreaCode">
        /// The unique 3 letter identifier code for the user area to check for.
        /// </param>
        Task<ICurrentUserViewHelperContext> GetAsync(string userAreaCode);
    }
}