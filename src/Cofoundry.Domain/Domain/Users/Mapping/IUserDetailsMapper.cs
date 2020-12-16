using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Simple mapper for mapping to UserDetails objects.
    /// </summary>
    public interface IUserDetailsMapper
    {
        /// <summary>
        /// Maps an EF user record from the db into a UserDetails object. If the
        /// db record is null then null is returned.
        /// </summary>
        /// <param name="dbUser">User record from the database.</param>
        UserDetails Map(User dbUser);
    }
}
