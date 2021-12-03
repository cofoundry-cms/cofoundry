using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;

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
