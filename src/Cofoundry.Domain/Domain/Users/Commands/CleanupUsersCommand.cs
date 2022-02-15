using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using System;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Domain
{
    /// <summary>
    /// General task for cleaning up stale user data. Currently this only removes data 
    /// from the <see cref="UserAuthenticationLog"/> and  <see cref="UserAuthenticationFailLog"/> tables.
    /// </summary>
    public class CleanupUsersCommand : ICommand, ILoggableCommand
    {
        /// <summary>
        /// The <see cref="IUserAreaDefinition.UserAreaCode"/> of the user area to clean up
        /// data for. User areas may have different data retention requirements so this command
        /// needs to be run for each user area separately.
        /// </summary>
        [Required]
        public string UserAreaCode { get; set; }

        /// <summary>
        /// The default retention period for stale data.
        /// </summary>
        public TimeSpan? DefaultRetentionPeriod { get; set; } = TimeSpan.FromDays(30);

        /// <summary>
        /// The amount of time to keep records in the <see cref="UserAuthenticationLog"/> tables. 
        /// Defaults to the <see cref="DefaultRetentionPeriod"/>. If set to less than zero then data
        /// is kept indefinitely.
        /// </summary>
        public TimeSpan? AuthenticationLogRetentionPeriod { get; set; }

        /// <summary>
        /// The amount of time to keep records in the <see cref="UserAuthenticationFailLog"/> tables. 
        /// Defaults to the <see cref="DefaultRetentionPeriod"/>. If set to less than zero then data
        /// is kept indefinitely.
        /// </summary>
        public TimeSpan? AuthenticationFailLogRetentionPeriod { get; set; }
    }
}