using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Finds a user with a specific username address in a specific user area 
    /// returning null if the user could not be found. Note that depending on the
    /// user area, the username may be a copy of the email address.
    /// </summary>
    public interface IContentRepositoryUserByUsernameQueryBuilder
    {
        /// <summary>
        /// The UserMicroSummary is a minimal projection of user data that is quick
        /// to load. 
        /// </summary>
        IDomainRepositoryQueryContext<UserMicroSummary> AsMicroSummary();
    }
}
