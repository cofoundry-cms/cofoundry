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
    /// Finds a user by it's database id, returning a UserDetails object if it 
    /// is found, otherwise null.
    /// </summary>
    public class GetUserDetailsByIdQuery : IQuery<UserDetails>
    {
        public GetUserDetailsByIdQuery()
        {
        }

        /// <summary>
        /// Initializes the query with the specified user id.
        /// </summary>
        /// <param name="userId">Database id of the user.</param>
        public GetUserDetailsByIdQuery(int userId)
        {
            UserId = userId;
        }

        /// <summary>
        /// Database id of the user.
        /// </summary>
        public int UserId { get; set; }
    }
}
