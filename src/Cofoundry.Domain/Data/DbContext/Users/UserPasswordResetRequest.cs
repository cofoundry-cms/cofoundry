using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    public partial class UserPasswordResetRequest
    {
        /// <summary>
        /// A unique identifier required to authenticate when 
        /// resetting the password.
        /// </summary>
        public Guid UserPasswordResetRequestId { get; set; }

        public int UserId { get; set; }

        /// <summary>
        /// A token used to authenticate when resetting the password.
        /// </summary>
        public string Token { get; set; }

        public string IPAddress { get; set; }

        public DateTime CreateDate { get; set; }

        public bool IsComplete { get; set; }

        public User User { get; set; }
    }
}
