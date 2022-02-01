using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    /// <summary>
    /// A central logging table of IP addresses which supports optional
    /// hashing of the IP addresses for privacy purposes.
    /// </summary>
    public class IPAddress
    {
        /// <summary>
        /// Auto-incrementing primary key for the record.
        /// </summary>
        public long IPAddressId { get; set; }

        /// <summary>
        /// The textual representation of the address, which is 45 characters in
        /// length to support IP4 and IP6 addresses. If IP hashing is enabled then
        /// this value could be a hash.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// The date the IP address was first logged.
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// <see cref="AuthorizedTask"/> record created by this IP Address.
        /// </summary>
        public virtual ICollection<AuthorizedTask> AuthorizedTasks { get; set; } = new List<AuthorizedTask>();
    }
}