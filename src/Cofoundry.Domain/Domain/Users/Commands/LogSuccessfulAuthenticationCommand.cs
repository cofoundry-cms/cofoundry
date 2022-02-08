using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Logs user auditing information in the database to record 
    /// the successful authentication of a user account.
    /// </summary>
    public class LogSuccessfulAuthenticationCommand : ICommand
    {
        /// <summary>
        /// The database id of the user that has been successfully authenticated.
        /// </summary>
        [PositiveInteger]
        public int UserId { get; set; }
    }
}