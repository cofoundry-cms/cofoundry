using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    public partial class PageModuleTypeTemplate : ICreateAuditable
    {
        public int PageModuleTypeTemplateId { get; set; }
        public int PageModuleTypeId { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
        public virtual PageModuleType PageModuleType { get; set; }

        #region ICreateAuditable

        public System.DateTime CreateDate { get; set; }
        public int CreatorId { get; set; }
        public virtual User Creator { get; set; }

        #endregion
    }
}
