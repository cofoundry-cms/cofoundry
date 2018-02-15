using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain;

namespace Cofoundry.Web
{
    /// <summary>
    /// A view helper for providing information about the currently logged in user
    /// </summary>
    public interface ICurrentUserViewHelper
    {
        /// <summary>
        /// Returns information about the the currently logged in user. Once
        /// the user data is loaded it is cached so you don't have to worry 
        /// about calling this multiple times.
        /// </summary>
        Task<CurrentUserViewHelperContext> GetAsync();
    }
}
