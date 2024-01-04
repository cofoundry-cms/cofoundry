using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Simple mapper for mapping to RoleDetails objects.
/// </summary>
public interface IRoleDetailsMapper
{
    /// <summary>
    /// Maps an EF Role record from the db into an RoleDetails 
    /// object. If the db record is null then null is returned.
    /// </summary>
    /// <param name="dbRole">Role record from the database.</param>
    [return: NotNullIfNotNull(nameof(dbRole))]
    RoleDetails? Map(Role? dbRole);
}
