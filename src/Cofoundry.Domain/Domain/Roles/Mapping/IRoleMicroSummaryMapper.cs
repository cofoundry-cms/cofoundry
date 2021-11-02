using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Simple mapper for mapping to RoleMicroSummary objects.
    /// </summary>
    public interface IRoleMicroSummaryMapper
    {
        /// <summary>
        /// Maps an EF Role record from the db into an <see cref="RoleMicroSummary"/> 
        /// projection. If the db record is null then <see langword="null"/> is returned.
        /// </summary>
        /// <param name="dbRole">Role record from the database.</param>
        RoleMicroSummary Map(Role dbRole);

        /// <summary>
        /// Maps a <see cref="RoleDetails"/> projection (typically from the cache) into an 
        /// <see cref="RoleMicroSummary"/> projection. If <paramref name="roleDetails"/> is 
        /// <see langword="null"/> then <see langword="null"/> is returned.
        /// </summary>
        /// <param name="roleDetails"><see cref="RoleDetails"/> projection to map from.</param>
        RoleMicroSummary Map(RoleDetails roleDetails);
    }
}
