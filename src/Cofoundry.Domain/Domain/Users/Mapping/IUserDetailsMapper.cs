﻿using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

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
    [return: NotNullIfNotNull(nameof(dbUser))]
    UserDetails? Map(User? dbUser);
}
