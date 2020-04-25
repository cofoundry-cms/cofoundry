using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    /// <summary>
    /// Rewrite rules can be used to redirect users from one url to another.
    /// This functionality is incomplete and subject to change.
    /// </summary>
    public partial class RewriteRule : ICreateAuditable
    {
        /// <summary>
        /// Identifier and database primary key.
        /// </summary>
        public int RewriteRuleId { get; set; }

        /// <summary>
        /// The incoming url to redirect from. Wildcard matching is supported
        /// by using an asterisk '*' at the end of the path.
        /// </summary>
        public string WriteFrom { get; set; }

        /// <summary>
        /// The url to rewrite to.
        /// </summary>
        public string WriteTo { get; set; }

        #region ICreateAuditable

        /// <summary>
        /// The user that created the entity.
        /// </summary>
        public virtual User Creator { get; set; }

        /// <summary>
        /// The date the entity was created.
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// The database id of the user that created the entity.
        /// </summary>
        public int CreatorId { get; set; }

        #endregion
    }
}
