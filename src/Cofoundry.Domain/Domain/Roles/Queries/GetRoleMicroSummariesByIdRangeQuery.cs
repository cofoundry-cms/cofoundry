using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Cofoundry.Domain
{
    /// <summary>
    /// <para>
    /// Finds a set of roles using a collection of database ids, returning them as a 
    /// <see cref="RoleMicroSummary"/> projection.
    /// </para>
    /// <para>
    /// Roles are cached, so repeat uses of this query is inexpensive.
    /// </para>
    /// </summary>
    public class GetRoleMicroSummariesByIdRangeQuery : IQuery<IDictionary<int, RoleMicroSummary>>
    {
        public GetRoleMicroSummariesByIdRangeQuery()
        {
        }

        /// <summary>
        /// Initializes the query with parameters.
        /// </summary>
        /// <param name="roleIds">Collection of database ids of the roles to get.</param>
        public GetRoleMicroSummariesByIdRangeQuery(
            IEnumerable<int> roleIds
            )
            : this(roleIds?.ToList())
        {
        }

        /// <summary>
        /// Initializes the query with parameters.
        /// </summary>
        /// <param name="roleIds">Collection of database ids of the roles to get.</param>
        public GetRoleMicroSummariesByIdRangeQuery(
            IReadOnlyCollection<int> roleIds
            )
        {
            if (roleIds == null) throw new ArgumentNullException(nameof(roleIds));

            RoleIds = roleIds;
        }

        /// <summary>
        /// Collection of database ids of the roles to get.
        /// </summary>
        [Required]
        public IReadOnlyCollection<int> RoleIds { get; set; }
    }
}
