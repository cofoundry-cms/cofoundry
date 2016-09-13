using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    public partial class UserPasswordResetRequest
    {
        public Guid UserPasswordResetRequestId { get; set; }

        public int UserId { get; set; }

        public string Token { get; set; }

        public string IPAddress { get; set; }

        public DateTime CreateDate { get; set; }

        public bool IsComplete { get; set; }

        public User User { get; set; }
    }
}
