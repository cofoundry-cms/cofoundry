using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Simple mapper for mapping to UserMicroSummary objects.
    /// </summary>
    public interface IUserMicroSummaryMapper
    {
        /// <summary>
        /// Maps an EF user record from the db into a UserMicroSummary object. If the
        /// db record is null then null is returned.
        /// </summary>
        /// <param name="dbUser">User record from the database</param>
        UserMicroSummary Map(User dbUser);
    }
}
