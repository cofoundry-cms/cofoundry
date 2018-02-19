using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Finds a user by it's database id, returning a UserSummary object if it 
    /// is found, otherwise null.
    /// </summary>
    public class GetUserSummaryByIdQuery : IQuery<UserSummary>
    {
        public GetUserSummaryByIdQuery()
        {
        }

        /// <summary>
        /// Initializes the query with the specified user id.
        /// </summary>
        /// <param name="userId">Database id of the user.</param>
        public GetUserSummaryByIdQuery(int userId)
        {
            UserId = userId;
        }

        /// <summary>
        /// Database id of the user.
        /// </summary>
        public int UserId { get; set; }
    }
}
