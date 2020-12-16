using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Simple mapper for mapping to UserSummary objects.
    /// </summary>
    public interface IUserSummaryMapper
    {
        /// <summary>
        /// Maps an EF user record from the db into a UserSummary object. If the
        /// db record is null then null is returned.
        /// </summary>
        /// <param name="dbUser">User record from the database.</param>
        UserSummary Map(User dbUser);
    }
}
