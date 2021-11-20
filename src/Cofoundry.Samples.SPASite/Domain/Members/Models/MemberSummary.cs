using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Samples.SPASite.Domain
{
    /// <summary>
    /// A small model projection of the a member
    /// </summary>
    public class MemberSummary
    {
        public int UserId { get; set; }

        public string Name { get; set; }
    }
}
