using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.Validation;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Finds a user by it's database id, returning a UserMicroSummary object if it 
    /// is found, otherwise null.
    /// </summary>
    public class GetUserMicroSummaryByIdQuery : IQuery<UserMicroSummary>
    {
        public GetUserMicroSummaryByIdQuery()
        {
        }

        /// <summary>
        /// Initializes the query with the specified user id.
        /// </summary>
        /// <param name="userId">Database id of the user.</param>
        public GetUserMicroSummaryByIdQuery(int userId)
        {
            UserId = userId;
        }

        /// <summary>
        /// Database id of the user.
        /// </summary>
        public int UserId { get; set; }
    }
}
