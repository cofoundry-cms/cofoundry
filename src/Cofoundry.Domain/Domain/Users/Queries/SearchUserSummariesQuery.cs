using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Searches users based on simple filter criteria and returns a paged result. 
    /// </summary>
    public class SearchUserSummariesQuery : SimplePageableQuery, IQuery<PagedQueryResult<UserSummary>>
    {
        /// <summary>
        /// Users are partitioned by UserArea and are treated as completely
        /// separate, therefore a UserAreaCode is required for the search.
        /// </summary>
        [Required]
        public string UserAreaCode { get; set; }

        /// <summary>
        /// Filter by first or last name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Filter by email address
        /// </summary>
        public string Email { get; set; }
    }
}
