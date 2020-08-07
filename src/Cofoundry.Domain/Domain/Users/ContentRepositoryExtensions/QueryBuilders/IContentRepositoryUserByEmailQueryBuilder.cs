using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Finds a user with a specific email address in a specific user area 
    /// returning null if the user could not be found. Note that if the user
    /// area does not support email addresses then the email field will be empty.
    /// </summary>
    public interface IContentRepositoryUserByEmailQueryBuilder
    {
        /// <summary>
        /// The UserMicroSummary is a minimal projection of user data that is quick
        /// to load. 
        /// </summary>
        IDomainRepositoryQueryContext<UserMicroSummary> AsMicroSummary();
    }
}
