using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Simple mapper for mapping to UserAccountDetails objects.
    /// </summary>
    public interface IUserAccountDetailsMapper
    {
        /// <summary>
        /// Maps an EF user record from the db into a UserAccountDetails object. If the
        /// db record is null then null is returned.
        /// </summary>
        /// <param name="dbUser">User record from the database.</param>
        UserAccountDetails Map(User dbUser);
    }
}
