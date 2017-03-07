using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Seaches roles based on simple filter criteria and returns a paged result. 
    /// </summary>
    public class SearchRolesQuery : SimplePageableQuery, IQuery<PagedQueryResult<RoleMicroSummary>>
    {
        /// <summary>
        /// Text filtering that currently filters only on the title property.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Whether to exclude the special anonymous system role from the result 
        /// or not. This is used occasionally because the anonymous role cannot be 
        /// assigned to a user.
        /// </summary>
        public bool ExcludeAnonymous { get; set; }

        /// <summary>
        /// Roles are partitioned by UserArea so use this to filter to a single
        /// user area only.
        /// </summary>
        public string UserAreaCode { get; set; }
    }
}
