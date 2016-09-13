using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    public partial class PageTag : IEntityTag, ICreateAuditable
    {
        public int PageId { get; set; }
        public int TagId { get; set; }
        public virtual Page Page { get; set; }
        public virtual Tag Tag { get; set; }

        #region ICreateAuditable

        public System.DateTime CreateDate { get; set; }
        public int CreatorId { get; set; }
        public virtual User Creator { get; set; }

        #endregion
    }
}
