using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.Validation;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Updates user auditing information in the database to record 
    /// the successful login. Does not do anything to login a user
    /// session.
    /// </summary>
    public class LogSuccessfulLoginCommand : ICommand
    {
        /// <summary>
        /// The database id of the user to mark as logged in.
        /// </summary>
        [PositiveInteger]
        public int UserId { get; set; }
    }
}
