using System;

namespace Cofoundry.Domain.Data
{
    /// <summary>
    /// A logging table that record successful user authentication events.
    /// </summary>
    public class UserAuthenticationLog
    {
        /// <summary>
        /// Database id of the <see cref="UserAuthenticationLog"/>.
        /// </summary>
        public long UserAuthenticationLogId { get; set; }

        /// <summary>
        /// The <see cref="User.UserId"/> of the user that successfully authenticated.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// The <see cref="Data.User"/> that successfully authenticated.
        /// </summary>
        public virtual User User { get; set; }

        /// <summary>
        /// IP Address of the connection that authenticated the user.
        /// </summary>
        public long IPAddressId { get; set; }

        /// <summary>
        /// IP Address of the connection that authenticated the user.
        /// </summary>
        public IPAddress IPAddress { get; set; }

        /// <summary>
        /// The date and time of the authentication event.
        /// </summary>
        public DateTime CreateDate { get; set; }
    }
}