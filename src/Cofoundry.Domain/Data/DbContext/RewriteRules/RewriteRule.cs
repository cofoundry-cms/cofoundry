using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    public partial class RewriteRule : ICreateAuditable
    {
        public int RewriteRuleId { get; set; }
        public string WriteFrom { get; set; }
        public string WriteTo { get; set; }

        #region ICreateAuditable

        public System.DateTime CreateDate { get; set; }
        public int CreatorId { get; set; }
        public virtual User Creator { get; set; }

        #endregion
    }
}
