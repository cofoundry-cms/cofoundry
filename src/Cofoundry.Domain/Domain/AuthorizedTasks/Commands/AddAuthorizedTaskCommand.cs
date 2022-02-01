using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;
using System;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Adds a new authorized task record, generating a new authorization token
    /// that can be used to re-validate the task at a later date.
    /// </summary>
    public class AddAuthorizedTaskCommand : ICommand, ILoggableCommand
    {
        /// <summary>
        /// Id of the user to scope the task to.
        /// </summary>
        [Required]
        [PositiveInteger]
        public int UserId { get; set; }

        /// <summary>
        /// The <see cref="IAuthorizedTaskTypeDefinition.AuthorizedTaskTypeCode"/> to group tasks by.
        /// </summary>
        [Required]
        public string AuthorizedTaskTypeCode { get; set; }

        /// <summary>
        /// Data to be included with the task. This might be data used to validate the
        /// task or data to be used once the task is authorized. E.g. when verifying
        /// an email address you could store the email being verified, this can later be used to 
        /// validate that the users email is the same as when the task token was generated.
        /// If you only wanted to change an email address after it has been verified, then you could
        /// store the email in <see cref="TaskData"/>, and then once verified the user can be updated 
        /// with the new email address.
        /// </summary>
        public string TaskData { get; set; }

        /// <summary>
        /// The maximum number of token generations to allow within the given <see cref="RateLimitWindow"/>.
        /// If zero or less, then rate limiting does not occur.
        /// </summary>
        public int? RateLimitQuantity { get; set; }

        /// <summary>
        /// The time-window in which to count the number of tokens generated when enforcing
        /// the <see cref="RateLimitQuantity"/>. If zero or less, then <see cref="RateLimitQuantity"/> 
        /// is applied without a time window.
        /// </summary>
        public TimeSpan? RateLimitWindow { get; set; }

        /// <summary>
        /// The length of time a token is valid for, specified as a  <see cref="TimeSpan"/>. If <see langword="null"/>,
        /// zero or less, then the task will not expire.
        /// </summary>
        public TimeSpan? ExpireAfter { get; set; }

        /// <summary>
        /// The primary key and unique identifier for the task record.
        /// </summary>
        [OutputValue]
        public Guid OutputAuthorizedTaskId { get; set; }

        /// <summary>
        /// A url-safe token used to identify and authenticate a task before it is executed. Tokens
        /// are typically formatted into a url to authorize tasks such as password resets or email
        /// verification.
        /// </summary>
        [OutputValue]
        public string OutputToken { get; set; }
    }
}